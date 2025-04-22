using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using kafi.Models;

namespace kafi.Data;

public interface IAnalyticsDao
{
    Task<Dashboard> GetDashboard(string timeRange);
    Task<TopSellingProductsResponse> GetTopSellingProducts(int limit, string? startDate, string? endDate);
    Task<Employees> GetEmployeesCount();
    Task<OrdersByDayResponse> GetOrdersByDayAndPaymentMethod(int month);
    Task<HourlySalesResponse> GetHourlySalesData(string date);
    Task<LowStockMaterialsResponse> GetLowStockMaterials(int limit);
    Task<string> GetBusinessReport(string timeRange);
}

public class RestAnalyticsDao(IHttpClientFactory httpClientFactory) : IAnalyticsDao
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("Common");

    private async Task<T> GetAsync<T>(string endpoint, bool isText = false)
    {
        var response = await _httpClient.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        if (isText)
        {
            return (T)(object)json;
        }
        return JsonSerializer.Deserialize<T>(json)
            ?? throw new Exception($"Failed to deserialize response from {endpoint}");
    }

    public async Task<Dashboard> GetDashboard(string timeRange)
    {
        return await GetAsync<Dashboard>($"analytics/dashboard?timeRange={timeRange}");
    }

    public async Task<TopSellingProductsResponse> GetTopSellingProducts(int limit, string? startDate, string? endDate)
    {
        var url = $"analytics/products/top-selling?limit={limit}";

        if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
        {
            url += $"&startDate={startDate}&endDate={endDate}";
        }

        return await GetAsync<TopSellingProductsResponse>(url);
    }

    public async Task<Employees> GetEmployeesCount()
    {
        return await GetAsync<Employees>("analytics/users/count?role=Employee");
    }

    public async Task<OrdersByDayResponse> GetOrdersByDayAndPaymentMethod(int month)
    {
        return await GetAsync<OrdersByDayResponse>($"analytics/orders/months?month={month}");
    }

    public async Task<HourlySalesResponse> GetHourlySalesData(string date)
    {
        return await GetAsync<HourlySalesResponse>($"analytics/orders/hours?date={date}");
    }

    public async Task<LowStockMaterialsResponse> GetLowStockMaterials(int limit)
    {
        return await GetAsync<LowStockMaterialsResponse>($"analytics/materials/low-stock?limit={limit}");
    }

    public async Task<string> GetBusinessReport(string timeRange)
    {
        return await GetAsync<string>($"analytics/business-report?timeRange={timeRange}", true);
    }
}
