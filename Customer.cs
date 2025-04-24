namespace TestApplication
{
    public class Customer
    {
        public int customerId { get; set; }
        public string Name { get; set; }

        public string Address { get; set; }

        public string Segment { get; set; }

        public int DeliveryDateId { get; set; } = 0;

        public List<CustomerDeliveryDateDto> CustomerDeliveryDates { get; set; } = new List<CustomerDeliveryDateDto>();
        public List<Order> Orders { get; set; } = new List<Order>();
    }
}
