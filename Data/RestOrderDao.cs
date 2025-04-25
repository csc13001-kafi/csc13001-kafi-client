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
            // Store form field data for later use
            var formFields = new Dictionary<string, string>();

            // Process the entity to extract form data
            if (entity is MultipartFormDataContent form)
            {
                // Extract form data without disposing original content
                foreach (var httpContent in form)
                {
                    if (httpContent.Headers.ContentDisposition != null)
                    {
                        var name = httpContent.Headers.ContentDisposition.Name?.Trim('"');

                        if (httpContent is StringContent stringContent)
                        {
                            var value = await stringContent.ReadAsStringAsync();

                            if (!string.IsNullOrEmpty(name))
                                formFields[name] = value;
                        }
                    }
                }
            }
            else if (entity is CreateOrderRequest createOrderRequest)
            {
                // Convert CreateOrderRequest to form field dictionary
                formFields["id"] = createOrderRequest.Id.ToString();
                formFields["table"] = createOrderRequest.Table;
                formFields["employeeName"] = createOrderRequest.EmployeeName;
                formFields["time"] = createOrderRequest.CreatedAt.ToString("o");

                if (!string.IsNullOrEmpty(createOrderRequest.ClientPhoneNumber))
                    formFields["clientPhoneNumber"] = createOrderRequest.ClientPhoneNumber;

                formFields["paymentMethod"] = createOrderRequest.PaymentMethod;

                // Add products and quantities as JSON arrays
                if (createOrderRequest.Products != null && createOrderRequest.Products.Count > 0)
                {
                    formFields["products"] = JsonSerializer.Serialize(createOrderRequest.Products);
                }

                if (createOrderRequest.Quantities != null && createOrderRequest.Quantities.Count > 0)
                {
                    formFields["quantities"] = JsonSerializer.Serialize(createOrderRequest.Quantities);
                }
            }
            else
            {
                throw new ArgumentException($"Unsupported entity type: {entity.GetType().Name}");
            }

            // Create a fresh MultipartFormDataContent object right before sending the request
            using var freshContent = new MultipartFormDataContent();

            // Add all form fields from the dictionary
            foreach (var field in formFields)
            {
                freshContent.Add(new StringContent(field.Value), field.Key);
            }

            // Send the request with the fresh content
            var response = await _httpClient.PostAsync("/orders/order", freshContent);
            var resultJson = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            return JsonSerializer.Deserialize<CreateOrderResponse>(resultJson) ??
                throw new JsonException("Failed to deserialize order");
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
