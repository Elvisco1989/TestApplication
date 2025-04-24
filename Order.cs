namespace TestApplication
{
    public class Order
    {
        public int OrderId { get; set; }
        public int customerId { get; set; }

        public Customer Customer { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new();

        public int UnitPrice { get; set; } // Price in cents

       

    }
}
