using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using kafi.Contracts.Data;
using kafi.Models;

namespace kafi.Data
{
    public interface IOrderDao : IDao<Order>
    {
    }

    public class RestOrderDao(IHttpClientFactory httpClientFactory) : IOrderDao
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("Common");

        public async Task<IEnumerable<Order>> GetAll()
        {
            var response = await _httpClient.GetAsync("/orders");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<Order>>(json);
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

        public async Task<Order>? GetById(string id)
        {
            throw new NotImplementedException();
        }


    }
}
