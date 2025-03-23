using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace kafi.Models
{
    public class Order
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("time")]
        public DateTime Time { get; set; }

        [JsonPropertyName("employeeName")]
        public string EmployeeName { get; set; }

        [JsonPropertyName("paymentMethod")]
        public string PaymentMethod { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }
    }
}