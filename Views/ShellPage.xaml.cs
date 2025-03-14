using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace kafi.Views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ShellPage : Page
{
    public ShellPage()
    {
        this.InitializeComponent();
        ContentFrame.Navigate(typeof(MainPage));
    }

    private void OnNavigationViewSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItem is ViewModels.NavItem selectedItem)
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
            }
            ShowOrHideHeader(selectedTag);
        }
    }

    private async void ShowOrHideHeader(string pageTag)
    {
        var pagesWithoutHeader = new string[] { "LoginPage" };
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

    private void NavView_PaneOpening(NavigationView sender, object args)
    {
        UpdateTextVisibility(sender, Visibility.Visible); // Show text
    }

    private void NavView_PaneClosing(NavigationView sender, NavigationViewPaneClosingEventArgs args)
    {
        UpdateTextVisibility(sender, Visibility.Collapsed); // Hide text
    }

    private void UpdateTextVisibility(NavigationView navView, Visibility visibility)
    {
        Debug.WriteLine(navView.MenuItems.Count);
        foreach (var item in navView.MenuItems.OfType<NavigationViewItem>())
        {
            // Get the container (StackPanel) of the menu item
            var container = navView.ContainerFromMenuItem(item) as FrameworkElement;
            if (container != null)
            {
                Debug.WriteLine(container.Name);
                // Find the TextBlock named "ItemContent" inside the container
                var textBlock = FindVisualChildByName<TextBlock>(container, "ItemContent");
                if (textBlock != null)
                {
                    textBlock.Visibility = visibility;
                }
            }
        }
    }

    // Helper to find a named child in the visual tree
    private T FindVisualChildByName<T>(DependencyObject parent, string name) where T : FrameworkElement
    {
        if (parent == null) return null;
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i) as FrameworkElement;
            if (child != null && child is T target && child.Name == name)
            {
                return target;
            }
            var result = FindVisualChildByName<T>(child, name);
            if (result != null) return result;
        }
        return null;
    }
    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        UpdateTextVisibility(NavigationViewControl,
            NavigationViewControl.IsPaneOpen ? Visibility.Visible : Visibility.Collapsed);
    }
}
