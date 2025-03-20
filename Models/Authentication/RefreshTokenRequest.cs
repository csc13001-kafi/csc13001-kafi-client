using System.Text.Json.Serialization;

namespace kafi.Models.Authentication
{
    public class RefreshTokenRequest
    {
        [JsonPropertyName("refreshToken")]
        public required string RefreshToken { get; set; }
    }
}
