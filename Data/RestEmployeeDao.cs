using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using kafi.Contracts;
using kafi.Models;

namespace kafi.Data;

public interface IEmployeeDao : IDao<User>
{
}
public class RestEmployeeDao(IHttpClientFactory httpClientFactory) : IEmployeeDao
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("Common");
    private readonly JsonSerializerOptions _options = new()
    {
        Converters = { new JsonStringEnumConverter() },
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public async Task<object> Add(object employee)
    {
        var json = JsonSerializer.Serialize(employee, _options);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("users/employee", content);
        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadAsStringAsync();
        var user = JsonSerializer.Deserialize<UserResponse>(payload, _options);
        return user ?? throw new Exception("Failed to deserialize user");
    }

    public async Task Delete(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"users/employee/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<IEnumerable<User>> GetAll()
    {
        var response = await _httpClient.GetAsync("users?role=Employee");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var users = JsonSerializer.Deserialize<IEnumerable<User>>(json, _options);
        return users ?? [];
    }

    public async Task<User>? GetById(Guid id)
    {
        var response = await _httpClient.GetAsync($"users/employee/{id}");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var user = JsonSerializer.Deserialize<User>(json, _options);
        return user ?? throw new Exception("Failed to deserialize user");
    }

    public async Task Update(Guid id, object entity)
    {
        var json = JsonSerializer.Serialize(entity, _options);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PatchAsync($"users/employee/{id}", content);
        response.EnsureSuccessStatusCode();
    }
}
