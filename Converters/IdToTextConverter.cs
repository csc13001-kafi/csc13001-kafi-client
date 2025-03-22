using System;
using System.Linq;
using Microsoft.UI.Xaml.Data;

namespace kafi.Converters
{
    public class IdToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string id = (string)value;
            if (!id.Contains('-'))
            {
                return id;
            }
            string[] splits = id.Split('-');
            return splits.Last();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
