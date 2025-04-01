using System;
using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace kafi.Converters
{
    public class PaymentTypeToForeground : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string type && parameter is string paymentType)
            {
                if (type == paymentType)
                {
                    return new SolidColorBrush(Colors.White);
                }
                return new SolidColorBrush(Colors.Black);

            }
            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
