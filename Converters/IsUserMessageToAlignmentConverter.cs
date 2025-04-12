using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace kafi.Converters;

public partial class IsUserMessageToAlignmentConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool isUserMessage)
        {
            return isUserMessage ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        }
        return HorizontalAlignment.Left;
    }
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
