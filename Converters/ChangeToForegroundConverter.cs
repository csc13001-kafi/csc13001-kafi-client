using System;
using Microsoft.UI.Xaml.Data;

namespace kafi.Converters;

public partial class ChangeToForegroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is float change)
        {
            string colorString = change > 0 ? "#4CAF50" : change < 0 ? "#F44336" : "#00000000";
            return ColorConverter.FromHex(colorString);
        }
        return ColorConverter.FromHex("#00000000");
    }
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
