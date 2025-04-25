using System;
using System.Threading.Tasks;
using kafi.Data;
using kafi.Models;

namespace kafi.Repositories;

public interface IAnalyticsRepository
{
    Task<Dashboard> GetDashboard(string timeRange);
    Task<TopSellingProductsResponse> GetTopSellingProducts(int limit = 10, string? startDate = null, string? endDate = null);
    Task<Employees> GetEmployeesCount();
    Task<OrdersByDayResponse> GetOrdersByDayAndPaymentMethod(int month);
    Task<HourlySalesResponse> GetHourlySalesData(string date);
    Task<LowStockMaterialsResponse> GetLowStockMaterials(int limit = 3);
    Task<string> GetBusinessReport(string timeRange);
}

public class AnalyticsRepository(IAnalyticsDao dao) : IAnalyticsRepository
{
    private readonly IAnalyticsDao _dao = dao;

    public async Task<Dashboard> GetDashboard(string timeRange)
    {
        // Validate timeRange if needed
        if (string.IsNullOrEmpty(timeRange))
        {
            throw new ArgumentException("Time range cannot be empty", nameof(timeRange));
        }

        return await _dao.GetDashboard(timeRange);
    }

    public async Task<TopSellingProductsResponse> GetTopSellingProducts(int limit = 10, string? startDate = null, string? endDate = null)
    {
        // Validate parameters
        if (limit <= 0)
        {
            throw new ArgumentException("Limit must be positive", nameof(limit));
        }

        if ((startDate != null && endDate == null) || (startDate == null && endDate != null))
        {
            throw new ArgumentException("Both startDate and endDate must be provided or none");
        }

        if (startDate != null && endDate != null)
        {
            // Validate date formats
            if (!DateTime.TryParse(startDate, out _) || !DateTime.TryParse(endDate, out _))
            {
                throw new ArgumentException("Invalid date format");
            }
        }

        return await _dao.GetTopSellingProducts(limit, startDate, endDate);
    }

    public async Task<Employees> GetEmployeesCount()
    {
        return await _dao.GetEmployeesCount();
    }

    public async Task<OrdersByDayResponse> GetOrdersByDayAndPaymentMethod(int month)
    {
        // Validate month
        if (month < 1 || month > 12)
        {
            throw new ArgumentException("Month must be between 1 and 12", nameof(month));
        }

        return await _dao.GetOrdersByDayAndPaymentMethod(month);
    }

    public async Task<HourlySalesResponse> GetHourlySalesData(string date)
    {
        // Validate date format
        if (string.IsNullOrEmpty(date) || !DateTime.TryParse(date, out _))
        {
            throw new ArgumentException("Invalid date format", nameof(date));
        }

        return await _dao.GetHourlySalesData(date);
    }

    public async Task<LowStockMaterialsResponse> GetLowStockMaterials(int limit = 3)
    {
        // Validate limit
        if (limit <= 0)
        {
            limit = 3; // Default to 3 if invalid
        }

        return await _dao.GetLowStockMaterials(limit);
    }

    public async Task<string> GetBusinessReport(string timeRange)
    {
        // Validate timeRange if needed
        if (string.IsNullOrEmpty(timeRange))
        {
            throw new ArgumentException("Time range cannot be empty", nameof(timeRange));
        }
        return await _dao.GetBusinessReport(timeRange);
    }
}
