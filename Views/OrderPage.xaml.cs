using System;
using kafi.Helpers;
using kafi.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace kafi.Views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class OrderPage : Page
{
    public OrderViewModel ViewModel { get; }
    public OrderPage()
    {
        ViewModel = (OrderViewModel)App.Services.GetService(typeof(OrderViewModel))!;
        this.NavigationCacheMode = NavigationCacheMode.Enabled;
        this.InitializeComponent();
        var proxy = (BindingProxy)Resources["OrderProxy"];
        proxy.Data = ViewModel;
    }

    protected async override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (ViewModel.LoadDataCommand.CanExecute(null))
            await ViewModel.LoadDataCommand.ExecuteAsync(null);
    }

    private void ViewOrderButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ViewOrderPopup.Height = XamlRoot.Size.Height - 20;
        ViewOrderPopup.IsOpen = true;

        var storyboard = new Storyboard();
        var animation = new DoubleAnimation
        {
            From = 300,
            To = 0,
            Duration = new Duration(TimeSpan.FromMilliseconds(300)),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };

        Storyboard.SetTarget(animation, PopupTranslateTransform);
        Storyboard.SetTargetProperty(animation, "X");
        storyboard.Children.Add(animation);
        storyboard.Begin();
    }

    private void ClosePopupButton_Click(object sender, RoutedEventArgs e)
    {
        var closeStoryboard = new Storyboard();
        var closeAnimation = new DoubleAnimation
        {
            From = 0,
            To = 300,
            Duration = new Duration(TimeSpan.FromMilliseconds(300)),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
        };

        Storyboard.SetTarget(closeAnimation, PopupTranslateTransform);
        Storyboard.SetTargetProperty(closeAnimation, "X");
        closeStoryboard.Children.Add(closeAnimation);

        closeStoryboard.Completed += (s, e) =>
        {
            ViewOrderPopup.IsOpen = false;
        };

        closeStoryboard.Begin();
    }

    private void Grid_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        var storyboard = new Storyboard();
        var fadeIn = new DoubleAnimation
        {
            To = 1,
            Duration = new Duration(TimeSpan.FromMilliseconds(100))
        };

        Storyboard.SetTarget(fadeIn, ClosePopupButton);
        Storyboard.SetTargetProperty(fadeIn, "Opacity");

        storyboard.Children.Add(fadeIn);
        storyboard.Begin();
    }

    private void Grid_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        var storyboard = new Storyboard();
        var fadeOut = new DoubleAnimation
        {
            To = 0,
            Duration = new Duration(TimeSpan.FromMilliseconds(100))
        };

        Storyboard.SetTarget(fadeOut, ClosePopupButton);
        Storyboard.SetTargetProperty(fadeOut, "Opacity");

        storyboard.Children.Add(fadeOut);
        storyboard.Begin();
    }
}
