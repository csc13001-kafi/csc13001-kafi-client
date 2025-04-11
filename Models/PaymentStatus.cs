using System.Text.Json.Serialization;

namespace kafi.Models;

public class PaymentStatus
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }
    [JsonPropertyName("status")]
    public string? Status { get; set; }
    [JsonPropertyName("message")]
    public string? Message { get; set; }
    [JsonPropertyName("orderCode")]
    public int OrderCode { get; set; }
    [JsonPropertyName("orderCreated")]
    public bool OrderCreated { get; set; }
}
