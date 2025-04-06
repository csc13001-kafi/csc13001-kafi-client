using System;
using System.Text.Json.Serialization;

namespace kafi.Models;

public class Inventory
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("orginalStock")]
    public int OriginalStock { get; set; }

    [JsonPropertyName("currentStock")]
    public int CurrentStock { get; set; }

    [JsonPropertyName("unit")]
    public string? Unit { get; set; }

    [JsonPropertyName("expiredDate")]
    public DateTime ExpiredDate { get; set; }

    [JsonPropertyName("price")]
    public int Price { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}