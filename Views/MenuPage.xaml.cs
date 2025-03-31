using System;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using kafi.Helpers;
using kafi.Models;
using kafi.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace kafi.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MenuPage : Page
    {
        public MenuViewModel ViewModel { get; }

        public MenuPage()
        {
            ViewModel = (MenuViewModel)App.Services.GetService(typeof(MenuViewModel))!;
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            this.InitializeComponent();
            var proxy = (BindingProxy)Resources["MenuProxy"];
            proxy.Data = ViewModel;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            WeakReferenceMessenger.Default.Register<ValueChangedMessage<string>>(this, (recipient, message) =>
            {
                if (message.Value == "")
                {
                    ClosePopupButton_Click(null, null);
                }
            });
            if (ViewModel.LoadDataCommand.CanExecute(null))
                await ViewModel.LoadDataCommand.ExecuteAsync(null);
            if (ViewModel.LoadMaterialsCommand.CanExecute(null))
                await ViewModel.LoadMaterialsCommand.ExecuteAsync(null);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            WeakReferenceMessenger.Default.Unregister<ValueChangedMessage<string>>(this);
        }

        private const int MinColumnSpacing = 20;
        private const int MinItemWidth = 200;
        private void Category_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is not ItemsView itemsView)
            {
                return;
            }

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
            if (sender.SelectedItem is not Category category)
                return;
            if (category == ViewModel.SelectedCategory)
                return;

            ViewModel.FilterByCategoryCommand.Execute(category);
        }

        private void AddProductButton_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            AddProductButtonText.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void AddProductButton_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            AddProductButtonText.Foreground = new SolidColorBrush(Colors.White);
        }
        private void AddButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
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
        private void DeleteAllInputsButton_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (sender is Button button)
            {
                var textBlock = (TextBlock)button.Content;
                textBlock.Foreground = (SolidColorBrush)App.Current.Resources["PrimaryBrush"];
            }
        }

        private void DeleteAllInputsButton_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (sender is Button button)
            {
                var textBlock = (TextBlock)button.Content;
                textBlock.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void AddPopup_Closed(object sender, object e)
        {
            if (ViewModel.IsAddingCategory)
                ViewModel.IsAddingCategory = false;

            if (ViewModel.IsAddingProduct)
                ViewModel.IsAddingProduct = false;

            if (ViewModel.IsEditingCategory)
            {
                ViewModel.DeleteAllInputCommand.Execute(null);
                ViewModel.IsEditingCategory = false;
            }

            if (ViewModel.IsEditingProduct)
            {
                ViewModel.IsEditingProduct = false;
                ViewModel.DeleteAllInputCommand.Execute(null);
            }
        }
    }
}