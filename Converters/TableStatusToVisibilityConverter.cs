using System;
using kafi.Models;
using Microsoft.UI.Xaml.Data;

namespace kafi.Converters;

public partial class TableStatusToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is TableStatus status)
        {
            return status switch
            {
                TableStatus.Selected => Microsoft.UI.Xaml.Visibility.Visible,
                _ => Microsoft.UI.Xaml.Visibility.Collapsed,
            };
        }
        return Microsoft.UI.Xaml.Visibility.Collapsed;
    }
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
