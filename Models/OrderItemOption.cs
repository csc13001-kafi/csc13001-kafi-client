namespace kafi.Models
{
    public class OrderItemOption
    {
        public int Id { get; set; }
        public int ProductOptionId { get; set; }
        public ProductOption ProductOption { get; set; }
        public decimal AdditionalPrice { get; set; }

    }
}
