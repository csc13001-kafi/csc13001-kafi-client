using System;
using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace kafi.Converters
{
    class StatusToForegroundConverter : IValueConverter
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

            switch (value)
            {
                case "Cash" or "InStock":
                    return new SolidColorBrush(Color.FromArgb(255, 69, 131, 83));
                case "QR":
                    return new SolidColorBrush(Color.FromArgb(255, 255, 154, 0));
                case "OutOfStock":
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
}
