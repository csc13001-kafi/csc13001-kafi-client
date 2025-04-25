using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace kafi.Converters;

public partial class IsProductAvailableToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool isAvailable)
        {
            return isAvailable ? Visibility.Collapsed : Visibility.Visible;
        }
        return Visibility.Collapsed;
    }
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }

}
