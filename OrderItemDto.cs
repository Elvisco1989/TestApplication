namespace TestApplication
{
    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        public ProductDto Product { get; set; }

        public int UnitPrice { get; set; } // Price in cents


    }
}
