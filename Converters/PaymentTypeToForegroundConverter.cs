using System;
using kafi.Models;
using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace kafi.Converters
{
    class PaymentTypeToForegroundConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            switch (value)
            {
                case "Cash":
                    return new SolidColorBrush(Color.FromArgb(255, 69, 131, 83));
                case "QR":
                    return new SolidColorBrush(Color.FromArgb(255, 255, 154, 0));
            }
            
            return new SolidColorBrush(Colors.Transparent);
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
