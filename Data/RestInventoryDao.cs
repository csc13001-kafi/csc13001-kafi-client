using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using kafi.Contracts.Data;
using kafi.Models;

namespace kafi.Data
{
    public interface IInventoryDao : IDao<Inventory>
    {
    }

    public class RestInventoryDao(IHttpClientFactory httpClientFactory) : IInventoryDao
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("Common");
        private readonly JsonSerializerOptions _options = new()
        {
            Converters = { new JsonStringEnumConverter() },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public async Task<IEnumerable<Inventory>> GetAll()
        {
            try
            {
                var response = await _httpClient.GetAsync("/materials");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var inventories = JsonSerializer.Deserialize<IEnumerable<Inventory>>(json, _options);
                return inventories ?? [];
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"HTTP request failed: {ex.Message}");
                return [];
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"JSON deserialization failed: {ex.Message}");
                return [];
            }
        }

        public async Task<Inventory>? GetById(Guid id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/materials/{id}");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var inventory = JsonSerializer.Deserialize<Inventory>(json, _options);
                return inventory;
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"HTTP request failed: {ex.Message}");
                return null;
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"JSON deserialization failed: {ex.Message}");
                return null;
            }
        }

        public async Task<object> Add(object inventory)
        {
            try
            {
                var json = JsonSerializer.Serialize(inventory, _options);
                var response = await _httpClient.PostAsync("/materials", new StringContent(json));
                response.EnsureSuccessStatusCode();
                return inventory;
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"HTTP request failed: {ex.Message}");
                return null;
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"JSON serialization failed: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }

        public async Task Update(Guid id, object inventory)
        {
            try
            {
                var json = JsonSerializer.Serialize(inventory, _options);
                var response = await _httpClient.PutAsync($"/materials/{id}", new StringContent(json));
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"HTTP request failed: {ex.Message}");
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"JSON serialization failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        public async Task Delete(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/materials/{id}");
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"HTTP request failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
