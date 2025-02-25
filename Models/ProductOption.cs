namespace kafi.Models
{
    public class ProductOption
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal AdditionalPrice { get; set; } = 0;
        public int ProductOptionGroupId { get; set; }
        public ProductOptionGroup ProductOptionGroup { get; set; }
    }
}
