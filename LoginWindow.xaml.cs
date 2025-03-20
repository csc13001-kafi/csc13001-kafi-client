using kafi.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace kafi
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginWindow : Window
    {
        public LoginViewModel ViewModel { get; }
        public LoginWindow()
        {
            this.InitializeComponent();
            AppWindow.SetIcon("/Assets/WindowIcon.ico");
            ViewModel = App.Services.GetService(typeof(LoginViewModel)) as LoginViewModel;
        }
        private void LoginButton_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            LoginButtonText.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void LoginButton_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            LoginButtonText.Foreground = new SolidColorBrush(Colors.White);
        }
    }
}
