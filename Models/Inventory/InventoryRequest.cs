using System;
using System.Text.Json.Serialization;

namespace kafi.Models
{
    public class InventoryRequest
    {

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("originalStock")]
        public int OriginalStock { get; set; }

        [JsonPropertyName("unit")]
        public string Unit { get; set; }

        [JsonPropertyName("expiredDate")]
        public DateTime ExpiredDate { get; set; }

        [JsonPropertyName("price")]
        public int Price { get; set; }
    }
}