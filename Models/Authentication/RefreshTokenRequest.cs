using System.Text.Json.Serialization;

namespace kafi.Models;

public class RefreshTokenRequest
{
    [JsonPropertyName("refreshToken")]
    public required string RefreshToken { get; set; }
}
