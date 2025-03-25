using System;
using System.Globalization;
using Microsoft.UI.Xaml.Data;

namespace kafi.Converters
{
    public class PriceToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int price)
            {
                return price.ToString("N0", new CultureInfo("vi-VN")) + " ₫";
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
