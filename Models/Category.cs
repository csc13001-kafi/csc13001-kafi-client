using System;
using System.IO;
using System.Text.Json.Serialization;

namespace kafi.Models
{
    public record CreateCategoryRequest
    (
        string Name,
        Stream FileStream,
        string ContentType,
        string FileName
    );

    public class Category
    {
        [JsonPropertyName("id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Guid Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("image")]
        public string Image { get; set; }

        [JsonPropertyName("createdAt")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public DateTime UpdatedAt { get; set; }
    }
}