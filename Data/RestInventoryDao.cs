using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

    public class RestInventoryDao : IInventoryDao
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options = new()
        {
            Converters = { new JsonStringEnumConverter() },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public RestInventoryDao(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("Common");
        }

        public async Task<IEnumerable<Inventory>> GetAll()
        {
            try
            {
                var response = await _httpClient.GetAsync("/materials");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<IEnumerable<Inventory>>(json, _options);
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"HTTP request failed: {ex.Message}");
                return Enumerable.Empty<Inventory>();
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"JSON deserialization failed: {ex.Message}");
                return Enumerable.Empty<Inventory>();
            }
        }

        public async Task<Inventory>? GetById(string id)
        {
            throw new NotImplementedException();
        }

        public async Task Add(object inventory)
        {
            throw new NotImplementedException();
        }

        public async Task Update(string id, object inventory)
        {
            throw new NotImplementedException();
        }

        public async Task Delete(string id)
        {
            throw new NotImplementedException();
        }
    }
}
