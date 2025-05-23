using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using kafi.Contracts;
using kafi.Models;

namespace kafi.Data;

public interface IInventoryDao : IDao<Inventory>
{
    Task<string> UpdateCurrentStock(Guid id, int currentStock);
}

public class RestInventoryDao(IHttpClientFactory httpClientFactory) : IInventoryDao
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("Common");
    private readonly JsonSerializerOptions _options = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
    };

    public async Task<IEnumerable<Inventory>> GetAll()
    {
        var response = await _httpClient.GetAsync("/materials");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var inventories = JsonSerializer.Deserialize<IEnumerable<Inventory>>(json, _options);
        return inventories ?? throw new Exception("Failed to deserialize inventories");
    }

    public async Task<Inventory>? GetById(Guid id)
    {
        var response = await _httpClient.GetAsync($"/materials/{id}");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var inventory = JsonSerializer.Deserialize<Inventory>(json, _options);
        return inventory ?? throw new Exception("Failed to deserialize inventory");
    }

    public async Task<object> Add(object inventory)
    {
        var json = JsonSerializer.Serialize(inventory, _options);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/materials", content);
        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadAsStringAsync();
        var newInventory = JsonSerializer.Deserialize<Inventory>(payload, _options);
        return newInventory ?? throw new Exception("Failed to deserialize inventory");
    }

    public async Task Update(Guid id, object inventory)
    {
        var json = JsonSerializer.Serialize(inventory, _options);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PatchAsync($"/materials/{id}", content);
        response.EnsureSuccessStatusCode();
    }

    public async Task Delete(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"/materials/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<string> UpdateCurrentStock(Guid id, int currentStock)
    {
        var json = JsonSerializer.Serialize(new { currentStock }, _options);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"/materials/{id}/stock", content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}
