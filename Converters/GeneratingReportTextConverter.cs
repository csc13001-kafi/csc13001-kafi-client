using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace kafi.Converters;

public class GeneratingReportTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool isGenerating)
        {
            return isGenerating ? "Đang tạo báo cáo..." : "Tạo báo cáo phân tích";
        }
        return "Tạo báo cáo phân tích";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}