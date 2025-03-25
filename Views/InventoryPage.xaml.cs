using System;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
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
    public sealed partial class InventoryPage : Page
    {
        public InventoryViewModel ViewModel { get; }

        public InventoryPage()
        {
            ViewModel = (InventoryViewModel)App.Services.GetService(typeof(InventoryViewModel))!;
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            this.InitializeComponent();
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
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            WeakReferenceMessenger.Default.Unregister<ValueChangedMessage<string>>(this);
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
            AddInventoryPopup.Height = XamlRoot.Size.Height - 20;
            AddInventoryPopup.IsOpen = true;

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
                AddInventoryPopup.IsOpen = false;
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

        private void AddInventoryPopup_Closed(object sender, object e)
        {
            if (ViewModel.IsEditing)
            {
                ViewModel.IsEditing = false;
                ViewModel.DeleteAllInputCommand.Execute(null);
            }

        }
    }
}
