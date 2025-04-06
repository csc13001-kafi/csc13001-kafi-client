using System;
using Microsoft.UI.Xaml.Data;

namespace kafi.Converters;

public partial class DateTimeToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is DateTime dateTime)
        {
            if (parameter != null && parameter.ToString()?.ToLower() == "day")
            {
                var dayOfWeek = dateTime.ToString("ddd", new System.Globalization.CultureInfo("vi-VN"));
                if (dayOfWeek == "CN")
                    dayOfWeek = "Chủ nhật";
                else
                    dayOfWeek = string.Concat("Thứ ", dayOfWeek.AsSpan(3));
                return $"{dayOfWeek}, {dateTime:dd/MM/yyyy - HH:mm}";
            }
            return dateTime.ToString("dd/MM/yyyy - HH:mm");
        }

        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
