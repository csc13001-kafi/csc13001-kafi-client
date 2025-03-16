using CommunityToolkit.Mvvm.Messaging.Messages;

namespace kafi.Messages
{
    class CloseLoginWindowMessage : ValueChangedMessage<bool>
    {
        public CloseLoginWindowMessage() : base(true)
        {
        }
    }
}
