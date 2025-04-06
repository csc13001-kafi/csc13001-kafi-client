using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using kafi.Contracts;
using kafi.Models;

namespace kafi.Data;

public interface ICategoryDao : IDao<Category>
{
}

public class RestCategoryDao(IHttpClientFactory httpClientFactory) : ICategoryDao
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("Common");

    public async Task<object> Add(object entity)
    {
        if (entity is not MultipartFormDataContent form)
        {
            throw new ArgumentException("Invalid entity type");
        }
        var response = await _httpClient.PostAsync("categories", form);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Category>(json) ?? throw new Exception("Failed to deserialize category");
    }

    public async Task Delete(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"categories/{id}");
        response.EnsureSuccessStatusCode();
    }

    public Task<IEnumerable<Category>> GetAll()
    {
        throw new NotImplementedException();
    }

    public async Task<Category>? GetById(Guid id)
    {
        var response = await _httpClient.GetAsync($"categories/{id}");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        Category? category = JsonSerializer.Deserialize<Category>(json) ?? throw new Exception("Failed to deserialize category");
        category.Id = id;
        return category;
    }

    public async Task Update(Guid id, object entity)
    {
        if (entity is not MultipartFormDataContent form)
        {
            throw new ArgumentException("Invalid entity type");
        }
        var response = await _httpClient.PatchAsync($"categories/{id}", form);
        response.EnsureSuccessStatusCode();
    }
}
