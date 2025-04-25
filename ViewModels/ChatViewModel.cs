using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kafi.Contracts.Services;
using kafi.Models;

namespace kafi.ViewModels;

public partial class ChatViewModel(IAiService aiService) : ObservableObject
{
    private readonly IAiService _aiService = aiService;
    private string _sessionId = string.Empty;
    private bool _isFirstTimeChat = true;
    private readonly string _startMessage = "Chào bạn, bạn cần cập nhật điều gì về Kafi hôm nay?";

    public ObservableCollection<Message> Messages { get; } = [];

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendMessageCommand))]
    public partial string MessageText { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendMessageCommand))]
    public partial bool IsSessionLoading { get; set; } = false;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendMessageCommand))]
    public partial bool IsMessageLoading { get; set; } = false;

    private bool CanSendMessage() => !string.IsNullOrWhiteSpace(MessageText) && !IsSessionLoading && !IsMessageLoading && !string.IsNullOrEmpty(_sessionId);
    [RelayCommand(CanExecute = nameof(CanSendMessage))]
    private async Task SendMessageAsync()
    {
        IsMessageLoading = true;
        try
        {
            var userMessage = new Message { IsUser = true, Text = MessageText, IsNew = true };
            Messages.Add(userMessage);
            MessageText = string.Empty;

            Message response = await _aiService.SendMessageAsync(_sessionId, userMessage.Text);
            if (response != null)
            {
                Messages.Remove(userMessage);
                Messages.Add(new Message { IsUser = true, Text = userMessage.Text, SessionId = _sessionId });
                Messages.Add(new Message { IsUser = false, Text = response.Text, SessionId = response.SessionId });
            }
            else
            {
                Messages.Add(new Message { IsUser = false, Text = "Error: Unable to get response." });
            }
        }
        catch (Exception ex)
        {
            Messages.Add(new Message { IsUser = false, Text = $"Error: {ex.Message}" });
        }
        finally
        {
            IsMessageLoading = false;
        }
    }

    private bool CanStartNewChat() => !IsSessionLoading;
    [RelayCommand(CanExecute = nameof(CanStartNewChat))]
    public async Task StartNewChatAsync()
    {
        Messages.Clear();
        IsSessionLoading = true;
        try
        {
            _sessionId = await _aiService.StartNewChatAsync();
            Messages.Add(new Message { IsUser = false, Text = _startMessage, SessionId = _sessionId });
        }
        catch (Exception ex)
        {
            Messages.Add(new Message { IsUser = false, Text = ex.Message, SessionId = _sessionId });
        }
        finally
        {
            IsSessionLoading = false;
        }
    }

    [RelayCommand]
    public async Task StartChatAsync()
    {
        if (!_isFirstTimeChat)
            return;

        await StartNewChatAsync();
        _isFirstTimeChat = false;
    }
}
