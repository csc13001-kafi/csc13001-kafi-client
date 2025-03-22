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

        public async Task Add(object employee)
        {
            try
            {
                var json = JsonSerializer.Serialize(employee, _options);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("users/employee", content);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"HTTP request failed: {ex.Message}");
            }
        }

        public async Task Delete(string id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"users/employee/{id}");
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"HTTP request failed: {ex.Message}");
            }
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            try
            {
                var response = await _httpClient.GetAsync("users?role=Employee");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();

                var users = JsonSerializer.Deserialize<IEnumerable<User>>(json, _options);
                return users;
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"HTTP request failed: {ex.Message}");
                return Enumerable.Empty<User>();
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"JSON deserialization failed: {ex.Message}");
                return Enumerable.Empty<User>();
            }
        }

        public async Task<User>? GetById(string id)
        {
            throw new NotImplementedException();
        }

        public async Task Update(string id, object entity)
        {
            try
            {
                var json = JsonSerializer.Serialize(entity, _options);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PatchAsync($"users/employee/{id}", content);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"HTTP request failed: {ex.Message}");
            }
        }
    }
}
