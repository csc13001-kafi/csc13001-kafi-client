using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;

namespace kafi.Models
{
    public record CreateProductRequest
    (
        string Name,
        int Price,
        bool IsAvailable,
        Guid CategoryId,
        List<ProductMaterial> Materials,
        Stream FileStream,
        string ContentType,
        string FileName
    );
    public class ProductMaterial
    {
        [JsonPropertyName("materialId")]
        public Guid Id { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
    }
    public class Product
    {
        [JsonPropertyName("id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Guid Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("image")]
        public string Image { get; set; }

        [JsonPropertyName("price")]
        public int Price { get; set; }

        [JsonPropertyName("onStock")]
        public bool IsAvailable { get; set; }

        [JsonPropertyName("categoryId")]
        public Guid CategoryId { get; set; }

        [JsonPropertyName("createdAt")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("newMaterials")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ProductMaterial[] Materials { get; set; }
    }
}
