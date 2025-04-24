using Microsoft.AspNetCore.Mvc;
using TestApplication;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly OrderRepo _orderRepo;
    private readonly CustomerRepo _customerRepo;
    private readonly ProductRepo _productRepo;

    public OrderController(OrderRepo orderRepo, CustomerRepo customerRepo, ProductRepo productRepo)
    {
        _orderRepo = orderRepo;
        _customerRepo = customerRepo;
        _productRepo = productRepo;
    }

    // POST: api/order
    [HttpPost]
    //public ActionResult<OrderDto> PlaceOrder(OrderDto dto)
    //{
    //    // Find the customer by ID
    //    var customer = _customerRepo.GetCustomerById(dto.CustomerId);
    //    if (customer == null)
    //    {
    //        return NotFound($"Customer with ID {dto.CustomerId} not found.");
    //    }

    //    // Create the order
    //    var order = new Order
    //    {
    //        customerId = dto.CustomerId,
    //        OrderItems = new List<OrderItem>()
    //    };

    //    // Add the products to the order
    //    foreach (var item in dto.OrderItems)
    //    {
    //        // Fetch the product using ProductId
    //        var product = _productRepo.GetProductById(item.ProductId);
    //        if (product == null)
    //        {
    //            return NotFound($"Product with ID {item.ProductId} not found.");
    //        }

    //        // Create the order item
    //        var orderItem = new OrderItem
    //        {
    //            ProductId = item.ProductId,
    //            Quantity = item.Quantity,
    //            Product = product // Assign the product to the order item
    //        };

    //        // Add the order item to the order
    //        order.OrderItems.Add(orderItem);
    //    }

    //    // Save the order to the database
    //    _orderRepo.AddOrder(order);

    //    // Map the order to DTO
    //    var orderDto = new OrderDto
    //    {
    //        OrderId = order.OrderId,
    //        CustomerId = order.customerId,
    //        OrderItems = order.OrderItems.Select(oi => new OrderItemDto
    //        {
    //            ProductId = oi.ProductId,
    //            Quantity = oi.Quantity,
    //            Product = new ProductDto
    //            {
    //                ProductId = oi.Product.ProductId,
    //                Name = oi.Product.Name,
    //                Price = oi.Product.Price,
    //                Description = oi.Product.Description,
    //                Category = oi.Product.Category
    //            }
    //        }).ToList()
    //    };

    //    return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderId }, orderDto);
    //}
    [HttpPost]
    public ActionResult<OrderDto> PlaceOrder([FromBody] CreateOrderDto dto)
    {
        var customer = _customerRepo.GetCustomerById(dto.CustomerId);
        if (customer == null)
            return NotFound("Customer not found.");

        var order = new Order
        {
            customerId = dto.CustomerId,
            OrderItems = dto.OrderItems.Select(oi =>
            {
                var product = _productRepo.GetProductById(oi.ProductId);
                return new OrderItem
                {
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity,
                    Product = product
                };
            }).ToList()
        };

        var createdOrder = _orderRepo.AddOrder(order);
        customer.Orders.Add(createdOrder);

        var orderDto = new OrderDto
        {
            OrderId = createdOrder.OrderId,
            CustomerId = createdOrder.customerId,
            OrderItems = createdOrder.OrderItems.Select(oi => new OrderItemDto
            {
                ProductId = oi.ProductId,
                Quantity = oi.Quantity,
                Product = null // or omit this field in DTO
            }).ToList()
        };

        return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.OrderId }, orderDto);
    }


    // GET: api/order/{id}
    //[HttpGet("{id}")]
    //public ActionResult<OrderDto> GetOrderById(int id)
    //{
    //    var order = _orderRepo.GetOrder(id);
    //    if (order == null)
    //        return NotFound($"Order with ID {id} not found.");

    //    var orderDto = new OrderDto
    //    {
    //        OrderId = order.OrderId,
    //        CustomerId = order.customerId,
    //        OrderItems = order.OrderItems.Select(oi => new OrderItemDto
    //        {
    //            ProductId = oi.ProductId,
    //            Quantity = oi.Quantity,
    //            Product = new ProductDto
    //            {
    //                ProductId = oi.Product.ProductId,
    //                Name = oi.Product.Name,
    //                Price = oi.Product.Price,
    //                Description = oi.Product.Description,
    //                Category = oi.Product.Category
    //            }
    //        }).ToList()
    //    };

    //    return Ok(orderDto);
    //}

    //[HttpGet("{id}")]
    //public ActionResult<OrderDto> GetOrderById(int id)
    //{
    //    var order = _orderRepo.GetOrder(id);
    //    if (order == null)
    //        return NotFound($"Order with ID {id} not found.");

    //    var customer = _customerRepo.GetCustomerById(order.customerId);
    //    if (customer == null)
    //        return NotFound($"Customer with ID {order.customerId} not found.");

    //    customer.CustomerDeliveryDates = _customerRepo.GetDeliveryDatesForCustomer(customer.customerId);

    //    var nextDelivery = customer.CustomerDeliveryDates
    //        .Select(d => d.DeliveryDate)
    //        .Where(date => date > DateTime.Now)
    //        .OrderBy(date => date)
    //        .FirstOrDefault();

    //    var result = new
    //    {
    //        CustomerId = customer.customerId,
    //        CustomerName = customer.Name,
    //        CustomerAddress = customer.Address,
    //        NextDeliveryDate = nextDelivery,
    //        OrderId = order.OrderId,
    //        Products = order.OrderItems.Select(oi => new
    //        {
    //            ProductName = oi.Product?.Name ?? "Unknown",
    //            Price = oi.Product?.Price ?? 0,
    //            Quantity = oi.Quantity
    //        }).ToList()
    //    };

    //    return Ok(result);
    //}

    [HttpGet("{id}")]
    public ActionResult<OrderSummaryDto> GetOrderById(int id)
    {
        var order = _orderRepo.GetOrder(id);
        if (order == null) return NotFound();

        var customer = _customerRepo.GetCustomerById(order.customerId);
        if (customer == null) return NotFound();

        var deliveryDates = _customerRepo.GetDeliveryDatesForCustomer(customer.customerId);
        var nextDate = deliveryDates.OrderBy(d => d.DeliveryDate).FirstOrDefault()?.DeliveryDate;

        var products = order.OrderItems.Select(oi =>
        {
            var product = _productRepo.GetProductById(oi.ProductId);
            return new ProductOrderSummaryDto
            {
                ProductName = product.Name,
                Price = (int)product.Price,
                Quantity = oi.Quantity
            };
        }).ToList();

        var summary = new OrderSumDto
        {
            CustomerId = customer.customerId,
            CustomerName = customer.Name,
            CustomerAddress = customer.Address,
            NextDeliveryDate = nextDate ?? DateTime.MinValue,
            OrderId = order.OrderId,
            Products = products
        };

        return Ok(summary);
    }

    [HttpGet("by-customer/{customerId}")]
    public ActionResult<List<OrderSumDto>> GetOrdersByCustomerId(int customerId)
    {
        var customer = _customerRepo.GetCustomerById(customerId);
        if (customer == null) return NotFound();

        var deliveryDates = _customerRepo.GetDeliveryDatesForCustomer(customerId);
        var nextDate = deliveryDates.OrderBy(d => d.DeliveryDate).FirstOrDefault()?.DeliveryDate ?? DateTime.MinValue;

        var orderSummaries = customer.Orders.Select(order =>
        {
            var products = order.OrderItems.Select(oi =>
            {
                var product = _productRepo.GetProductById(oi.ProductId);
                return new ProductOrderSummaryDto
                {
                    ProductName = product?.Name ?? "Unknown",
                    Price = (int)(product?.Price ?? 0),
                    Quantity = oi.Quantity
                };
            }).ToList();

            return new OrderSumDto
            {
                CustomerId = customer.customerId,
                CustomerName = customer.Name,
                CustomerAddress = customer.Address,
                NextDeliveryDate = nextDate,
                OrderId = order.OrderId,
                Products = products
            };
        }).ToList();

        return Ok(orderSummaries);
    }




}
