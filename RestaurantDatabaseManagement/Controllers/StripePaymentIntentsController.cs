using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestaurantDatabaseManagement.Models.Request;
using RestaurantDatabaseManagement.Services.Implementations;
using Stripe;

namespace RestaurantDatabaseManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StripePaymentIntentsController : ControllerBase
    {
        private readonly StripePaymentIntentsService _service;
        public StripePaymentIntentsController(StripePaymentIntentsService service)

        {
            _service = service;
        }

        [HttpPost("CreateIntent")]
        public async Task<IActionResult> CreateIntent(OrderRequest order)
        {
            try
            {
                string paymentId = await _service.CreatePayment(order);

                if (paymentId.Contains("failed"))
                    return BadRequest("Payment failed!");

                return Ok("Payment created!");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("CapturePaymentIntent({id})")]
        public async Task<IActionResult> CapturePaymentIntent(string id = " ")
        {
            try
            {
                if (id != " " || id != "string")
                {
                    string transactionStatus = await _service.CapturePaymentIntent(id);

                    if(transactionStatus.Contains("succeeded"))
                    {
                        return Ok("Payment succeeded!");
                    }

                    return BadRequest("Payment failed!");
                }

                return BadRequest("Transaction ID is required!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

