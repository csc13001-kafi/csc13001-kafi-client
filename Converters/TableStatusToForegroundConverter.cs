using System;
using kafi.Models;
using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace kafi.Converters;

public partial class TableStatusToForegroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is TableStatus status)
        {
            return status switch
            {
                TableStatus.Available => new SolidColorBrush(Colors.Black),
                TableStatus.Selected => new SolidColorBrush(Colors.White),
                TableStatus.Ordered => new SolidColorBrush(Colors.Black),
                _ => new SolidColorBrush(Colors.Black),
            };
        }
        return new SolidColorBrush(Colors.Black);
    }
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
