using System;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;

namespace kafi.Converters;

public partial class ImageStringToImageSourceConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is string imagePath)
        {
            var imageSource = new BitmapImage
            {
                UriSource = new Uri(imagePath)
            };
            return imageSource;
        }
        return new BitmapImage();
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
