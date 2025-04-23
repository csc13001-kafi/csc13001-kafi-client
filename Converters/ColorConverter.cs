using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace kafi.Converters;

internal class ColorConverter
{
    public static SolidColorBrush FromHex(string hex)
    {
        hex = hex.TrimStart('#');

        byte a = 255; // Default alpha value
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

        if (hex.Length == 8)
        {
            a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
        }

        return new SolidColorBrush(Color.FromArgb(a, r, g, b));
    }
}
