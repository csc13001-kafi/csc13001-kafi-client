using System.Text.Json.Serialization;

namespace kafi.Models
{
    public class OrderProduct
    {
        [JsonPropertyName("productName")]
        public string Name { get; set; }

        [JsonPropertyName("price")]
        public int Price { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
    }
}
