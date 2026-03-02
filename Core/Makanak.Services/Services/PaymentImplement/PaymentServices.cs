using Makanak.Abstraction.IServices.PaymentService;
using Makanak.Shared.Common.Settings;
using Makanak.Shared.Dto_s.Payment;
using Microsoft.Extensions.Options;
using Stripe;
namespace Makanak.Services.Services.PaymentImplement
{
    public class PaymentServices(IOptions<StripeSettings> stripeSettings) : IPaymentService
    {
        public async Task<BookingPaymentDto> CreateOrUpdatePaymentIntent(string paymentIntentId, decimal amount)
        {
            // 1. إعداد الـ Stripe Configuration
            StripeConfiguration.ApiKey = stripeSettings.Value.SecretKey;

            var service = new PaymentIntentService();
            PaymentIntent intent = null; // نبدأ بـ null

            // حساب المبلغ بالسنت/قرش
            var amountInCents = (long)(amount * 100);

            // 2. محاولة استرجاع الـ Intent القديم لو موجود
            if (!string.IsNullOrEmpty(paymentIntentId))
            {
                try
                {
                    
                    var existingIntent = await service.GetAsync(paymentIntentId);

                    if (existingIntent.Status == "canceled" || existingIntent.Status == "succeeded")
                    {
                        paymentIntentId = null;
                    }
                    else
                    {
                        var options = new PaymentIntentUpdateOptions
                        {
                            Amount = amountInCents
                        };
                        intent = await service.UpdateAsync(paymentIntentId, options);
                    }
                }
                catch (StripeException) 
                {
                    paymentIntentId = null; 
                }
            }

            if (string.IsNullOrEmpty(paymentIntentId))
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = amountInCents,
                    Currency = "egp",
                    PaymentMethodTypes = new List<string> { "card" }
                };
                intent = await service.CreateAsync(options);
            }


            return new BookingPaymentDto
            {
                PaymentIntentId = intent.Id,
                ClientSecret = intent.ClientSecret,
                TotalAmount = amount,
            };
        }

        public Task<BookingPaymentDto> MapWebhookEvent(string json, string signature)
        {
            // CLI send the Whsecret
            const string WhSecret = "whsec_e9y0SToZkXY8DMoI5COm72mq9wv3upMg";

            try
            {
                // it used to construct event to make a signature validation sent from stripe 
                var stripeEvent = EventUtility.ConstructEvent(json, signature, WhSecret);

                var result = new BookingPaymentDto();

                // is payment succeeded or failed
                if (stripeEvent.Type == "payment_intent.succeeded")
                {
                    var intent = (PaymentIntent)stripeEvent.Data.Object;
                    result.PaymentIntentId = intent.Id;
                    result.Status = "succeeded";
                }
                else if (stripeEvent.Type == "payment_intent.payment_failed")
                {
                    var intent = (PaymentIntent)stripeEvent.Data.Object;
                    result.PaymentIntentId = intent.Id;
                    result.Status = "failed";
                }

                return Task.FromResult(result);
            }
            catch (StripeException)
            {
                return Task.FromResult<BookingPaymentDto>(null);
            }
        }
    }
}
