using System.Threading.Tasks;
using kafi.Models;

namespace kafi.Contracts.Services;

public interface IAiService
{
    Task<Message> SendMessageAsync(string sessionId, string message);
    Task<string> StartNewChatAsync();
}
