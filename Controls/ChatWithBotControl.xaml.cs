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
        }

        private void CloseChatPopupButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            ChatPopup.IsOpen = false;
        }

        private async void MarkdownTextBlock_LinkClicked(object sender, CommunityToolkit.WinUI.UI.Controls.LinkClickedEventArgs e)
        {
            var uri = new Uri(e.Link);
            await Launcher.LaunchUriAsync(uri);
        }
    }
}
