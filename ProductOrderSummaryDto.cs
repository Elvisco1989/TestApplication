namespace TestApplication
{
    public class ProductOrderSummaryDto
    {
        public string ProductName { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
        public int Total => Price * Quantity;
    }
}
