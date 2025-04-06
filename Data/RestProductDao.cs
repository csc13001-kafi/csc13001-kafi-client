using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using kafi.Contracts;
using kafi.Models;

namespace kafi.Data;

public interface IProductDao : IDao<Product>
{
}

public class RestProductDao(IHttpClientFactory httpClientFactory) : IProductDao
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("Common");

    public async Task<object> Add(object entity)
    {
        if (entity is not MultipartFormDataContent form)
        {
            throw new ArgumentException("Invalid entity type");
        }
        var response = await _httpClient.PostAsync("products", form);
        response.EnsureSuccessStatusCode();
        var jsonResult = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Product>(jsonResult)
               ?? throw new Exception("Failed to deserialize product");
    }

    public async Task Delete(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"products/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<IEnumerable<Product>> GetAll()
    {
        var response = await _httpClient.GetAsync("products");
        response.EnsureSuccessStatusCode();
        var jsonResult = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<IEnumerable<Product>>(jsonResult)
               ?? throw new Exception("Failed to deserialize products");
    }

    public Task<Product>? GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task Update(Guid id, object entity)
    {
        if (entity is not MultipartFormDataContent form)
        {
            throw new ArgumentException("Invalid entity type");
        }
        var response = await _httpClient.PatchAsync($"products/{id}", form);
        response.EnsureSuccessStatusCode();
    }
}
