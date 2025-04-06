using System;
using Microsoft.UI.Xaml.Data;

namespace kafi.Converters;

public partial class NotEnabledConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool isEnabled)
        {
            return !isEnabled;
        }
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
