using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace kafi.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool boolValue)
            {
                // If parameter is provided and is "Reverse", invert the logic
                bool shouldInvert = parameter != null && parameter.ToString() == "Reverse";
                
                if (shouldInvert)
                {
                    return boolValue ? Visibility.Collapsed : Visibility.Visible;
                }
                
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is Visibility visibility)
            {
                bool shouldInvert = parameter != null && parameter.ToString() == "Reverse";
                
                if (shouldInvert)
                {
                    return visibility != Visibility.Visible;
                }
                
                return visibility == Visibility.Visible;
            }
            
            return false;
        }
    }
} 