namespace TestApplication
{
    public class PaymentRequest
    {
        public int OrderId { get; set; } // ID of the order
        public List<OrderItemDto> Items { get; set; } = new();
        public string Currency { get; set; } = "dkk";
    }
}
