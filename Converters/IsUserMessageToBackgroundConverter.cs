using System;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace kafi.Converters;

public partial class IsUserMessageToBackgroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool isUserMessage)
        {
            return isUserMessage ? (SolidColorBrush)App.Current.Resources["PrimaryBrush"] : new SolidColorBrush(Color.FromArgb(255, 242, 243, 243));
        }
        return new SolidColorBrush(Color.FromArgb(255, 242, 243, 243));
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
