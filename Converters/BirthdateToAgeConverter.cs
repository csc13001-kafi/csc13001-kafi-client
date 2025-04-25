using System;
using Microsoft.UI.Xaml.Data;

namespace kafi.Converters;

public partial class BirthdateToAgeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is DateTime birthdate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthdate.Year;
            if (birthdate.Date > today.AddYears(-age))
            {
                age--;
            }
            return age;
        }
        return 0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
