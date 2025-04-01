using System;
using Microsoft.UI.Xaml.Data;

namespace kafi.Converters
{
    public class RoleToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool isManager && isManager)
            {
                return "Manager";
            }
            return "Employee";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
