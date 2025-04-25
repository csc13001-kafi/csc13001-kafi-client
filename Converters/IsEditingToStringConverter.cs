using System;
using Microsoft.UI.Xaml.Data;

namespace kafi.Converters;

public partial class IsEditingToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (parameter is string thing && value is bool isEditing)
        {
            return isEditing ? $"Cập nhật {thing}" : $"Thêm {thing} mới";
        }
        return "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
