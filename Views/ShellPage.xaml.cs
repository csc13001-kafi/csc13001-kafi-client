using System;
using System.Linq;
using System.Threading.Tasks;
using kafi.Models;
using kafi.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace kafi.Views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ShellPage : Page
{
    public ShellViewModel ViewModel { get; }
    public ShellPage()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetService(typeof(ShellViewModel)) as ShellViewModel;
        ContentFrame.Navigate(typeof(MainPage));
    }

    private void OnNavigationViewSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItem is NavItem selectedItem)
        {
            var selectedTag = selectedItem.Tag?.ToString();
            switch (selectedTag)
            {
                case "MainPage":
                    ContentFrame.Navigate(typeof(MainPage));
                    break;
                case "OrderPage":
                    ContentFrame.Navigate(typeof(OrderPage));
                    break;
                case "TablePage":
                    ContentFrame.Navigate(typeof(TablePage));
                    break;
                case "InfoPage":
                    ContentFrame.Navigate(typeof(InfoPage));
                    break;
                case "MenuPage":
                    ContentFrame.Navigate(typeof(MenuPage));
                    break;
                case "InventoryPage":
                    ContentFrame.Navigate(typeof(InventoryPage));
                    break;
                case "EmployeePage":
                    ContentFrame.Navigate(typeof(EmployeePage));
                    break;
                case "Logout":
                    ViewModel.LogoutCommand.Execute(null);
                    break;
            }
            ShowOrHideHeader(selectedTag);
        }
    }

    private async void ShowOrHideHeader(string pageTag)
    {
        var pagesWithoutHeader = new string[] { };
        var shouldShowHeader = !pagesWithoutHeader.Contains(pageTag);

        var header = FindName("PageHeader") as UIElement;
        if (header == null) return; // Ensure header exists

        if (shouldShowHeader)
        {
            // Fade in animation for the header
            var fadeIn = new DoubleAnimation()
            {
                To = 1,
                Duration = new Duration(TimeSpan.FromMilliseconds(500)),
                EasingFunction = new QuadraticEase()
            };
            Storyboard.SetTarget(fadeIn, header);
            Storyboard.SetTargetProperty(fadeIn, "Opacity");

            var sb = new Storyboard();
            sb.Children.Add(fadeIn);
            header.Visibility = Visibility.Visible;
            sb.Begin();
        }
        else
        {
            // Fade out animation for the header
            var fadeOut = new DoubleAnimation()
            {
                To = 0,
                Duration = new Duration(TimeSpan.FromMilliseconds(500)),
                EasingFunction = new QuadraticEase()
            };
            Storyboard.SetTarget(fadeOut, header);
            Storyboard.SetTargetProperty(fadeOut, "Opacity");

            var sb = new Storyboard();
            sb.Children.Add(fadeOut);
            sb.Begin();

            // Wait for animation to finish before hiding
            await Task.Delay(500);
            header.Visibility = Visibility.Collapsed;
        }
    }
}
