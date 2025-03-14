using System.Diagnostics;
using kafi.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace kafi.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        public LoginViewModel ViewModel { get; }
        public LoginPage()
        {
            this.InitializeComponent();
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

        private void PasswordBox_PasswordChanged(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Debug.WriteLine($"Password changed: {ViewModel.Password}");
        }

        private void Username_TextChanged(object sender, TextChangedEventArgs e)
        {
            Debug.WriteLine($"Username changed: {ViewModel.UserName}");
        }
    }
}