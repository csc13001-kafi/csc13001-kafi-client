using System;
using System.Linq;
using Microsoft.UI.Xaml.Data;

namespace kafi.Converters
{
    public class IdToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Guid id)
            {
                string idString = id.ToString().ToUpper();
                return idString.Length > 5 ? idString.Substring(0, 5) : idString;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
