using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace kafi.Models;

public record CreateOrderRequest
(
    Guid Id,
    string Table,
    string EmployeeName,
    DateTime CreatedAt,
    string ClientPhoneNumber,
    string PaymentMethod,
    List<Guid> Products,
    List<int> Quantities
);

public class Order
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("employeeName")]
    public string? EmployeeName { get; set; }

    [JsonPropertyName("clientPhoneNumber")]
    public string? ClientPhoneNumber { get; set; }

    [JsonPropertyName("table")]
    public string? Table { get; set; }

    [JsonPropertyName("time")]
    public DateTime Time { get; set; }

    [JsonPropertyName("numberOfProducts")]
    public int DistinctProductCount { get; set; }

    [JsonPropertyName("totalPrice")]
    public int TotalPrice { get; set; }

    [JsonPropertyName("discountPercentage")]
    public int DiscountPercentage { get; set; }

    [JsonPropertyName("discount")]
    public int Discount { get; set; }

    [JsonPropertyName("afterDiscountPrice")]
    public int FinalPrice { get; set; }

    [JsonPropertyName("price")]
    public int Price { get; set; }

    [JsonPropertyName("paymentMethod")]
    public string? PaymentMethod { get; set; }

    [JsonPropertyName("orderDetails")]
    public List<OrderProduct>? Products { get; set; }
}
public class OrderProduct
{
    [JsonPropertyName("productName")]
    public string? Name { get; set; }

    [JsonPropertyName("price")]
    public int Price { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }
}
public class CreateOrderResponse
{
    [JsonPropertyName("discountPercentage")]
    public int DiscountPercentage { get; set; }

    [JsonPropertyName("discount")]
    public int Discount { get; set; }

    [JsonPropertyName("paymentLink")]
    public string? PaymentLink { get; set; }

    [JsonPropertyName("qrLink")]
    public string? QrLink { get; set; }

    [JsonPropertyName("orderCode")]
    public int OrderCode { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }
}