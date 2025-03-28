using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace kafi.Models
{
    public class Product
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
        
        [JsonPropertyName("image")]
        public string Image { get; set; }
        
        [JsonPropertyName("onStock")]
        public bool IsAvailable { get; set; }
        
        [JsonPropertyName("categoryId")]
        public string CategoryId { get; set; }
        
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
        
        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}
