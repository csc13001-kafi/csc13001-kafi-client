using System;
using Microsoft.UI.Xaml.Data;

namespace kafi.Converters;

public partial class ExpiredDateToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not DateTime expiryDate)
            return "";

        DateTime now = DateTime.Now;
        if (expiryDate < now)
            return "Expired";

        int monthsDiff = (expiryDate.Year - now.Year) * 12 + expiryDate.Month - now.Month;

        if (monthsDiff == 3)
        {
            return "Còn 3 tháng";
        }
        else if (monthsDiff == 2)
        {
            return "Còn 2 tháng";
        }
        else if (monthsDiff == 1)
        {
            int days = (int)Math.Ceiling((expiryDate - now).TotalDays);
            return $"Còn {days} ngày";
        }
        else
        {
            return expiryDate.ToString("dd/MM/yyyy");
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
