using System.Text.Json.Serialization;

namespace TestApplication
{
    public class CustomerDeliveryDates
    {
       
        public int CustomerId { get; set; }
        public int DeliveryDateId { get; set; }
        [JsonIgnore]

        public Customer Customer { get; set; }
        public DeliveryDates DeliveryDate { get; set; }


    }
}
