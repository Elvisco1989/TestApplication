namespace TestApplication
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public string Category { get; set; } // Example: "Electronics", "Clothing", etc.
        public List<OrderItem> OrderItems { get; set; } = new();

    }
}
