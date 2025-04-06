using System;
using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace kafi.Converters;

public partial class StatusToBackgroundConverter : IValueConverter
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
                return new SolidColorBrush(Color.FromArgb(255, 230, 246, 233));
            case "credit":
                return new SolidColorBrush(Color.FromArgb(38, 255, 176, 116));
            case "outofstock":
                return new SolidColorBrush(Color.FromArgb(255, 246, 230, 240));
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