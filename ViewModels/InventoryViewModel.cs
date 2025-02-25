using System;
using System.Collections.ObjectModel;
using kafi.Models;

namespace kafi.ViewModels
{
    public partial class InventoryViewModel
    {
        public ObservableCollection<Inventory> Inventories
        {
            get; private set;
        }

        public InventoryViewModel()
        {
            Inventories = new ObservableCollection<Inventory>
            {
                new() { Id = 1, ProductId = 101, QuantityInStock = 50, ReorderLevel = 10, LastUpdated = DateTime.Now, ExpireDate = DateTime.Now.AddMonths(6), Unit = "Kg", UnitPrice = 12.5m },
                new(){ Id = 2, ProductId = 102, QuantityInStock = 30, ReorderLevel = 5, LastUpdated = DateTime.Now, ExpireDate = DateTime.Now.AddMonths(3), Unit = "L", UnitPrice = 8.99m },
                new() { Id = 3, ProductId = 103, QuantityInStock = 100, ReorderLevel = 20, LastUpdated = DateTime.Now, ExpireDate = DateTime.Now.AddMonths(12), Unit = "Pack", UnitPrice = 2.5m },
                new() { Id = 4, ProductId = 104, QuantityInStock = 75, ReorderLevel = 15, LastUpdated = DateTime.Now, ExpireDate = DateTime.Now.AddMonths(9), Unit = "Box", UnitPrice = 20.0m },
                new() { Id = 5, ProductId = 105, QuantityInStock = 60, ReorderLevel = 12, LastUpdated = DateTime.Now, ExpireDate = DateTime.Now.AddMonths(4), Unit = "Bottle", UnitPrice = 5.49m },
                new() { Id = 1, ProductId = 101, QuantityInStock = 50, ReorderLevel = 10, LastUpdated = DateTime.Now, ExpireDate = DateTime.Now.AddMonths(6), Unit = "Kg", UnitPrice = 12.5m },
                new() { Id = 2, ProductId = 102, QuantityInStock = 30, ReorderLevel = 5, LastUpdated = DateTime.Now, ExpireDate = DateTime.Now.AddMonths(3), Unit = "L", UnitPrice = 8.99m },
                new() { Id = 3, ProductId = 103, QuantityInStock = 100, ReorderLevel = 20, LastUpdated = DateTime.Now, ExpireDate = DateTime.Now.AddMonths(12), Unit = "Pack", UnitPrice = 2.5m },
                new() { Id = 4, ProductId = 104, QuantityInStock = 75, ReorderLevel = 15, LastUpdated = DateTime.Now, ExpireDate = DateTime.Now.AddMonths(9), Unit = "Box", UnitPrice = 20.0m },
                new() { Id = 5, ProductId = 105, QuantityInStock = 60, ReorderLevel = 12, LastUpdated = DateTime.Now, ExpireDate = DateTime.Now.AddMonths(4), Unit = "Bottle", UnitPrice = 5.49m }
            };
        }
    }
}
