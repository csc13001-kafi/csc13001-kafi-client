using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using kafi.Models;
using Microsoft.UI.Xaml.Media;

namespace kafi.ViewModels;

public partial class NumericReportItemViewModel(ReportItem reportItem) : ObservableObject
{
    private ReportItem Model { get; } = reportItem;

    public string Name => Model.Name;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FormattedValue))]
    public partial int Value { get; set; } = reportItem.Value;
    [ObservableProperty]
    public partial float Change { get; set; } = reportItem.Change;

    public bool IsMoney => Model.IsMoney;

    public string Icon => Model.Icon;
    public SolidColorBrush Color => Model.Color;
    public string FormattedValue
    {
        get
        {
            if (IsMoney)
            {
                return Value.ToString("N0", new CultureInfo("vi-VN")) + " ₫";
            }
            else
            {
                return Value.ToString();
            }
        }
    }
}
