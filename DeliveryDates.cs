using System.Text.Json.Serialization;

namespace TestApplication
{
    public class DeliveryDates
    {
        public int DeliveryDateId { get; set; }
        public DateTime DeliveryDate { get; set; }

        public int customerId { get; set; } = 0;
        [JsonIgnore]
        public List<CustomerDeliveryDates> CustomerDeliveryDates { get; set; } = new List<CustomerDeliveryDates>();

    }
}
