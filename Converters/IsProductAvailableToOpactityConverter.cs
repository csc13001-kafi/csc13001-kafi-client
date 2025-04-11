using System;
using Microsoft.UI.Xaml.Data;

namespace kafi.Converters;

public partial class IsProductAvailableToOpactityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not bool isAvailable)
            return 1.0;
        return isAvailable ? 1.0 : 0.4;
    }
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
