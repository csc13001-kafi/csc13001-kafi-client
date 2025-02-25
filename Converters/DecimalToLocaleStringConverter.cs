using System;
using Microsoft.UI.Xaml.Data;

namespace kafi.Converters
{
    class DecimalToLocaleStringConverter : IValueConverter
    {
        // 40 -> 40.000, 40.5 -> 40.500
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is decimal)
            {
                return ((decimal)value).ToString("0.000");
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is string)
            {
                return decimal.Parse((string)value);
            }
            return value;
        }
    }
}
