using System;
using Microsoft.UI.Xaml.Data;

namespace kafi.Converters;

public partial class ChangeToIconGlyphConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is float change)
        {
            if (change > 0)
            {
                return "\uEDDB"; // Up arrow icon
            }
            return "\uEDDC"; // Down arrow icon
        }
        return "\uEDDC";
    }
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
