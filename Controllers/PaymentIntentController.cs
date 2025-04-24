using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace TestApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentIntentController : ControllerBase
    {
        [HttpPost("create-payment-intent")]
        public async Task<ActionResult<object>> CreatePaymentIntent([FromBody] PaymentRequestDto paymentRequest)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = paymentRequest.Amount,
                Currency = "dkk", // or "eur", "usd", etc.
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
            };

            var service = new PaymentIntentService();
            var intent = await service.CreateAsync(options);

            return Ok(new { clientSecret = intent.ClientSecret });
        }

    }
}
