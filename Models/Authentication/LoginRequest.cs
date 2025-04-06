using System.Text.Json.Serialization;

namespace kafi.Models;

public class LoginRequest
{
    [JsonPropertyName("username")]
    public required string UserName { get; set; }
    [JsonPropertyName("password")]
    public required string Password { get; set; }
}