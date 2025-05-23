using System;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using kafi.Contracts.Services;
using kafi.Helpers;
using kafi.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace kafi.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MenuPage : Page, IRecipient<ValueChangedMessage<string>>
{
    private Window? _window;
    public MenuViewModel ViewModel { get; }
    public MenuPage()
    {
        ViewModel = App.Services.GetRequiredService<MenuViewModel>();
        this.NavigationCacheMode = NavigationCacheMode.Enabled;
        this.InitializeComponent();
        var proxy = (BindingProxy)Resources["MenuProxy"];
        proxy.Data = ViewModel;
        _window = App.Services.GetRequiredService<IWindowService>().GetCurrentWindow();
        WeakReferenceMessenger.Default.Register<ValueChangedMessage<string>>(this);
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (ViewModel.LoadDataCommand.CanExecute(null))
            await ViewModel.LoadDataCommand.ExecuteAsync(null);
        if (ViewModel.LoadMaterialsCommand.CanExecute(null))
            await ViewModel.LoadMaterialsCommand.ExecuteAsync(null);
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        // Unregister from messages when navigating away
        WeakReferenceMessenger.Default.Unregister<ValueChangedMessage<string>>(this);
        base.OnNavigatedFrom(e);
    }

    private const int MinColumnSpacing = 20;
    private const int MinItemWidth = 200;
    private void Category_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (sender is not ItemsView)
            return;

        var minWidth = MinItemWidth * 5 + MinColumnSpacing * 4 + 20;
        var moreWidth = minWidth;
        var categoryCount = ViewModel.Categories.Count;
        if (categoryCount > 5)
        {
            for (int i = categoryCount; i > 5; i--)
            {
                moreWidth += MinItemWidth + MinColumnSpacing;
                if (e.NewSize.Width <= moreWidth)
                {
                    CategoryScrollViewer.HorizontalScrollMode = ScrollMode.Enabled;
                    CategoryScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                    return;
                }
            }
        }

        if (e.NewSize.Width <= minWidth)
        {
            CategoryScrollViewer.HorizontalScrollMode = ScrollMode.Enabled;
            CategoryScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
        }
        else
        {
            CategoryScrollViewer.HorizontalScrollMode = ScrollMode.Disabled;
            CategoryScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
        }
    }

    private void Category_SelectionChanged(ItemsView sender, ItemsViewSelectionChangedEventArgs args)
    {
        // Pass all selected items to the new multiple filter method
        ViewModel.FilterByMultipleCategoriesCommand.Execute(sender.SelectedItems);
    }

    private void AddProductButton_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        if (AddProductButtonText != null)
            AddProductButtonText.Foreground = new SolidColorBrush(Colors.Black);
    }

    private void AddProductButton_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        if (AddProductButtonText != null)
            AddProductButtonText.Foreground = new SolidColorBrush(Colors.White);
    }

    private void DeleteAllInputsButton_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        if (sender is Button button && button.Content is TextBlock textBlock)
            textBlock.Foreground = (SolidColorBrush)App.Current.Resources["PrimaryBrush"];
    }

    private void DeleteAllInputsButton_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        if (sender is Button button && button.Content is TextBlock textBlock)
            textBlock.Foreground = new SolidColorBrush(Colors.Black);
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
            AddPopup.IsOpen = false;
        };
        closeStoryboard.Begin();
    }

    private async void OpenPickerButton_Click(object sender, RoutedEventArgs e)
    {
        AddPopup.IsLightDismissEnabled = false;
        try
        {
            await ViewModel.PickImageCommand.ExecuteAsync(null);
        }
        finally
        {
            AddPopup.IsLightDismissEnabled = true;
        }
    }

    private void AddPopup_Closed(object sender, object e)
    {
        ViewModel.ResetFormCommand.Execute(null);
    }

    private void OpenPopup_Click(object sender, RoutedEventArgs e)
    {
        AddPopup.Height = XamlRoot.Size.Height - 20;
        AddPopup.IsOpen = true;

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

    public void Receive(ValueChangedMessage<string> message)
    {
        if (message.Value == "close")
        {
            ClosePopupButton_Click(this, new RoutedEventArgs());
        }
    }
}
