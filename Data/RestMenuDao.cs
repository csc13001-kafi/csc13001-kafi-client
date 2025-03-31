using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using kafi.Models;

namespace kafi.Data
{
    public interface IMenuDao
    {
        Task<CategoryProductsResponse> GetCategoriesAndProducts();
    }

    public class RestMenuDao(IHttpClientFactory httpClientFactory) : IMenuDao
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("Common");
        private readonly JsonSerializerOptions _options = new()
        {
            Converters = { new JsonStringEnumConverter() },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public async Task<CategoryProductsResponse> GetCategoriesAndProducts()
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
    }
}