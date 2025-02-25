using System.Collections.Generic;
using System.Linq;

namespace kafi.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public List<OrderItemOption> SelectedOptions { get; set; } = [];
        public decimal Total => (Price + SelectedOptions.Sum(modifier => modifier.AdditionalPrice)) * Quantity;
    }
}
