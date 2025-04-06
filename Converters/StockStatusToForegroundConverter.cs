using System;
using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace kafi.Converters;

public partial class StockStatusToForegroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is int currentStock)
        {
            value = currentStock switch
            {
                > 0 => "InStock",
                < 20 => "OutOfStock",
            };
        }

        switch (value.ToString()?.ToLower())
        {
            case "cash" or "instock":
                return new SolidColorBrush(Color.FromArgb(255, 69, 131, 83));
            case "credit":
                return new SolidColorBrush(Color.FromArgb(255, 255, 154, 0));
            case "outofstock":
                return new SolidColorBrush(Color.FromArgb(255, 197, 44, 164));
            default:
                break;
        }

        return new SolidColorBrush(Colors.Transparent);
    }
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
