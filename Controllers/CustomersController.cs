using Microsoft.AspNetCore.Mvc;
using TestApplication;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly CustomerRepo _customerRepo;
    private readonly DeliveryDateRepo _deliveryDateRepo;
    private readonly ProductRepo _productRepo; // ✅ Added

    public CustomerController(CustomerRepo customerRepo, DeliveryDateRepo deliveryDateRepo, ProductRepo productRepo)
    {
        _customerRepo = customerRepo;
        _deliveryDateRepo = deliveryDateRepo;
        _productRepo = productRepo; // ✅ Assigned
    }

    // GET: api/customer
    [HttpGet]
    public ActionResult<List<CustomerDto>> GetAllCustomers()
    {
        var customers = _customerRepo.GetAllCustomers();

        if (customers == null || !customers.Any())
            return NotFound("No customers found.");

        var customerDtos = customers.Select(c =>
        {
            c.CustomerDeliveryDates = _customerRepo.GetDeliveryDatesForCustomer(c.customerId);
            return MapToCustomerDto(c);
        }).ToList();

        return Ok(customerDtos);
    }

    // GET: api/customer/5
    [HttpGet("{id}")]
    public ActionResult<CustomerDto> GetCustomerById(int id)
    {
        var customer = _customerRepo.GetCustomerById(id);

        if (customer == null)
            return NotFound($"Customer with ID {id} not found.");

        customer.CustomerDeliveryDates = _customerRepo.GetDeliveryDatesForCustomer(id);

        return Ok(MapToCustomerDto(customer));
    }

    // POST: api/customer
    [HttpPost]
    public ActionResult<CustomerDto> AddCustomer([FromBody] CreatCustomer creatCustomer)
    {
        var customer = new Customer
        {
            Name = creatCustomer.Name,
            Address = creatCustomer.Address,
            Segment = creatCustomer.Segment
        };

        var addedCustomer = _customerRepo.AddCustomer(customer);

        // Assign delivery dates based on segment
        if (Enum.TryParse<Segment>(customer.Segment, out var segmentEnum))
        {
            var matchingDates = _deliveryDateRepo.GetDeliveryDatesForSegment(segmentEnum, 10);
            _customerRepo.AssignDeliveryDates(addedCustomer, matchingDates);
        }
        else
        {
            return BadRequest("Invalid segment value.");
        }

        addedCustomer.CustomerDeliveryDates = _customerRepo.GetDeliveryDatesForCustomer(addedCustomer.customerId);

        return CreatedAtAction(nameof(GetCustomerById), new { id = addedCustomer.customerId }, MapToCustomerDto(addedCustomer));
    }

    // GET: api/customer/search?id=1&deliveryCount=5
    [HttpGet("search")]
    public ActionResult<List<CustomerDto>> SearchCustomers([FromQuery] int? id, [FromQuery] int? deliveryCount)
    {
        var customers = _customerRepo.GetAllCustomers();

        if (id.HasValue)
            customers = customers.Where(c => c.customerId == id.Value).ToList();

        foreach (var customer in customers)
        {
            customer.CustomerDeliveryDates = _customerRepo.GetDeliveryDatesForCustomer(customer.customerId);
        }

        if (deliveryCount.HasValue)
            customers = customers.Where(c => c.CustomerDeliveryDates.Count == deliveryCount.Value).ToList();

        if (!customers.Any())
            return NotFound("No matching customers found.");

        var customerDtos = customers.Select(MapToCustomerDto).ToList();

        return Ok(customerDtos);
    }

    // 🔁 Mapper method from Customer → CustomerDto
    private CustomerDto MapToCustomerDto(Customer customer)
    {
        // ✅ Fix: Populate Product for each OrderItem
        foreach (var order in customer.Orders)
        {
            foreach (var item in order.OrderItems)
            {
                item.Product = _productRepo.GetProductById(item.ProductId);
            }
        }

        return new CustomerDto
        {
            CustomerId = customer.customerId,
            Name = customer.Name,
            Address = customer.Address,
            Segment = customer.Segment,
            DeliveryDates = customer.CustomerDeliveryDates.Select(d => d.DeliveryDate).ToList(),
            Orders = customer.Orders.Select(order => new OrderDto
            {
                OrderId = order.OrderId,
                CustomerId = order.customerId,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity,
                    Product = new ProductDto
                    {
                        ProductId = oi.Product.ProductId,
                        Name = oi.Product.Name,
                        Price = oi.Product.Price,
                        Description = oi.Product.Description,
                        Category = oi.Product.Category
                    }
                }).ToList()
            }).ToList()
        };
    }

    


}
