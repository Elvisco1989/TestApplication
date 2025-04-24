using TestApplication;

public class CustomerRepo
{
    private List<Customer> _customers = new List<Customer>();
    private List<CustomerDeliveryDates> _links = new List<CustomerDeliveryDates>();
    private int _nextId = 1;

    // Get all customers
    public List<Customer> GetAllCustomers()
    {
        return _customers;
    }

    // Add a new customer with a unique ID
    public Customer AddCustomer(Customer customer)
    {
        customer.customerId = _nextId++;
        _customers.Add(customer);
        return customer;
    }

    // Assign delivery dates to a customer
    public void AssignDeliveryDates(Customer customer, List<DeliveryDates> deliveryDates)
    {
        foreach (var date in deliveryDates)
        {
            _links.Add(new CustomerDeliveryDates
            {
                CustomerId = customer.customerId,
                DeliveryDateId = date.DeliveryDateId,
                Customer = customer,
                DeliveryDate = date
            });
        }
    }

    // Get a customer by ID
    public Customer? GetCustomerById(int id)
    {
        return _customers.FirstOrDefault(c => c.customerId == id);
    }

    // Get delivery dates for a customer (as DTO with only DateTime)
    public List<CustomerDeliveryDateDto> GetDeliveryDatesForCustomer(int customerId)
    {
        return _links
            .Where(l => l.CustomerId == customerId)
            .Select(l => new CustomerDeliveryDateDto
            {
                DeliveryDate = l.DeliveryDate.DeliveryDate,
            })
            .ToList();
    }
}
