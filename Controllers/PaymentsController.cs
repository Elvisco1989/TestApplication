using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace TestApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly OrderRepo _orderRepo;
        private readonly ProductRepo _productRepo;
        private readonly IStripeClient _stripeClient;

        public PaymentsController(OrderRepo orderRepo, ProductRepo productRepo, IStripeClient stripeClient)
        {
            _orderRepo = orderRepo;
            _productRepo = productRepo;
            _stripeClient = stripeClient;
        }

        [HttpPost("create-payment-intent")]
        public async Task<ActionResult> CreatePaymentIntent([FromBody] string orderId)
        {
            var order = _orderRepo.GetOrder(int.Parse(orderId));
            if (order == null)
                return NotFound("Order not found.");

            int totalAmount = order.OrderItems.Sum(i => i.UnitPrice * i.Quantity);

            var options = new PaymentIntentCreateOptions
            {
                Amount = totalAmount,
                Currency = "sek",
                Metadata = new Dictionary<string, string>
        {
            { "OrderId", order.OrderId.ToString() },
            { "Products", string.Join(", ", order.OrderItems.Select(i => $"{i.Product.Name} x{i.Quantity}")) }
        }
            };

            var paymentIntentService = new PaymentIntentService(_stripeClient);
            var paymentIntent = await paymentIntentService.CreateAsync(options);

            return Ok(new { clientSecret = paymentIntent.ClientSecret });
        }


    }
}
