using System;
using kafi.Models;
using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace kafi.Converters;

public partial class TableStatusToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is TableStatus status)
        {
            if (parameter is string str && str == "border")
            {
                return status switch
                {
                    TableStatus.Available => (SolidColorBrush)App.Current.Resources["PrimaryBrush"],
                    TableStatus.Selected => new SolidColorBrush(Colors.White),
                    TableStatus.Ordered => (SolidColorBrush)App.Current.Resources["SecondaryBrush"],
                    _ => (SolidColorBrush)App.Current.Resources["PrimaryBrush"],
                };
            }
            return status switch
            {
                TableStatus.Ordered => (SolidColorBrush)App.Current.Resources["SecondaryBrush"],
                _ => (SolidColorBrush)App.Current.Resources["PrimaryBrush"],
            };
        }
        return (SolidColorBrush)App.Current.Resources["PrimaryBrush"];
    }
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
