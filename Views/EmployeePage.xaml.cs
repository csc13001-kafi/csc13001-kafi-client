using System;
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

        private async void DataGrid_CellEditEnded(object sender, CommunityToolkit.WinUI.UI.Controls.DataGridCellEditEndedEventArgs e)
        {
            if (e.EditAction != CommunityToolkit.WinUI.UI.Controls.DataGridEditAction.Commit)
                return;

            var editedEmployee = (User)e.Row.DataContext;
            if (editedEmployee == null) return;
            try
            {
                if (ViewModel.UpdateEmployeeCommand.CanExecute(editedEmployee))
                    await ViewModel.UpdateEmployeeCommand.ExecuteAsync(editedEmployee);
            }
            catch (Exception ex)
            {
                await new ContentDialog
                {
                    Title = "Error",
                    Content = $"Failed to update employee: {ex.Message}",
                    CloseButtonText = "OK"
                }.ShowAsync();
            }
        }

        private void DeleteAllInputsButton_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            DeleteAllText.Foreground = (SolidColorBrush)App.Current.Resources["PrimaryBrush"];
        }

        private void DeleteAllInputsButton_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            DeleteAllText.Foreground = new SolidColorBrush(Colors.Black);
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
    }
}
