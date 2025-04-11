using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using kafi.Contracts.Services;
using kafi.Controls;
using kafi.Models;
using kafi.Repositories;
using Microsoft.UI.Xaml;

namespace kafi.ViewModels;

public partial class TableViewModel : ObservableRecipient
{
    private readonly IMenuRepository _menuRepository;
    private readonly IOrderRepository _orderRepository;
    private Window? _window;
    public string EmployeeName;

    private readonly List<Table> _tables = [
        new Table { Id = 0, Name = "Mua về", Status = TableStatus.Selected},
        new Table { Id = 1, Name = "Bàn 01"},
        new Table { Id = 2, Name = "Bàn 02"},
        new Table { Id = 3, Name = "Bàn 03"},
        new Table { Id = 4, Name = "Bàn 04"},
        new Table { Id = 5, Name = "Bàn 05"},
        new Table { Id = 6, Name = "Bàn 06"},
        new Table { Id = 7, Name = "Bàn 07"},
        new Table { Id = 8, Name = "Bàn 08"},
        new Table { Id = 9, Name = "Bàn 09"},
        new Table { Id = 10, Name = "Bàn 10"},
        new Table { Id = 11, Name = "Bàn 11"},
    ];

    public ObservableCollection<TableWrapperViewModel> Tables { get; set; }

    public TableViewModel(IMenuRepository menuRepository, IOrderRepository orderRepository, IAuthService authService)
    {
        EmployeeName = authService.CurrentUser?.Name ?? "Nhân viên";
        _menuRepository = menuRepository;
        _orderRepository = orderRepository;

        IsActive = true;

        Tables = [.. _tables.Select(t => new TableWrapperViewModel(t))];
        SelectedTable = Tables[0];
        SelectedProducts.CollectionChanged += OnSelectedProductsChanged;
        TotalPrice = SelectedProducts.Sum(p => p.TotalPrice);
        PaymentMethod = "Cash";
        SelectedProductCount = SelectedProducts.Sum(p => p.Quantity);
    }

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    public ObservableCollection<Category> Categories { get; } = [];
    public ObservableCollection<Product> FilteredProducts { get; } = [];
    private List<Product> _fullProducts = [];

    [ObservableProperty]
    public partial Category? SelectedCategory { get; set; }

    [ObservableProperty]
    public partial TableWrapperViewModel SelectedTable { get; set; }

