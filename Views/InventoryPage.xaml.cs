using kafi.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace kafi.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InventoryPage : Page
    {
        public InventoryViewModel ViewModel
        {
            get;
        }

        public InventoryPage()
        {
            ViewModel = new InventoryViewModel();
            this.InitializeComponent();
        }

        private void AddInventoryButton_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            AddInventoryButtonText.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void AddInventoryButton_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            AddInventoryButtonText.Foreground = new SolidColorBrush(Colors.White);
        }

        private void AddInventoryButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Overlay.Visibility = Visibility.Visible;
            var showOverlay = (Storyboard)Resources["ShowOverlayStoryboard"];
            showOverlay.Begin();

            var showSheet = (Storyboard)Resources["ShowSheetStoryboard"];
            showSheet.Begin();
        }

        private void CloseSheetButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var hideSheet = (Storyboard)Resources["HideSheetStoryboard"];
            hideSheet.Completed += (s, args) =>
            {
                var hideOverlay = (Storyboard)Resources["HideOverlayStoryboard"];
                hideOverlay.Completed += (s2, args2) =>
                {
                    Overlay.Visibility = Visibility.Collapsed;
                };
                hideOverlay.Begin();
            };
            hideSheet.Begin();
        }

        private void Overlay_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            CloseSheetButton_Click(sender, e);
        }
    }
}
