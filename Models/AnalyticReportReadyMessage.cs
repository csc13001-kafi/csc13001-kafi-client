using CommunityToolkit.Mvvm.Messaging.Messages;

namespace kafi.Models;

/// <summary>
/// Message sent when an analytic report is ready to be displayed
/// </summary>
public class AnalyticReportReadyMessage : ValueChangedMessage<string>
{
    public AnalyticReportReadyMessage(string value) : base(value)
    {
    }
}