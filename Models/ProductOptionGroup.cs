using System.Collections.Generic;

namespace kafi.Models
{
    public enum ProductModifierGroupType
    {
        Single = 1,
        Multiple = 2
    }

    public class ProductOptionGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ProductModifierGroupType Type { get; set; }
        public bool IsRequired { get; set; }
        public List<ProductOption> Options { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
