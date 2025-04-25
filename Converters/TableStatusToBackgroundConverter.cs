using System;
using kafi.Models;
using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace kafi.Converters;

public partial class TableStatusToBackgroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is TableStatus status)
        {
            return status switch
            {
                TableStatus.Available => new SolidColorBrush(Colors.White),
                TableStatus.Selected => (SolidColorBrush)App.Current.Resources["SecondaryBrush"],
                TableStatus.Ordered => (SolidColorBrush)App.Current.Resources["PrimaryBrush"],
                _ => new SolidColorBrush(Colors.White),
            };
        }
        return new SolidColorBrush(Colors.White);
    }
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}