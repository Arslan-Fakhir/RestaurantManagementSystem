using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using RestaurantDatabaseManagement.Data;
using RestaurantDatabaseManagement.Models.Request;
using Stripe;

namespace RestaurantDatabaseManagement.Services.Implementations
{
    public class StripePaymentIntentsService
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _ctx;
        public StripePaymentIntentsService(IConfiguration configuration, ApplicationDbContext ctx)
        {
            _configuration = configuration;
            _ctx = ctx;
        }
        private async Task<double> CalculateTotalAmount(List<OrderItemRequest> items)
        {
            double totalAmount = 0;
            foreach (var item in items)
            {
                var existingItem = await _ctx.Items
                    .Where(c => c.item_name == item.item_name)
                    .FirstOrDefaultAsync();

                if (existingItem == null)
                    return 0;

                totalAmount += existingItem.price * item.quantity;
            }
            return totalAmount;
        }

        public async Task<string> CreatePayment(OrderRequest order)
        {
            try
            {
                double totalAmount = await CalculateTotalAmount(order.Items);
                if (totalAmount > 0)
                {
                    long amount = Convert.ToInt64(totalAmount * 100);

                    StripeConfiguration.ApiKey = _configuration.GetValue<string>("Stripe:SecretKey");

                    var options = new PaymentIntentCreateOptions
                    {
                        Amount = amount,
                        Currency = "usd",
                        PaymentMethod = "pm_card_visa",   // test card
                        Confirm = true,                   // confirm immediately
                        CaptureMethod = "automatic",      // or "manual" if you want capture later
                        AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                        {
                            Enabled = true,
                            AllowRedirects = "never"      // block redirect methods
                        }
                    };



                    var service = new PaymentIntentService();
                    PaymentIntent paymentIntent = service.Create(options);

                    if (paymentIntent.Id.Length != 0)
                    {
                        /*string status = await CapturePaymentIntent(paymentIntent.Id);

                        if(status.Contains("succeeded"))*/
                           return paymentIntent.Id;

                        //return "Transaction failed!";
                    }
                    else
                        return "Transaction failed! payment intent not found.";
                }
                else
                {
                    return "Transaction failed! Total amount must be greater than zero.";
                }

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> CapturePaymentIntent(string transactionId)
        {
            try
            {
                StripeConfiguration.ApiKey = _configuration.GetValue<string>("Stripe:SecretKey");

                var service = new PaymentIntentService();
                PaymentIntent paymentIntent = service.Capture(transactionId);

                return $"Transaction {paymentIntent.Status}";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

    }
}
