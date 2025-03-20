using System.Text.Json.Serialization;

namespace kafi.Models.Authentication
{
    public class LoginResponse
    {
        [JsonPropertyName("accessToken")]
        public required string AccessToken { get; set; }
        [JsonPropertyName("refreshToken")]
        public required string RefreshToken { get; set; }
        [JsonPropertyName("message")]
        public required string Message { get; set; }
    }
}
