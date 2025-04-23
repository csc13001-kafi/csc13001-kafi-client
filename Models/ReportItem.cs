using Microsoft.UI;
using Microsoft.UI.Xaml.Media;

namespace kafi.Models;

public class ReportItem
{
    public string Name { get; set; } = string.Empty;
    public int Value { get; set; }
    public float Change { get; set; } = 0;
    public string Icon { get; set; } = string.Empty;
    public SolidColorBrush Color { get; set; } = new SolidColorBrush(Colors.White);
    public bool IsMoney { get; set; } = false;
}
