using System;
using Microsoft.UI.Xaml.Data;

namespace kafi.Converters;

public partial class PhoneToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not string phone)
            return "Khách lẻ";
        if (string.IsNullOrEmpty(phone))
            return "Khách lẻ";
        if (phone.Length < 10)
            return "Khách lẻ";
        var formattedPhone = $"{phone[..4]} {phone[4..7]} {phone[7..]}";
        return formattedPhone;
    }
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
