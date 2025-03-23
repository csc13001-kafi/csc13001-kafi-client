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
    public interface IMenuDao : IDao<Product>
    {
        Task<CategoryProductsResponse> GetCategoriesAndProducts();
    }

    public class RestMenuDao : IMenuDao
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options = new()
        {
            Converters = { new JsonStringEnumConverter() },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public RestMenuDao(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("Common");
        }

        public async Task<CategoryProductsResponse> GetCategoriesAndProducts()
        {
            try
            {
                var response = await _httpClient.GetAsync("categories/products");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
            
                var result = JsonSerializer.Deserialize<CategoryProductsResponse>(json, _options);
                if ((result?.Categories?.Count ?? 0) == 0 || (result?.Products?.Count ?? 0) == 0)
                {
                    var caseInsensitiveOptions = new JsonSerializerOptions(_options)
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    result = JsonSerializer.Deserialize<CategoryProductsResponse>(json, caseInsensitiveOptions);
                }
                
                return result ?? new CategoryProductsResponse();
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"HTTP request failed: {ex.Message}");
                return new CategoryProductsResponse();
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"JSON deserialization failed: {ex.Message}");
                return new CategoryProductsResponse();
            }
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            try
            {
                var response = await _httpClient.GetAsync("categories/products");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<CategoryProductsResponse>(json, _options);
                return result?.Products ?? new List<Product>();
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"HTTP request failed: {ex.Message}");
                return Enumerable.Empty<Product>();
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"JSON deserialization failed: {ex.Message}");
                return Enumerable.Empty<Product>();
            }
        }

        public async Task<Product>? GetById(string id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"products/{id}");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();

                var product = JsonSerializer.Deserialize<Product>(json, _options);
                return product;
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

        public Task Add(object entity)
        {
            throw new NotImplementedException();
        }

        public Task Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Task Update(string id, object entity)
        {
            throw new NotImplementedException();
        }
    }
} 