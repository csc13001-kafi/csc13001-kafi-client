using System;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using kafi.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Text;
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
            WeakReferenceMessenger.Default.Register<ValueChangedMessage<string>>(this, (recipient, message) =>
            {
                if (message.Value == "")
                {
                    ViewEmployeePopup_Closed(null, null);
                }
            });
            if (ViewModel.LoadEmployeesCommand.CanExecute(null))
                await ViewModel.LoadEmployeesCommand.ExecuteAsync(null);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            WeakReferenceMessenger.Default.Unregister<ValueChangedMessage<string>>(this);
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
            TurnOnDateOrTimePicker(EditBirthdatePicker);
            TurnOnDateOrTimePicker(StartShiftTimePicker);
            TurnOnDateOrTimePicker(EndShiftTimePicker);
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
            TurnOffDateOrTimePicker(EditBirthdatePicker);
            TurnOffDateOrTimePicker(StartShiftTimePicker);
            TurnOffDateOrTimePicker(EndShiftTimePicker);
            EditEmployeeButton.Visibility = Visibility.Visible;
            PostEditGrid.Visibility = Visibility.Collapsed;
        }

        private void TurnOnDateOrTimePicker(object picker)
        {
            if (picker is CalendarDatePicker datePicker)
            {
                datePicker.IsHitTestVisible = true;
                datePicker.Background = new SolidColorBrush(Color.FromArgb(255, 240, 240, 240));
            }
            else if (picker is TimePicker timePicker)
            {
                timePicker.IsHitTestVisible = true;
                timePicker.Background = new SolidColorBrush(Color.FromArgb(255, 240, 240, 240));
            }
        }

        private void TurnOffDateOrTimePicker(object picker)
        {
            if (picker is CalendarDatePicker datePicker)
            {
                datePicker.IsHitTestVisible = false;
                datePicker.Background = new SolidColorBrush(Colors.Transparent);
            }
            else if (picker is TimePicker timePicker)
            {
                timePicker.IsHitTestVisible = false;
                timePicker.Background = new SolidColorBrush(Colors.Transparent);
            }
        }

        private async void ChangeEmployeePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            var oldPasswordBox = new PasswordBox { PlaceholderText = "Old Password" };
            var newPasswordBox = new PasswordBox { PlaceholderText = "New Password" };
            var confirmPasswordBox = new PasswordBox { PlaceholderText = "Confirm Password" };

            var oldPasswordError = new TextBlock
            {
                Foreground = new SolidColorBrush(Colors.Red),
                FontSize = 12,
                Visibility = Visibility.Collapsed
            };
            var newPasswordError = new TextBlock
            {
                Foreground = new SolidColorBrush(Colors.Red),
                FontSize = 12,
                Visibility = Visibility.Collapsed
            };
            var confirmPasswordError = new TextBlock
            {
                Foreground = new SolidColorBrush(Colors.Red),
                FontSize = 12,
                Visibility = Visibility.Collapsed
            };
            var submitError = new TextBlock
            {
                Foreground = new SolidColorBrush(Colors.Red),
                FontSize = 14,
                HorizontalAlignment = HorizontalAlignment.Center,
                Visibility = Visibility.Collapsed
            };

            var panel = new StackPanel { Spacing = 10 };

            panel.Children.Add(new TextBlock { Text = "Old Password:", FontWeight = FontWeights.Bold });
            panel.Children.Add(oldPasswordBox);
            panel.Children.Add(oldPasswordError);

            panel.Children.Add(new TextBlock { Text = "New Password:", FontWeight = FontWeights.Bold });
            panel.Children.Add(newPasswordBox);
            panel.Children.Add(newPasswordError);

            panel.Children.Add(new TextBlock { Text = "Confirm Password:", FontWeight = FontWeights.Bold });
            panel.Children.Add(confirmPasswordBox);
            panel.Children.Add(confirmPasswordError);

            panel.Children.Add(submitError);

            var progressRing = new ProgressRing
            {
                IsActive = true,
                Visibility = Visibility.Collapsed,
                Width = 40,
                Height = 40,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            var container = new Grid();
            container.Children.Add(panel);
            container.Children.Add(progressRing);

            var changePasswordDialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = "Change Password",
                Content = container,
                PrimaryButtonText = "Change",
                CloseButtonText = "Cancel",
            };

            changePasswordDialog.PrimaryButtonClick += async (s, args) =>
            {
                var deferral = args.GetDeferral();

                try
                {
                    // Clear errors
                    oldPasswordError.Visibility = Visibility.Collapsed;
                    newPasswordError.Visibility = Visibility.Collapsed;
                    confirmPasswordError.Visibility = Visibility.Collapsed;
                    submitError.Visibility = Visibility.Collapsed;

                    // Sync validation
                    bool hasError = false;
                    if (string.IsNullOrWhiteSpace(oldPasswordBox.Password))
                    {
                        oldPasswordError.Text = "Old password required.";
                        oldPasswordError.Visibility = Visibility.Visible;
                        hasError = true;
                    }
                    // Add other validations...

                    if (hasError)
                    {
                        args.Cancel = true;
                        return; // Cancel closing
                    }

                    // Show loading
                    progressRing.Visibility = Visibility.Visible;
                    changePasswordDialog.IsPrimaryButtonEnabled = false;

                    // Async password change
                    string message = await ViewModel.ChangePasswordAsync(oldPasswordBox.Password, newPasswordBox.Password, confirmPasswordBox.Password);

                    if (message.Contains("error"))
                    {
                        submitError.Text = message;
                        submitError.Visibility = Visibility.Visible;
                        args.Cancel = true; // Block dialog close
                    }
                    else
                    {
                        // Let dialog close on success
                    }
                }
                finally
                {
                    progressRing.Visibility = Visibility.Collapsed;
                    changePasswordDialog.IsPrimaryButtonEnabled = true;
                    deferral.Complete();
                }
            };

            await changePasswordDialog.ShowAsync();
        }
    }
}
