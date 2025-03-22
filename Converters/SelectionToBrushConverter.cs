using System;
using kafi.Models;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace kafi.Converters
{
    public class SelectionToBrushConverter : IValueConverter
    {
        public Brush DefaultBrush { get; set; }
        public Brush SelectedBrush { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Category category && parameter is Category selectedCategory &&
            category.Equals(selectedCategory))
            {
                return SelectedBrush;
            }
            return DefaultBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
