using System.Collections.Generic;

namespace kafi.Models
{
    public class CategoryProductsResponse
    {
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<Product> Products { get; set; } = new List<Product>();
    }
}