using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace kafi.Controls
{
    public sealed partial class LoginImage : UserControl
    {
        public LoginImage()
        {
            this.InitializeComponent();
        }

        public ImageSource ImageSource
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register(
                nameof(ImageSource),
                typeof(ImageSource),
                typeof(LoginImage),
                new PropertyMetadata(null));

        public string ControlText
        {
            get => (string)GetValue(ControlTextProperty);
            set => SetValue(ControlTextProperty, value);
        }

        public static readonly DependencyProperty ControlTextProperty =
            DependencyProperty.Register(
                nameof(ControlText),
                typeof(string),
                typeof(LoginImage),
                new PropertyMetadata(string.Empty));
    }
}