    public ObservableCollection<SelectedProductWrapperViewModel> SelectedProducts { get; set; } = [];
    public ObservableCollection<GroupedSelectedProducts> GroupedSelectedProducts { get; } = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PriceAfterDiscount), nameof(ClientChange))]
    public partial int TotalPrice { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CheckoutCommand))]
    public partial int SelectedProductCount { get; set; }

    [ObservableProperty]
    public partial Guid OrderId { get; set; } = Guid.NewGuid();

    [ObservableProperty]
    public partial DateTime CreatedAt { get; set; } = DateTime.Now;

    [ObservableProperty]
    public partial string PaymentMethod { get; set; }

    [ObservableProperty]
    public partial string ClientPhoneNumber { get; set; } = string.Empty;

    [ObservableProperty]
    public partial int DiscountPercent { get; set; } = 0;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PriceAfterDiscount), nameof(ClientChange))]
    public partial int Discount { get; set; } = 0;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CheckoutCommand))]
    public partial bool IsCheckingout { get; set; } = false;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ClientChange))]
    public partial int ClientPayment { get; set; }

    public int PriceAfterDiscount => TotalPrice - Discount;

    public ObservableCollection<int> PaymentSuggestions { get; set; } = [];

    public int ClientChange => ClientPayment - PriceAfterDiscount;

    [ObservableProperty]
    public partial string SuccessMessage { get; set; } = string.Empty;

    private DispatcherTimer _paymentStatusTimer;
    private int _orderCode;

    private bool CanLoadData => !IsLoading && !Categories.Any();
    [RelayCommand(CanExecute = nameof(CanLoadData))]
    private async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            var response = await _menuRepository.GetCategoriesAndProducts();
            _fullProducts = response.Products;

            Categories.Clear();
            foreach (var category in response.Categories)
                Categories.Add(category);

            if (Categories.Count != 0)
            {
                SelectedCategory = Categories[0];
                FilterByCategory(SelectedCategory);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void FilterByCategory(Category? category)
    {
        if (category == null) return;

        FilteredProducts.Clear();
        var categoryProducts = _fullProducts.Where(p => p.CategoryId == category.Id);
        foreach (var product in categoryProducts)
            FilteredProducts.Add(product);

        SelectedCategory = category;
    }

    private bool CanSelectTable(TableWrapperViewModel? table) => table != null;
    [RelayCommand(CanExecute = nameof(CanSelectTable))]
    private void SelectTable(TableWrapperViewModel table)
    {
        if (SelectedTable.Id == table.Id)
            return;

        foreach (var t in Tables)
            t.Status = t.Id == table.Id ? TableStatus.Selected : TableStatus.Available;

        SelectedTable = table;
    }

    private bool CanAddProduct(Product? product) => product != null && product.IsAvailable;
    [RelayCommand(CanExecute = nameof(CanAddProduct))]
    private void AddProduct(Product product)
    {
        var selectedProduct = SelectedProducts.FirstOrDefault(p => p.Product.Id == product.Id);
        if (selectedProduct != null)
        {
            selectedProduct.Quantity++;
        }
        else
        {
            var newSelectedProduct = new SelectedProductWrapperViewModel(new SelectedProduct
            {
                Quantity = 1,
                Product = product,
            });
            SelectedProducts.Add(newSelectedProduct);
        }
    }

    [RelayCommand]
    private void IncreaseQuantity(Guid id)
    {
        var selectedProduct = SelectedProducts.FirstOrDefault(p => p.Product.Id == id);
        if (selectedProduct != null)
        {
            selectedProduct.Quantity++;
        }
    }

    private bool CanDecreaseQuantity(Guid id) => id != Guid.Empty;
    [RelayCommand]
    private void DecreaseQuantity(Guid id)
    {
        var selectedProduct = SelectedProducts.FirstOrDefault(p => p.Product.Id == id)!;
        if (selectedProduct.Quantity > 1)
        {
            selectedProduct.Quantity--;
        }
    }

    [RelayCommand]
    private void RemoveProduct(Guid id)
    {
        var selectedProduct = SelectedProducts.FirstOrDefault(p => p.Product.Id == id);
        if (selectedProduct != null)
        {
            SelectedProducts.Remove(selectedProduct);
        }
    }

    private bool CanCheckout => !IsCheckingout && SelectedProductCount > 0 && (ClientPhoneNumber == "" || IsVietnamesePhoneNumber(ClientPhoneNumber));
    [RelayCommand(CanExecute = nameof(CanCheckout))]
    private async Task Checkout()
    {
        if (IsVietnamesePhoneNumber(ClientPhoneNumber))
            ClientPhoneNumber = ConvertToZeroPrefix(ClientPhoneNumber);

        WeakReferenceMessenger.Default.Send(new ValueChangedMessage<string>("showoverlay"));
        IsCheckingout = true;
        CreatedAt = DateTime.Now;
        OrderId = Guid.NewGuid();
        SuccessMessage = string.Empty;

        try
        {
            var createOrderRequest = new CreateOrderRequest
            (
                Id: OrderId,
                Table: SelectedTable.Name!,
                EmployeeName: EmployeeName,
                CreatedAt: CreatedAt,
                ClientPhoneNumber: ClientPhoneNumber,
                PaymentMethod: PaymentMethod,
                Products: [.. SelectedProducts.Select(p => p.Product.Id)],
                Quantities: [.. SelectedProducts.Select(p => p.Quantity)]
            );

            CreateOrderResponse order = (CreateOrderResponse)await _orderRepository.Add(createOrderRequest);
            Discount = order.Discount;
            DiscountPercent = order.DiscountPercentage;

            ClientPayment = TotalPrice - Discount;
            if (PaymentMethod == "QR")
            {
                _orderCode = order.OrderCode;
                ShowQrCode(order.QrLink!);
            }
            else
            {
                SuggestPayment();
            }

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            WeakReferenceMessenger.Default.Send(new ValueChangedMessage<string>("hideoverlay"));
            CloseQrCode();
        }
        finally
        {
            IsCheckingout = false;
        }
    }

    [RelayCommand]
    private void CompleteCheckout(string PaymentMethod)
    {
        if (string.IsNullOrEmpty(PaymentMethod))
            return;
        if (PaymentMethod == "QR")
        {
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                SuccessMessage = "Thanh toán thành công! (Màn hình sẽ đóng trong 3 giây)";
            };
            timer.Start();
        }
        WeakReferenceMessenger.Default.Send(new ValueChangedMessage<string>("closepopup"));
        WeakReferenceMessenger.Default.Send(new ValueChangedMessage<string>("hideoverlay"));
        SelectedProducts.Clear();
        GroupedSelectedProducts.Clear();
        ClientPhoneNumber = string.Empty;
        TotalPrice = 0;
        PaymentMethod = "Cash";
        Discount = 0;
        DiscountPercent = 0;
        ClientPayment = 0;
    }

    private bool CanSelectPaymentMethod(string method) => !string.IsNullOrEmpty(method) && !string.Equals(method, PaymentMethod);
    [RelayCommand]
    private void SelectPaymentMethod(string method)
    {
        PaymentMethod = method;
    }

    [RelayCommand]
    private void QuickSelectClientPayment(int amount)
    {
        ClientPayment = amount;
    }

    private void SuggestPayment()
    {
        PaymentSuggestions.Clear();
        var suggestions = SuggestPayments(PriceAfterDiscount);
        foreach (var suggestion in suggestions)
            PaymentSuggestions.Add(suggestion);
    }

    private void OnSelectedProductsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
        {
            foreach (SelectedProductWrapperViewModel item in e.OldItems)
            {
                item.PropertyChanged -= OnItemPropertyChanged;
                RemoveFromGroup(item);
            }
        }

        if (e.NewItems != null)
        {
            foreach (SelectedProductWrapperViewModel item in e.NewItems)
            {
                item.PropertyChanged += OnItemPropertyChanged;
                AddToGroup(item);
            }
        }

        UpdateOrderNumbersForChange(e);
        UpdateTotalQuantity();
        UpdateTotalPrice();
    }

    private void AddToGroup(SelectedProductWrapperViewModel item)
    {
        var itemCategory = Categories.FirstOrDefault(c => c.Id == item.Product.CategoryId);
        if (itemCategory == null) return;

        var group = GroupedSelectedProducts.FirstOrDefault(g => g.CategoryName == itemCategory.Name);
        if (group == null)
        {
            group = new GroupedSelectedProducts(itemCategory.Name!, [item]);
            GroupedSelectedProducts.Add(group);
        }
        else
        {
            group.Add(item);
        }
    }
    private void RemoveFromGroup(SelectedProductWrapperViewModel item)
    {
        var itemCategory = Categories.FirstOrDefault(c => c.Id == item.Product.CategoryId);
        if (itemCategory == null) return;

        var group = GroupedSelectedProducts.FirstOrDefault(g => g.CategoryName == itemCategory.Name);
        if (group != null)
        {
            group.Remove(item);
            if (group.Count == 0)
            {
                GroupedSelectedProducts.Remove(group);
            }
        }
    }

    private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SelectedProductWrapperViewModel.Quantity))
        {
            UpdateTotalQuantity();
            UpdateTotalPrice();
        }

    }
    private void UpdateOrderNumbersForChange(NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                UpdateOrderNumbers(e.NewStartingIndex, SelectedProducts.Count);
                break;
            case NotifyCollectionChangedAction.Remove:
                UpdateOrderNumbers(e.OldStartingIndex, SelectedProducts.Count);
                break;
            case NotifyCollectionChangedAction.Move:
                int start = Math.Min(e.OldStartingIndex, e.NewStartingIndex);
                int end = Math.Max(e.OldStartingIndex, e.NewStartingIndex) + 1;
                UpdateOrderNumbers(start, end);
                break;
            case NotifyCollectionChangedAction.Reset:
                UpdateOrderNumbers(0, SelectedProducts.Count);
                break;
        }
    }
    private void UpdateOrderNumbers(int startIndex, int endIndex)
    {
        for (int i = startIndex; i < endIndex && i < SelectedProducts.Count; i++)
        {
            SelectedProducts[i].OrderNumber = i + 1;
        }
    }
    private void UpdateTotalPrice()
    {
        TotalPrice = SelectedProducts.Sum(item => item.TotalPrice);
    }

    private void UpdateTotalQuantity()
    {
        SelectedProductCount = SelectedProducts.Sum(item => item.Quantity);
    }

    private static readonly int[] Denominations = { 1000, 2000, 5000, 10000, 20000, 50000, 100000, 200000, 500000 };

    public static List<int> SuggestPayments(int totalPrice)
    {
        // Use a HashSet to avoid duplicates
        var suggestions = new HashSet<int> { totalPrice }; // Include the exact amount

        foreach (int denom in Denominations)
        {
            // If denomination is greater than or equal to the total price, add it directly
            if (denom >= totalPrice)
            {
                suggestions.Add(denom);
            }
            else
            {
                // Calculate the next multiple of this denomination above the total price
                int nextMultiple = ((totalPrice / denom) + 1) * denom;
                suggestions.Add(nextMultiple);
            }
        }

        // Convert to list and sort
        List<int> sortedSuggestions = new List<int>(suggestions);
        sortedSuggestions.Sort();

        // Optional: Limit suggestions to a reasonable maximum (e.g., 50x total price)
        sortedSuggestions.RemoveAll(s => s > totalPrice * 50);

        return sortedSuggestions;
    }

    private void ShowQrCode(string qrCodeUrl)
    {
        var qrControl = new QrCodeControl(qrCodeUrl);
        qrControl.ImageLoaded += OnQrImageLoaded;

        _window = new Window();
        _window.Closed += OnQrWindowClosed;
        _window.Title = "QR Code";
        _window.Content = qrControl;

        var qrAppWindow = _window.AppWindow;
        qrAppWindow.Resize(new Windows.Graphics.SizeInt32(514, 600));
        qrAppWindow.SetIcon("Assets\\WindowIcon.ico");

        _window.Activate();
    }
    private void OnQrImageLoaded(object sender, EventArgs e)
    {
        StartPaymentStatusPolling(_orderCode);
    }

    private void OnQrWindowClosed(object sender, WindowEventArgs e)
    {
        _paymentStatusTimer?.Stop();
        _window = null;
    }

    private void StartPaymentStatusPolling(int orderCode)
    {
        _paymentStatusTimer = new DispatcherTimer();
        _paymentStatusTimer.Interval = TimeSpan.FromSeconds(5);
        _paymentStatusTimer.Tick += async (s, e) => await CheckPaymentStatus(orderCode);
        _paymentStatusTimer.Start();
    }

    private async Task CheckPaymentStatus(int orderCode)
    {
        try
        {
            PaymentStatus status = await _orderRepository.GetPaymentStatus(orderCode);

            if (string.Equals(status.Message, "Payment successful and order created"))
            {
                _paymentStatusTimer?.Stop();
                CloseQrCode();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error checking payment status: {ex.Message}");
        }
    }

    private void CloseQrCode()
    {
        if (_window != null)
        {
            _window.Close();
            _window = null;
        }
    }

    public static bool IsVietnamesePhoneNumber(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return false;

        phone = phone.Trim();

        string pattern = @"^(0|\+84)(3[2-9]|5[25689]|7[06-9]|8[1-5]|9[0-9])\d{7}$";

        return Regex.IsMatch(phone, pattern);
    }

    public static string ConvertToZeroPrefix(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return phone;

        phone = phone.Trim();

        if (phone.StartsWith("+84"))
        {
            return "0" + phone.Substring(3);
        }

        return phone;
    }
}
