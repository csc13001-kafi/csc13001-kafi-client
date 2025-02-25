using System;
using System.Collections.Generic;
using System.Linq;

namespace kafi.Models
{
    public enum OrderStatus
    {
        New = 0,
        InProgress = 1,
        Completed = 2,
        Canceled = 3
    }
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public OrderStatus Status { get; set; } = OrderStatus.New;
        public int? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public int? CustomerId { get; set; }
        public int? TableId { get; set; }
        public ICollection<OrderItem> Items { get; set; } = [];
        public decimal Total => Items.Sum(item => item.Total);
        public Payment Payment { get; set; }
    }
}