using System;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using kafi.Models;
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
public sealed partial class TablePage : Page
{
    public TableViewModel ViewModel { get; }
    public TablePage()
    {
        ViewModel = App.Services.GetRequiredService<TableViewModel>();
        this.NavigationCacheMode = NavigationCacheMode.Enabled;
        this.InitializeComponent();
        this.SizeChanged += Page_SizeChanged;
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (ViewModel.LoadDataCommand.CanExecute(null))
            await ViewModel.LoadDataCommand.ExecuteAsync(null);

        WeakReferenceMessenger.Default.Register<ValueChangedMessage<string>>(this, (r, m) =>
        {
            if (m.Value == "closepopup")
                ClosePopupButton_Click(this, null);
        });
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        base.OnNavigatedFrom(e);
        WeakReferenceMessenger.Default.Unregister<ValueChangedMessage<string>>(this);
    }

    private void Category_SelectionChanged(ItemsView sender, ItemsViewSelectionChangedEventArgs args)
    {
        if (sender.SelectedItem is not Category category)
            return;

        ViewModel.FilterByCategoryCommand.Execute(category);
    }

    private const int MinColumnSpacing = 20;
    private const int MinItemWidth = 100;
    private void Category_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (sender is not ItemsView)
            return;
        if (e.NewSize.Width < 205)
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
    }

    private void Table_ItemInvoked(ItemsView sender, ItemsViewItemInvokedEventArgs args)
    {
        if (args.InvokedItem is not TableWrapperViewModel table)
            return;

        if (ViewModel.SelectTableCommand.CanExecute(table))
            ViewModel.SelectTableCommand.Execute(table);

        if (OpenMenuWhenTableSelectedCheckBox.IsChecked == true)
            OrderSelectorBar.SelectedItem = MenuBarItem;
    }

    private void OrderSelectorBar_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
    {
        if (sender.SelectedItem == TableBarItem)
        {
            Table.Visibility = Visibility.Visible;
            Menu.Visibility = Visibility.Collapsed;
        }
        else if (sender.SelectedItem == MenuBarItem)
        {
            Table.Visibility = Visibility.Collapsed;
            Menu.Visibility = Visibility.Visible;
        }
    }

    private void Product_ItemInvoked(ItemsView sender, ItemsViewItemInvokedEventArgs args)
    {
        if (args.InvokedItem is not Product product)
            return;

        if (ViewModel.AddProductCommand.CanExecute(product))
            ViewModel.AddProductCommand.Execute(product);
    }

    private void SelectedProductGrid_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        if (sender is not ItemContainer itemContainer)
            return;

        var grid = itemContainer.Child as Grid;
        if (grid == null)
            return;

        grid.BorderBrush = (SolidColorBrush)App.Current.Resources["SecondaryBrush"];
    }
    private void SelectedProductGrid_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        if (sender is not ItemContainer itemContainer)
            return;

        var grid = itemContainer.Child as Grid;
        if (grid == null)
            return;

        grid.BorderBrush = new SolidColorBrush(Colors.Transparent);
    }

    private void CheckoutButton_Click(object sender, RoutedEventArgs e)
    {
        CheckoutPopup.Height = XamlRoot.Size.Height - 20;
        CheckoutPopup.IsOpen = true;

        var storyboard = new Storyboard();
        var animation = new DoubleAnimation
        {
            From = 600,
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
            To = 600,
            Duration = new Duration(TimeSpan.FromMilliseconds(300)),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
        };

        Storyboard.SetTarget(closeAnimation, PopupTranslateTransform);
        Storyboard.SetTargetProperty(closeAnimation, "X");
        closeStoryboard.Children.Add(closeAnimation);

        closeStoryboard.Completed += (s, e) =>
        {
            CheckoutPopup.IsOpen = false;
        };

        closeStoryboard.Begin();
    }

    private void CashGrid_PointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        string paymentMethod = "Cash";
        if (ViewModel.SelectPaymentMethodCommand.CanExecute(paymentMethod))
            ViewModel.SelectPaymentMethodCommand.Execute(paymentMethod);
    }

    private void QrGrid_PointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        string paymentMethod = "QR";
        if (ViewModel.SelectPaymentMethodCommand.CanExecute(paymentMethod))
            ViewModel.SelectPaymentMethodCommand.Execute(paymentMethod);
    }

    private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (CheckoutPopup.IsOpen)
        {
            CheckoutPopup.Height = XamlRoot.Size.Height - 20;
        }
    }
}
