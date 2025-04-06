using System.Text.Json.Serialization;

namespace kafi.Models;

public class RefreshTokenResponse
{
    [JsonPropertyName("accessToken")]
    public required string AccessToken { get; set; }
}
