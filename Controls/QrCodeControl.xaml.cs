using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace kafi.Controls
{
    public sealed partial class QrCodeControl : UserControl
    {
        public event EventHandler? ImageLoaded;
        public QrCodeControl(string qrCodeUrl)
        {
            this.InitializeComponent();
            QrImage.ImageOpened += OnImageOpened;
            QrImage.ImageFailed += OnImageFailed;
            QrImage.Source = new BitmapImage(new Uri(qrCodeUrl));
        }

        private void OnImageOpened(object sender, RoutedEventArgs e)
        {
            LoadingIndicator.IsActive = false;
            LoadingIndicator.Visibility = Visibility.Collapsed;
            if (ImageLoaded != null)
            {
                var timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(5)
                };

                timer.Tick += (s, args) =>
                {
                    timer.Stop();
                    ImageLoaded.Invoke(this, EventArgs.Empty);
                };

                timer.Start();
            }
        }

        private void OnImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            LoadingIndicator.IsActive = false;
            LoadingIndicator.Visibility = Visibility.Collapsed;
        }
    }
}
