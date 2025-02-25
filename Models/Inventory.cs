using System;

namespace kafi.Models
{
    public class Inventory
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int QuantityInStock { get; set; }
        public int ReorderLevel { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime ExpireDate { get; set; }
        public string Unit { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
