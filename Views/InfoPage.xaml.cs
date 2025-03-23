using kafi.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace kafi.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InfoPage : Page
    {
        public InfoViewModel ViewModel { get; }

        public InfoPage()
        {
            ViewModel = App.Services.GetService(typeof(InfoViewModel)) as InfoViewModel;
            this.InitializeComponent();
            
            this.DataContext = ViewModel;
            
            Loaded += InfoPage_Loaded;
        }

        private async void InfoPage_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadUserInfoAsync();
        }

        private void ChangeProfilePictureButton_Click(object sender, RoutedEventArgs e)
        {
            //disable the button to avoid double-clicking
            var senderButton = sender as Button;
            senderButton.IsEnabled = false;

            //re-enable the button
            senderButton.IsEnabled = true;
        }
    }
}
