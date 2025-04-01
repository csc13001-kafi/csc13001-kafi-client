using System;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using kafi.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace kafi.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InfoPage : Page
    {
        public InfoViewModel ViewModel { get; }

        public InfoPage()
        {
            ViewModel = App.Services.GetRequiredService<InfoViewModel>();
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;

        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            WeakReferenceMessenger.Default.Register<ValueChangedMessage<string>>(this, (recipient, message) =>
            {
                if (message.Value == "")
                {
                    CancelEditInfoButton_Click(null, null);
                }
            });

            if (ViewModel.LoadUserInfoCommand.CanExecute(null))
                ViewModel.LoadUserInfoCommand.Execute(null);
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            WeakReferenceMessenger.Default.Unregister<ValueChangedMessage<string>>(this);
        }

        private void EditInfoButton_Click(object sender, RoutedEventArgs e)
        {
            TurnOffIsReadOnlyTextBoxes(PersonalInfoGrid);
            TurnOffIsReadOnlyTextBoxes(JobInfoGrid);
            TurnOnDateOrTimePicker(EditBirthdatePicker);
            EditInfoButton.Visibility = Visibility.Collapsed;
            PostEditGrid.Visibility = Visibility.Visible;
            ChangeProfileImageButton.Visibility = Visibility.Visible;
        }

        private void CancelEditInfoButton_Click(object sender, RoutedEventArgs e)
        {
            TurnOnIsReadOnlyTextBoxes(PersonalInfoGrid);
            TurnOnIsReadOnlyTextBoxes(JobInfoGrid);
            TurnOffDateOrTimePicker(EditBirthdatePicker);
            EditInfoButton.Visibility = Visibility.Visible;
            PostEditGrid.Visibility = Visibility.Collapsed;
            ChangeProfileImageButton.Visibility = Visibility.Collapsed;
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

                if (textBox.Name == "RoleTextBox" || textBox.Name == "SalaryTextBox")
                    continue;

                textBox.IsReadOnly = false;
                textBox.Background = new SolidColorBrush(Color.FromArgb(255, 240, 240, 240));
            }
        }
        private void TurnOnDateOrTimePicker(object picker)
        {
            if (picker is CalendarDatePicker datePicker)
            {
                datePicker.IsHitTestVisible = true;
                datePicker.Background = new SolidColorBrush(Color.FromArgb(255, 240, 240, 240));
            }
        }

        private void TurnOffDateOrTimePicker(object picker)
        {
            if (picker is CalendarDatePicker datePicker)
            {
                datePicker.IsHitTestVisible = false;
                datePicker.Background = new SolidColorBrush(Colors.Transparent);
            }
        }
        private async void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            var oldPasswordBox = new PasswordBox
            {
                Header = "Old Password:",
                PlaceholderText = "Old Password",
            };
            var newPasswordBox = new PasswordBox
            {
                Header = "New Password:",
                PlaceholderText = "New Password",
            };
            var confirmPasswordBox = new PasswordBox
            {
                Header = "Confirm Password:",
                PlaceholderText = "Confirm Password",
            };

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

            panel.Children.Add(oldPasswordBox);
            panel.Children.Add(oldPasswordError);

            panel.Children.Add(newPasswordBox);
            panel.Children.Add(newPasswordError);

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

        private void ChangeProfileImageButton_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (sender is Button button)
            {
                var textBlock = (TextBlock)button.Content;
                textBlock.Foreground = (SolidColorBrush)App.Current.Resources["PrimaryBrush"];
            }
        }

        private void ChangeProfileImageButton_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {

            if (sender is Button button)
            {
                var textBlock = (TextBlock)button.Content;
                textBlock.Foreground = new SolidColorBrush(Colors.Black);
            }
        }
    }
}
