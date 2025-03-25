using System;
using kafi.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace kafi.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EmployeePage : Page
    {
        EmployeeViewModel ViewModel { get; }
        public EmployeePage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            ViewModel = (EmployeeViewModel)App.Services.GetService(typeof(EmployeeViewModel))!;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (ViewModel.LoadEmployeesCommand.CanExecute(null))
                await ViewModel.LoadEmployeesCommand.ExecuteAsync(null);
        }

        private void AddEmployeeButton_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            AddInventoryButtonText.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void AddEmployeeButton_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            AddInventoryButtonText.Foreground = new SolidColorBrush(Colors.White);
        }

        private void AddEmployeeButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            AddEmployeePopup.Height = XamlRoot.Size.Height - 20;
            AddEmployeePopup.IsOpen = true;

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
                AddEmployeePopup.IsOpen = false;
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

        private void BirthdatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            var selectedDate = sender.Date;
            var currentDate = DateTimeOffset.Now;
            if (selectedDate > currentDate)
            {
                sender.Date = currentDate;
            }
        }

        private void ViewEmployeeButton_Click(object sender, RoutedEventArgs e)
        {

            ViewEmployeePopup.Height = XamlRoot.Size.Height - 120;
            ViewEmployeePopup.Width = this.ActualWidth - 80;
            ViewEmployeePopup.IsOpen = true;

            var storyboard = new Storyboard();
            var animation = new DoubleAnimation
            {
                From = -600,
                To = 0,
                Duration = new Duration(TimeSpan.FromMilliseconds(500)),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            Storyboard.SetTarget(animation, PopupTranslateYTransform);
            Storyboard.SetTargetProperty(animation, "Y");
            storyboard.Children.Add(animation);
            storyboard.Begin();
        }

        private void CloseViewPopupButton_Click(object sender, RoutedEventArgs e)
        {
            var closeStoryboard = new Storyboard();
            var closeAnimation = new DoubleAnimation
            {
                From = 0,
                To = -600,
                Duration = new Duration(TimeSpan.FromMilliseconds(500)),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
            };

            Storyboard.SetTarget(closeAnimation, PopupTranslateYTransform);
            Storyboard.SetTargetProperty(closeAnimation, "Y");
            closeStoryboard.Children.Add(closeAnimation);

            closeStoryboard.Completed += (s, e) =>
            {
                ViewEmployeePopup.IsOpen = false;
            };

            closeStoryboard.Begin();
        }

        private void EditEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            TurnOffIsReadOnlyTextBoxes(PersonalInfoGrid);
            TurnOffIsReadOnlyTextBoxes(JobInfoGrid);
            EditBirthdatePicker.IsHitTestVisible = true;
            EditBirthdatePicker.Background = new SolidColorBrush(Color.FromArgb(255, 240, 240, 240));
            EditEmployeeButton.Visibility = Visibility.Collapsed;
            PostEditGrid.Visibility = Visibility.Visible;
        }
        private void TurnOnIsReadOnlyTextBoxes(Grid grid)
        {
            foreach (var child in grid.Children)
            {
                if (child is not TextBox textBox)
                    continue;

                if (textBox.Name == "RoleTextBox")
                    continue;

                textBox.IsReadOnly = true;
                textBox.Background = new SolidColorBrush(Colors.Transparent);
            }
        }
        private void TurnOffIsReadOnlyTextBoxes(Grid grid)
        {
            foreach (var child in grid.Children)
            {
                if (child is not TextBox textBox)
                    continue;

                if (textBox.Name == "RoleTextBox")
                    continue;

                textBox.IsReadOnly = false;
                textBox.Background = new SolidColorBrush(Color.FromArgb(255, 240, 240, 240));
            }
        }

        private void ViewEmployeePopup_Closed(object sender, object e)
        {
            TurnOnIsReadOnlyTextBoxes(PersonalInfoGrid);
            TurnOnIsReadOnlyTextBoxes(JobInfoGrid);
            EditBirthdatePicker.IsHitTestVisible = false;
            EditBirthdatePicker.Background = new SolidColorBrush(Colors.Transparent);
            EditEmployeeButton.Visibility = Visibility.Visible;
            PostEditGrid.Visibility = Visibility.Collapsed;
        }
    }
}
