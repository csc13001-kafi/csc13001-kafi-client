using System;
using System.Globalization;
using Microsoft.UI.Xaml.Data;

namespace kafi.Converters;

public partial class PriceToStringConverter : IValueConverter
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
        if (value is string priceString)
        {
            priceString = priceString.Replace("₫", "").Trim();

            if (int.TryParse(priceString, NumberStyles.Number, new CultureInfo("vi-VN"), out int result))
            {
                return result;
            }
        }
        return 0;
    }
}
