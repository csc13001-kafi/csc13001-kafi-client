using System;
using kafi.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace kafi.Controls
{
    public sealed partial class HeaderControl : UserControl
    {
        public static readonly DependencyProperty UsernameProperty =
            DependencyProperty.Register("Username", typeof(string), typeof(HeaderControl), new PropertyMetadata("Unknown User"));

        public static readonly DependencyProperty IsManagerProperty =
            DependencyProperty.Register("IsManager", typeof(bool), typeof(HeaderControl), new PropertyMetadata(false));

        public static readonly DependencyProperty ProfileImageProperty =
            DependencyProperty.Register("ProfileImage", typeof(BitmapImage), typeof(HeaderControl), new PropertyMetadata(null));

        public string Username
        {
            get { return (string)GetValue(UsernameProperty); }
            set { SetValue(UsernameProperty, value); }
        }

        public bool IsManager
        {
            get { return (bool)GetValue(IsManagerProperty); }
            set { SetValue(IsManagerProperty, value); }
        }

        public BitmapImage ProfileImage
        {
            get { return (BitmapImage)GetValue(ProfileImageProperty); }
            set { SetValue(ProfileImageProperty, value); }
        }

        public Action<Type>? NavigateToPage { get; set; }

        public HeaderControl()
        {
            this.InitializeComponent();
        }

        private void NavigateToMenuPage_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage?.Invoke(typeof(TablePage));
        }
    }
}
