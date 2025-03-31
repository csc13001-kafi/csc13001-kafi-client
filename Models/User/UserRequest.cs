using System;
using System.IO;
using System.Text.Json.Serialization;

namespace kafi.Models
{
    public record ImageRequest
    (
        Stream FileStream,
        string ContentType,
        string FileName
    );

    public class UserRequest
    {
        [JsonPropertyName("username")]
        public string? Name { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        [JsonPropertyName("address")]
        public string? Address { get; set; }

        [JsonPropertyName("salary")]
        public int Salary { get; set; }

        [JsonPropertyName("birthdate")]
        public DateTime Birthdate { get; set; }

        [JsonPropertyName("workStart")]
        public TimeSpan StartShift { get; set; }

        [JsonPropertyName("workEnd")]
        public TimeSpan EndShift { get; set; }
    }
}
