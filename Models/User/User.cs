using System;
using System.Text.Json.Serialization;

namespace kafi.Models;

public enum Role
{
    Manager,
    Employee
}

public class User
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("username")]
    public string? Name { get; set; }

    [JsonPropertyName("role")]
    public Role Role { get; set; }

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

    [JsonPropertyName("image")]
    public string? Image { get; set; }

    [JsonIgnore]
    public bool IsActive { get; set; }

    [JsonIgnore]
    public DateTime CreatedAt { get; set; }
}
