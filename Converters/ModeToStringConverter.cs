using System;
using kafi.ViewModels; // Assumes Mode enum is defined here
using Microsoft.UI.Xaml.Data;

namespace kafi.Converters;

public partial class ModeToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is Mode mode)
        {
            return mode switch
            {
                Mode.AddingCategory => "Thêm phân loại",
                Mode.EditingCategory => "Chỉnh sửa phân loại",
                Mode.AddingProduct => "Thêm sản phẩm",
                Mode.EditingProduct => "Chỉnh sửa sản phẩm",
                _ => string.Empty
            };
        }
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}