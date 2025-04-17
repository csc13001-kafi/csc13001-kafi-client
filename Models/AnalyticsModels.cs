using System.Collections.Generic;
using System.Text.Json.Serialization;
using LiveChartsCore.Defaults;

namespace kafi.Models;

public class Dashboard
{
    [JsonPropertyName("Overview")]
    public OverviewStats Overview { get; set; }

    [JsonPropertyName("Product")]
    public ProductStats Product { get; set; }

    [JsonPropertyName("Membership")]
    public int Membership { get; set; }

    [JsonPropertyName("timeRange")]
    public TimeRangeResponse TimeRange { get; set; }
}

public class OverviewStats
{
    [JsonPropertyName("ordersCount")]
    public int OrdersCount { get; set; }

    [JsonPropertyName("ordersPercentChange")]
    public float OrdersPercentChange { get; set; } = 0;

    [JsonPropertyName("ordersTotalPrice")]
    public int OrdersTotalPrice { get; set; }

    [JsonPropertyName("revenuePercentChange")]
    public float RevenuePercentChange { get; set; } = 0;
}

public class ProductStats
{
    [JsonPropertyName("categoriesCount")]
    public int CategoriesCount { get; set; }

    [JsonPropertyName("productsCount")]
    public int ProductsCount { get; set; }
}

public class TimeRangeResponse
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("startDate")]
    public string StartDate { get; set; }

    [JsonPropertyName("endDate")]
    public string EndDate { get; set; }
}

public class HourlySalesResponse
{
    [JsonPropertyName("date")]
    public string Date { get; set; }

    [JsonPropertyName("rawDate")]
    public string RawDate { get; set; }

    [JsonPropertyName("hourlySales")]
    public List<HourlySale> HourlySales { get; set; }
}

public class HourlySale
{
    [JsonPropertyName("hour")]
    public int Hour { get; set; }

    [JsonPropertyName("totalPrice")]
    public int Count { get; set; }
}

public class TopSellingProductsResponse
{
    [JsonPropertyName("topProducts")]
    public List<TopProductResponse> TopProducts { get; set; }
}

public class Employees
{
    [JsonPropertyName("count")]
    public int Count { get; set; }
}

public class TopProductResponse
{
    [JsonPropertyName("productName")]
    public string ProductName { get; set; }

    [JsonPropertyName("totalQuantity")]
    public string Quantity { get; set; }


}

public class TopProduct : ObservableValue
{
    public TopProduct(string productName, string value)
    {
        ProductName = productName;
        Value = int.Parse(value);
    }
    public string ProductName { get; set; }
}

public class OrdersByDayResponse
{
    [JsonPropertyName("month")]
    public int Month { get; set; }

    [JsonPropertyName("monthName")]
    public string MonthName { get; set; }

    [JsonPropertyName("year")]
    public int Year { get; set; }

    [JsonPropertyName("days")]
    public List<DayStats> Days { get; set; }

    [JsonPropertyName("summary")]
    public PaymentSummary Summary { get; set; }
}

public class DayStats
{
    [JsonPropertyName("day")]
    public int Day { get; set; }

    [JsonPropertyName("cash")]
    public int Cash { get; set; }

    [JsonPropertyName("qr")]
    public int Qr { get; set; }

    [JsonPropertyName("total")]
    public int Total { get; set; }
}

public class PaymentSummary
{
    [JsonPropertyName("totalCash")]
    public int TotalCash { get; set; }

    [JsonPropertyName("totalQr")]
    public int TotalQr { get; set; }

    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("cashPercentage")]
    public int CashPercentage { get; set; }

    [JsonPropertyName("qrPercentage")]
    public int QrPercentage { get; set; }
}

public class LowStockMaterialsResponse
{
    [JsonPropertyName("materials")]
    public List<LowStockMaterial> Materials { get; set; }
}

public class LowStockMaterial
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("currentStock")]
    public float CurrentStock { get; set; }

    [JsonPropertyName("originalStock")]
    public float OriginalStock { get; set; }

    [JsonPropertyName("unit")]
    public string Unit { get; set; }

    [JsonPropertyName("percentRemaining")]
    public int PercentRemaining { get; set; }

    [JsonPropertyName("expiredDate")]
    public string ExpiredDate { get; set; }
}
