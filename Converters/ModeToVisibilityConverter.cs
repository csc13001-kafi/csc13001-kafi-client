using System;
using kafi.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace kafi.Converters;

public partial class ModeToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is Mode mode && parameter is string param)
        {
            if (param == "Category" && (mode == Mode.AddingCategory || mode == Mode.EditingCategory))
            {
                return Visibility.Visible;
            }
            if (param == "Product" && (mode == Mode.AddingProduct || mode == Mode.EditingProduct))
            {
                return Visibility.Visible;
            }
            if (param == "Add" && (mode == Mode.AddingCategory || mode == Mode.AddingProduct))
            {
                return Visibility.Visible;
            }
            if (param == "Edit" && (mode == Mode.EditingCategory || mode == Mode.EditingProduct))
            {
                return Visibility.Visible;
            }
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}