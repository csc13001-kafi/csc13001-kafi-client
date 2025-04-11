using System;
using System.Linq;
using Microsoft.UI.Xaml.Data;

namespace kafi.Converters;

public partial class PaymentTypeToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not string paymentType)
            return Microsoft.UI.Xaml.Visibility.Collapsed;
        var paymentTypes = parameter as string;
        if (string.IsNullOrEmpty(paymentTypes))
            return Microsoft.UI.Xaml.Visibility.Collapsed;
        var types = paymentTypes.Split(',');
        return types.Contains(paymentType) ? Microsoft.UI.Xaml.Visibility.Visible : Microsoft.UI.Xaml.Visibility.Collapsed;
    }
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
