using System;
using kafi.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace kafi.Controls
{
    public sealed partial class ChatWithBotControl : UserControl
    {
        public ChatViewModel ViewModel { get; }
        public ChatWithBotControl()
        {
            ViewModel = App.Services.GetRequiredService<ChatViewModel>();
            this.InitializeComponent();
        }

        private void ChatButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            ChatPopup.IsOpen = true;
            ChatButton.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        }

        private void CloseChatPopupButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            ChatPopup.IsOpen = false;
            ChatButton.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
        }

        private async void MarkdownTextBlock_LinkClicked(object sender, CommunityToolkit.WinUI.UI.Controls.LinkClickedEventArgs e)
        {
            var uri = new Uri(e.Link);
            await Launcher.LaunchUriAsync(uri);
        }

        private bool _isShiftPressed = false;
        private void TextBox_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Shift)
            {
                _isShiftPressed = true;
            }
        }

        private void TextBox_KeyUp(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Shift)
            {
                _isShiftPressed = false;
            }
        }

        private async void TextBox_PreviewKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                if (!_isShiftPressed)
                {
                    e.Handled = true;
                    await ViewModel.SendMessageCommand.ExecuteAsync(null);
                }
            }
        }
    }
}
