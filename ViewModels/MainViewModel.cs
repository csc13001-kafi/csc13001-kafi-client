using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using kafi.Contracts.Services;
using kafi.Converters;
using kafi.Models;
using kafi.Repositories;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace kafi.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly IAnalyticsRepository _repository;
    public bool IsManager => _authService.IsInRole(Role.Manager);
    public string Username => _authService.CurrentUser?.Name ?? "Unknown User";
    public DateTimeOffset Today => DateTime.Now;
    private List<ReportItem> GetReportItems()
    {
        return IsManager ?
        [
            new() { Icon = "/Assets/RevenueIcon.png", Name = "Doanh thu", Color = ColorConverter.FromHex("#E8F1FD"), IsMoney = true },
            new() { Icon = "/Assets/OrdersIcon.png", Name = "Số lượng đơn", Color = ColorConverter.FromHex("#FFEEDB") },
            new() { Icon = "/Assets/ProductIcon.png", Name = "Số lượng món ăn", Color = ColorConverter.FromHex("#E5F7FD") },
            new() { Icon = "/Assets/CategoriesIcon.png", Name = "Số lượng phân loại", Color = ColorConverter.FromHex("#E7E5FF") },
            new() { Icon = "/Assets/CustomerIcon.png", Name = "Khách hàng thân thiết", Color = ColorConverter.FromHex("#E5F7FD") },
            new() { Icon = "/Assets/EmployeeIcon.png", Name = "Số lượng nhân viên", Color = ColorConverter.FromHex("#E7E5FF") },
        ] :
        [
            new() { Icon = "/Assets/OrdersIcon.png", Name = "Số lượng đơn", Color = ColorConverter.FromHex("#FFEEDB") },
            new() { Icon = "/Assets/ProductIcon.png", Name = "Số lượng món ăn", Color = ColorConverter.FromHex("#E5F7FD") },
            new() { Icon = "/Assets/CategoriesIcon.png", Name = "Số lượng phân loại", Color = ColorConverter.FromHex("#E7E5FF") },
            new() { Icon = "/Assets/CustomerIcon.png", Name = "Khách hàng thân thiết", Color = ColorConverter.FromHex("#E5F7FD") },
        ];
    }

    public List<Tuple<string, string>> TimeRanges { get; } =
    [
        new Tuple<string, string> ("Today", "today"),
        new Tuple<string, string> ("This Week", "this_week"),
        new Tuple<string, string> ("This Month", "this_month"),
        new Tuple<string, string> ("Last 3 months", "three_months"),
        new Tuple<string, string> ("Last 6 months", "six_months"),
        new Tuple<string, string> ("This Year", "this_year")
    ];

    [ObservableProperty]
    public partial string SelectedTimeRange { get; set; }
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ChangeRevenueDateCommand))]
    public partial DateTimeOffset RevenueSelectedDate { get; set; }
    [ObservableProperty]
    public partial bool IsLoading { get; set; }
    [ObservableProperty]
    public partial string AnalyticReport { get; set; } = string.Empty;
    [ObservableProperty]
    public partial bool IsGeneratingReport { get; set; } = false;

    [ObservableProperty]
    public partial bool HasLowStockMaterials { get; set; }

    public ObservableCollection<NumericReportItemViewModel> Items { get; set; }
    public ObservableCollection<LowStockMaterial> LowStockMaterials { get; set; }
    public List<ISeries> RevenueSeries { get; set; } = [];
    public List<ISeries> ProductVolumeSeries { get; set; } = [];
    public List<ISeries> SaleVolumeSeries { get; set; } = [];

    private ObservableCollection<TimeSpanPoint> revenues = [];
    private ObservableCollection<TopProduct> productVolumes = [];
    private ObservableCollection<TimeSpanPoint> ordersByDayByCash = [];
    private ObservableCollection<TimeSpanPoint> ordersByDayByQr = [];
    public MainViewModel(IAuthService authService, IAnalyticsRepository repository)
    {
        _authService = authService;
        _repository = repository;
        Items = [.. GetReportItems().Select(item => new NumericReportItemViewModel(item))];
        SelectedTimeRange = TimeRanges.First().Item2;
        RevenueSelectedDate = Today;
        LowStockMaterials = [];

        // Initialize chart series
        InitializeChartSeries();
    }

    [RelayCommand]
    public async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            // Reinitialize chart series configuration to prevent data label shifting
            InitializeChartSeries();

            await ChangeTimeRangeAsync();
            if (IsManager)
            {
                Items[5].Value = (await _repository.GetEmployeesCount()).Count;
            }

            var hourlySales = await _repository.GetHourlySalesData(new DateTime(2025, 4, 11).ToString("yyyy-MM-dd"));

            revenues.Clear();
            foreach (var hourlySale in hourlySales.HourlySales)
            {
                revenues.Add(new TimeSpanPoint(TimeSpan.FromHours(hourlySale.Hour), hourlySale.Count));
            }

            var ordersCount = await _repository.GetOrdersByDayAndPaymentMethod(DateTime.Now.Month);
            ordersByDayByCash.Clear();
            ordersByDayByQr.Clear();
            foreach (var day in ordersCount.Days)
            {
                ordersByDayByCash.Add(new TimeSpanPoint(TimeSpan.FromDays(day.Day), day.Cash));
                ordersByDayByQr.Add(new TimeSpanPoint(TimeSpan.FromDays(day.Day), day.Qr));
            }

            var firstDateOfThisMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("yyyy-MM-dd");
            var today = DateTime.Now.ToString("yyyy-MM-dd");
            var topSellingProducts = await _repository.GetTopSellingProducts(10, firstDateOfThisMonth, today);

            productVolumes.Clear();
            foreach (var product in topSellingProducts.TopProducts)
            {
                productVolumes.Insert(0, new(product.ProductName, product.Quantity));
            }

            var lowStockMaterials = await _repository.GetLowStockMaterials(3);
            LowStockMaterials.Clear();
            foreach (var material in lowStockMaterials.Materials)
            {
                LowStockMaterials.Add(new LowStockMaterial
                {
                    Name = material.Name,
                    CurrentStock = material.CurrentStock,
                    OriginalStock = material.OriginalStock,
                    Unit = material.Unit,
                    PercentRemaining = material.PercentRemaining,
                    ExpiredDate = material.ExpiredDate,
                });
            }
            HasLowStockMaterials = LowStockMaterials.Count > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading data: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void InitializeChartSeries()
    {
        // Clear existing series
        RevenueSeries.Clear();
        ProductVolumeSeries.Clear();
        SaleVolumeSeries.Clear();

        // Reinitialize revenue series
        RevenueSeries.Add(new LineSeries<TimeSpanPoint>
        {
            Name = "Doanh thu",
            Values = revenues,
            Stroke = new SolidColorPaint(SKColor.Parse("#458353")) { StrokeThickness = 3 },
            Fill = new SolidColorPaint(SKColor.Parse("#FAC1D9").WithAlpha(84)),
            GeometryFill = new SolidColorPaint(SKColor.Parse("#458353")),
            GeometryStroke = new SolidColorPaint(SKColors.Black) { StrokeThickness = 2 },
        });

        // Reinitialize product volume series
        ProductVolumeSeries.Add(new RowSeries<TopProduct>
        {
            Values = productVolumes,
            Fill = new SolidColorPaint(SKColor.Parse("#458353")) { StrokeThickness = 3 },
            DataLabelsFormatter = point => $"{point.Model!.ProductName} {point.Coordinate.PrimaryValue}",
            DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.End,
            DataLabelsTranslate = new(-1, 0),
            DataLabelsPadding = new(0, 0, 6, 0),
            DataLabelsPaint = new SolidColorPaint(new SKColor(245, 245, 245)),
            YToolTipLabelFormatter = point => $"{point.Model!.ProductName} bán {point.Coordinate.PrimaryValue}",
        });

        // Reinitialize sale volume series
        SaleVolumeSeries.Add(new LineSeries<TimeSpanPoint>
        {
            Name = "Tiền mặt",
            Values = ordersByDayByCash,
            Stroke = new SolidColorPaint(SKColor.Parse("#DBA362")) { StrokeThickness = 3 },
            Fill = new SolidColorPaint(SKColor.Parse("#C5A674").WithAlpha(33)),
            GeometryFill = null,
            GeometryStroke = null
        });

        SaleVolumeSeries.Add(new LineSeries<TimeSpanPoint>
        {
            Name = "QR",
            Values = ordersByDayByQr,
            Stroke = new SolidColorPaint(SKColor.Parse("#B6D3FA")) { StrokeThickness = 3 },
            Fill = new SolidColorPaint(SKColor.Parse("#B6D3FA").WithAlpha(33)),
            GeometryFill = null,
            GeometryStroke = null
        });

        HasLowStockMaterials = LowStockMaterials.Count > 0;
    }

    private bool CanChangeTimeRange() => !string.IsNullOrEmpty(SelectedTimeRange);
    [RelayCommand(CanExecute = nameof(CanChangeTimeRange))]
    public async Task ChangeTimeRangeAsync()
    {
        try
        {
            Dashboard dashboard = await _repository.GetDashboard(SelectedTimeRange);
            if (IsManager)
            {
                Items[0].Value = dashboard.Overview.OrdersTotalPrice;
                Items[0].Change = dashboard.Overview.RevenuePercentChange;
                Items[1].Value = dashboard.Overview.OrdersCount;
                Items[1].Change = dashboard.Overview.OrdersPercentChange;
                Items[2].Value = dashboard.Product.ProductsCount;
                Items[3].Value = dashboard.Product.CategoriesCount;
                Items[4].Value = dashboard.Membership;
            }
            else
            {
                Items[0].Value = dashboard.Overview.OrdersCount;
                Items[0].Change = dashboard.Overview.OrdersPercentChange;
                Items[1].Value = dashboard.Product.ProductsCount;
                Items[2].Value = dashboard.Product.CategoriesCount;
                Items[3].Value = dashboard.Membership;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error changing time range: {ex.Message}");
        }
    }

    private bool CanChangeRevenueDate() => RevenueSelectedDate != Today;
    [RelayCommand(CanExecute = nameof(CanChangeRevenueDate))]
    public async Task ChangeRevenueDateAsync()
    {
        try
        {
            var date = RevenueSelectedDate.ToString("yyyy-MM-dd");
            var hourlySales = await _repository.GetHourlySalesData(date);
            revenues.Clear();
            foreach (var hourlySale in hourlySales.HourlySales)
            {
                revenues.Add(new TimeSpanPoint(TimeSpan.FromHours(hourlySale.Hour), hourlySale.Count));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error changing revenue date: {ex.Message}");
        }
    }

    private bool CanGenerateAnalytic() => !string.IsNullOrEmpty(SelectedTimeRange) && !IsGeneratingReport;
    [RelayCommand(CanExecute = nameof(CanGenerateAnalytic))]
    public async Task GenerateAnalyticAsync()
    {
        IsGeneratingReport = true;
        try
        {
            AnalyticReport = await _repository.GetBusinessReport(SelectedTimeRange);

            // Send a message that the report is ready to be displayed
            WeakReferenceMessenger.Default.Send(new AnalyticReportReadyMessage(AnalyticReport));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating analytic report: {ex.Message}");

            // Send the error message
            WeakReferenceMessenger.Default.Send(new AnalyticReportReadyMessage(AnalyticReport));
        }
        finally
        {
            IsGeneratingReport = false;
        }
    }

    public ICartesianAxis[] SaleVolumeXAxes { get; set; } = [
        new TimeSpanAxis(TimeSpan.FromDays(1), date => date.ToString("dd"))
    ];

    public ICartesianAxis[] SaleVolumeYAxes { get; set; } = [
        new Axis
        {
            SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#D0D3D9")),
            Position = LiveChartsCore.Measure.AxisPosition.End,
        }
    ];

    public ICartesianAxis[] RevenueXAxes { get; set; } = [
        new TimeSpanAxis(TimeSpan.FromHours(1), date => date.ToString("hh") + ":" + date.ToString("mm"))
    ];

    public ICartesianAxis[] RevenueYAxes { get; set; } = [
        new Axis
        {
            SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#D0D3D9")),
            Position = LiveChartsCore.Measure.AxisPosition.End,
        }
    ];


    public ICartesianAxis[] ProductVolumeYAxis { get; set; } = [
        new Axis
        {
            IsVisible = false,
        }
    ];

    public ICartesianAxis[] ProductVolumeXAxis { get; set; } = [
        new Axis
        {
            SeparatorsPaint = new SolidColorPaint(new SKColor(220, 220, 220))
        }
    ];
}
