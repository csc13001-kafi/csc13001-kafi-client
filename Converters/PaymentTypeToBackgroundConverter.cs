using System;
using kafi.Models;
using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace kafi.Converters
{
    public class PaymentTypeToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            switch (value)
            {
                case "Cash":
                    return new SolidColorBrush(Color.FromArgb(255, 230, 246, 233));
                case "QR":
                    return new SolidColorBrush(Color.FromArgb(38, 255, 176, 116));
            }
            
            return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}