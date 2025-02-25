using System.Collections.Generic;

namespace kafi.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public bool IsAvailable { get; set; }
        public List<ProductOptionGroup> OptionGroups { get; set; }

    }
}
