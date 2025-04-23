using System;
using CommunityToolkit.Mvvm.Messaging;
using kafi.Controls;
using kafi.Helpers;
using kafi.Models;
using kafi.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
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
    public sealed partial class MainPage : Page, IRecipient<AnalyticReportReadyMessage>
    {
        public MainViewModel ViewModel { get; }
        public MainPage()
        {
            ViewModel = App.Services.GetRequiredService<MainViewModel>();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            this.InitializeComponent();

            // Register for messages
            WeakReferenceMessenger.Default.Register<AnalyticReportReadyMessage>(this);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ViewModel.LoadDataCommand.ExecuteAsync(null);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Unregister from messages when navigating away
            WeakReferenceMessenger.Default.Unregister<AnalyticReportReadyMessage>(this);
            base.OnNavigatedFrom(e);
        }

        private async void DashboardFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel.ChangeTimeRangeCommand.CanExecute(null))
                await ViewModel.ChangeTimeRangeCommand.ExecuteAsync(null);
        }

        private async void RevenueDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            if (ViewModel.ChangeRevenueDateCommand.CanExecute(null))
            {
                await ViewModel.ChangeRevenueDateCommand.ExecuteAsync(null);
            }
        }

        private void GenerateAnalyticButton_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            // Apply black color to whichever text is currently visible
            if (ViewModel.IsGeneratingReport)
            {
                LoadingReportText.Foreground = new SolidColorBrush(Colors.Black);
            }
            else
            {
                NormalReportText.Foreground = new SolidColorBrush(Colors.Black);
            }

            GenerateAnalyticButton.Background = new SolidColorBrush(Color.FromArgb(255, 42, 78, 49));
        }

        private void GenerateAnalyticButton_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            // Restore original colors for both TextBlocks
            NormalReportText.Foreground = new SolidColorBrush(Colors.White);
            LoadingReportText.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 224, 130)); // #FFE082

            GenerateAnalyticButton.Background = App.Current.Resources["SecondaryBrush"] as SolidColorBrush;
        }

        /// <summary>
        /// Handles the AnalyticReportReadyMessage when it is received
        /// </summary>
        public async void Receive(AnalyticReportReadyMessage message)
        {
            // Ensure we run on the UI thread
            await this.DispatcherQueue.EnqueueAsync(async () =>
            {
                // Create and show the dialog with markdown content
                var dialog = new MarkdownDialog(this.XamlRoot);

                // Format the markdown text to ensure proper line breaks
                dialog.MarkdownText = message.Value;

                await dialog.ShowAsync();
            });
        }
    }
}
