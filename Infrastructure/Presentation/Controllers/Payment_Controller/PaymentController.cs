using Makanak.Abstraction.IServices.Manager;
using Makanak.Shared.EnumsHelper.Booking;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Makanak.Presentation.Controllers.Payment_Controller
{
    // called by stripe server when the payment success
    public class PaymentController(IServiceManager serviceManager , ILogger<PaymentController> logger) : AppBaseController
    {
        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            // 1. read body and header
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var signature = Request.Headers["Stripe-Signature"];

            try
            {
                // 2. send to service to check that the signature is true 
                var result = await serviceManager.PaymentService.MapWebhookEvent(json, signature);

                if (result == null)
                    return BadRequestError("Stripe Signature Verification Failed");

                // 3. تحديث حالة الحجز بناءً على النتيجة
                if (result.Status == "succeeded")
                {
                    logger.LogInformation("Payment Succeeded for {Id}", result.PaymentIntentId);
                    await serviceManager.BookingService.UpdateBookingStatusByIntentIdAsync(result.PaymentIntentId, BookingStatus.PaymentReceived);
                }
                else if (result.Status == "failed")
                {
                    logger.LogInformation("Payment Failed for {Id}", result.PaymentIntentId);
                    await serviceManager.BookingService.UpdateBookingStatusByIntentIdAsync(result.PaymentIntentId, BookingStatus.PaymentFailed);
                }

                // 4. لازم نرد بـ Success عشان Stripe يرتاح
                return Success("Webhook Handled Successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Webhook Error");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
