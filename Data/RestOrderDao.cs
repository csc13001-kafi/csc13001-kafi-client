using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using kafi.Contracts;
using kafi.Models;

namespace kafi.Data
{
    public interface IOrderDao : IDao<Order>
    {
        Task<PaymentStatus> GetPaymentStatus(int orderCode);
    }

    public class RestOrderDao(IHttpClientFactory httpClientFactory) : IOrderDao
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("Common");

        public async Task<IEnumerable<Order>> GetAll()
        {
            var response = await _httpClient.GetAsync("/orders");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<Order>>(json) ?? throw new JsonException("Failed to deserialize orders");
        }

        public async Task<object> Add(object entity)
        {
            if (entity is not MultipartFormDataContent form)
            {
                throw new ArgumentException("Invalid entity type");
            }
            var response = await _httpClient.PostAsync("/orders/order", form);
            response.EnsureSuccessStatusCode();
            var resultJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CreateOrderResponse>(resultJson) ?? throw new JsonException("Failed to deserialize order");
        }

        public Task Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task Update(Guid id, object entity)
        {
            throw new NotImplementedException();
        }

        public async Task<Order>? GetById(Guid id)
        {
            var response = await _httpClient.GetAsync($"/orders/{id}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Order>(json) ?? throw new JsonException("Failed to deserialize order");
        }

        public async Task<PaymentStatus> GetPaymentStatus(int orderCode)
        {
            var response = await _httpClient.GetAsync($"/orders/payment-status/{orderCode}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PaymentStatus>(json) ?? throw new JsonException("Failed to deserialize payment status");
            return result;
        }
    }
}
