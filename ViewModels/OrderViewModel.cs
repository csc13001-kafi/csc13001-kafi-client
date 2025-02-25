using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using kafi.Models;

namespace kafi.ViewModels
{
    public partial class OrderViewModel
    {
        public ObservableCollection<Order> Orders
        {
            get; private set;
        }

        private readonly Dictionary<int, string> _employeeNames = new()
        {
            { 101, "John Doe" },
            { 102, "Jane Doe" },
            { 103, "Alice Doe" },
            { 104, "Bob Doe" },
            { 105, "Charlie Doe" },
            { 106, "David Doe" },
            { 107, "Eve Doe" },
            { 108, "Frank Doe" },
            { 109, "Grace Doe" },
            { 110, "Henry Doe" }
        };

        public OrderViewModel()
        {
            Orders =
            [
                new Order
                {
                    Id = 1,
                    OrderDate = new DateTime(2025, 2, 25, 12, 0, 0),
                    Status = OrderStatus.InProgress,
                    EmployeeId = 101,
                    Payment = new()
                    {
                        Id = 10,
                        PaymentType = PaymentType.Cash,
                        Status = PaymentStatus.Completed,
                        PaymentDate = new DateTime(2025, 2, 25, 12, 15, 0),
                        Amount = 250.00m
                    },
                    Items =
                    [
                         new OrderItem { Id = 1, Price = 150.00m, Quantity = 1},
                         new OrderItem { Id = 2, Price = 100.00m, Quantity = 1}
                    ]
                },
                new Order
                {
                    Id = 2,
                    OrderDate = new DateTime(2025, 2, 26, 14, 30, 0),
                    Status = OrderStatus.InProgress,
                    EmployeeId = 102,
                    Payment = new Payment
                    {
                        Id = 1020,
                        PaymentType = PaymentType.Banking,
                        Status = PaymentStatus.Pending,
                        PaymentDate = new DateTime(2025, 2, 26, 14, 45, 0),
                        Amount = 300.00m
                    },
                    Items =
                    [
                        new OrderItem { Id = 3, Price = 300.00m, Quantity = 1}
                    ]
                },
                new Order
                {
                    Id = 3,
                    OrderDate = new DateTime(2025, 2, 27, 10, 0, 0),
                    Status = OrderStatus.Completed,
                    EmployeeId = 103,
                    Payment = new Payment
                    {
                        Id = 1030,
                        PaymentType = PaymentType.Momo,
                        Status = PaymentStatus.Completed,
                        PaymentDate = new DateTime(2025, 2, 27, 10, 15, 0),
                        Amount = 200.00m
                    },
                    Items =
                    [
                        new OrderItem { Id = 4, Price = 120.00m, Quantity = 1},
                        new OrderItem { Id = 5, Price = 80.00m, Quantity = 1}
                    ]
                },
                new Order
                {
                    Id = 4,
                    OrderDate = new DateTime(2025, 2, 28, 16, 0, 0),
                    Status = OrderStatus.Canceled,
                    EmployeeId = 104,
                    Payment = new Payment
                    {
                        Id = 1040,
                        PaymentType = PaymentType.Cash,
                        Status = PaymentStatus.Canceled,
                        PaymentDate = new DateTime(2025, 2, 28, 16, 15, 0),
                        Amount = 150.00m
                    },
                    Items =
                    [
                        new OrderItem { Id = 6, Price = 150.00m, Quantity = 1}
                    ]
                },
                new Order
                {
                    Id = 5,
                    OrderDate = new DateTime(2025, 3, 1, 11, 0, 0),
                    Status = OrderStatus.New,
                    EmployeeId = 105,
                    Payment = new Payment
                    {
                        Id = 1050,
                        PaymentType = PaymentType.Banking,
                        Status = PaymentStatus.Pending,
                        PaymentDate = new DateTime(2025, 3, 1, 11, 15, 0),
                        Amount = 400.00m
                    },
                    Items =
                    [
                        new OrderItem { Id = 7, Price = 200.00m, Quantity = 1},
                        new OrderItem { Id = 8, Price = 200.00m, Quantity = 1}
                    ]
                },
                new Order
                {
                    Id = 6,
                    OrderDate = new DateTime(2025, 3, 2, 9, 30, 0),
                    Status = OrderStatus.InProgress,
                    EmployeeId = 106,
                    Payment = new Payment
                    {
                        Id = 1060,
                        PaymentType = PaymentType.Momo,
                        Status = PaymentStatus.Completed,
                        PaymentDate = new DateTime(2025, 3, 2, 9, 45, 0),
                        Amount = 500.00m
                    },
                    Items =
                    [
                        new OrderItem { Id = 9, Price = 250.00m, Quantity = 1},
                        new OrderItem { Id = 10, Price = 250.00m, Quantity = 1}
                    ]
                },
                new Order
                {
                    Id = 7,
                    OrderDate = new DateTime(2025, 3, 3, 18, 0, 0),
                    Status = OrderStatus.Completed,
                    EmployeeId = 107,
                    Payment = new Payment
                    {
                        Id = 1070,
                        PaymentType = PaymentType.Cash,
                        Status = PaymentStatus.Completed,
                        PaymentDate = new DateTime(2025, 3, 3, 18, 15, 0),
                        Amount = 350.00m
                    },
                    Items =
                    [
                        new OrderItem { Id = 11, Price = 200.00m, Quantity = 1},
                        new OrderItem { Id = 12, Price = 150.00m, Quantity = 1}
                    ]
                },
                new Order
                {
                    Id = 8,
                    OrderDate = new DateTime(2025, 3, 4, 13, 0, 0),
                    Status = OrderStatus.New,
                    EmployeeId = 108,
                    Payment = new Payment
                    {
                        Id = 1080,
                        PaymentType = PaymentType.Banking,
                        Status = PaymentStatus.Pending,
                        PaymentDate = new DateTime(2025, 3, 4, 13, 15, 0),
                        Amount = 275.00m
                    },
                    Items =
                    [
                        new OrderItem { Id = 13, Price = 275.00m, Quantity = 1}
                    ]
                },
                new Order
                {
                    Id = 9,
                    OrderDate = new DateTime(2025, 3, 5, 15, 0, 0),
                    Status = OrderStatus.Canceled,
                    EmployeeId = 109,
                    Payment = new Payment
                    {
                        Id = 1090,
                        PaymentType = PaymentType.Momo,
                        Status = PaymentStatus.Canceled,
                        PaymentDate = new DateTime(2025, 3, 5, 15, 15, 0),
                        Amount = 100.00m
                    },
                    Items =
                    [
                        new OrderItem { Id = 14, Price = 100.00m, Quantity = 1}
                    ]
                },
                new Order
                {
                    Id = 10,
                    OrderDate = new DateTime(2025, 3, 6, 10, 30, 0),
                    Status = OrderStatus.InProgress,
                    EmployeeId = 110,
                    Payment = new Payment
                    {
                        Id = 1100,
                        PaymentType = PaymentType.Cash,
                        Status = PaymentStatus.Pending,
                        PaymentDate = new DateTime(2025, 3, 6, 10, 45, 0),
                        Amount = 600.00m
                    },
                    Items =
                    [
                        new OrderItem { Id = 15, Price = 400.00m, Quantity = 1},
                        new OrderItem { Id = 16, Price = 200.00m, Quantity = 1}
                    ]
                }
            ];

            foreach (var order in Orders)
            {
                order.EmployeeName = _employeeNames[order.EmployeeId.Value];
            }
        }
    }
}
