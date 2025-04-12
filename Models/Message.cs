using System.Text.Json.Serialization;

namespace kafi.Models;

public class Message
{
    [JsonIgnore]
    public bool IsUser { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("sessionId")]
    public string? SessionId { get; set; }
    [JsonPropertyName("message")]
    public string? Text { get; set; }
}