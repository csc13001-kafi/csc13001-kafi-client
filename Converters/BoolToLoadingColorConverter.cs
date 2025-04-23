using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;

namespace kafi.Converters
{
    public class BoolToLoadingColorConverter : IValueConverter
    {
        // Define colors directly with hex values
        private static readonly SolidColorBrush NormalBrush = new SolidColorBrush(Microsoft.UI.Colors.White);
        private static readonly SolidColorBrush LoadingBrush = new SolidColorBrush(
            Microsoft.UI.ColorHelper.FromArgb(255, 255, 224, 130)); // #FFE082

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool isLoading)
            {
                return isLoading ? LoadingBrush : NormalBrush;
            }
            
            return NormalBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
} 