using System;
using System.Text.Json.Serialization;

namespace kafi.Models
{
    public class UserResponse
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("username")]
        public string Username { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("role")]
        public Role Role { get; set; }
    }
}
