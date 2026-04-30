using Makanak.Abstraction.IServices.Booking;
using Makanak.Abstraction.IServices.Manager;
using Makanak.Shared.EnumsHelper.Booking;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Makanak.Presentation.Controllers.Payment_Controller
{
    public class PaymentController(IServiceManager serviceManager, ILogger<PaymentController> logger) : AppBaseController
    {
        [AllowAnonymous] // 🔓 السماح لأي حد يبعته، لأن الريكويست جاي من Paymob
        [HttpPost("/api/payment/paymob-webhook")]
        public async Task<ActionResult> PaymobWebhook([FromQuery] string hmac)
        {
            if (string.IsNullOrEmpty(hmac))
            {
                logger.LogWarning("Webhook received without HMAC signature.");
                return UnauthorizedError("Missing HMAC signature.");
            }

            // 1. قراءة الريكويست فقط
            using var reader = new StreamReader(HttpContext.Request.Body);
            var json = await reader.ReadToEndAsync();

            // 2. السيرفيس بتعمل كل حاجة (Parsing + Validation)
            var webhookResult = await serviceManager.PaymentService.ProcessPaymobWebhookAsync(json, hmac);

            if (!webhookResult.IsValid)
            {
                logger.LogCritical("HMAC Signature mismatch! Possible forged request.");
                return UnauthorizedError("Invalid HMAC signature.");
            }

            // 3. تحديث الحجز بناءً على النتيجة النظيفة اللي رجعت
            var newStatus = webhookResult.IsSuccess
                ? BookingStatus.PaymentReceived
                : BookingStatus.PaymentFailed;
            //var transactionId = webhookResult.IsSuccess ? webhookResult.TransactionId : null;

            await serviceManager.BookingService.UpdateBookingStatusByBookingIdAsync(
                webhookResult.BookingId,
                newStatus,
                webhookResult.TransactionId
            );

            logger.LogInformation("Webhook processed successfully for BookingId: {BookingId}", webhookResult.BookingId);
            return Success("Webhook Processed Successfully");
        }

        [HttpPost("refund/{bookingId}/confirm")]
        [Authorize(Roles = "Admin")] // 🔒 حماية قسوى للأدمن فقط
        public async Task<ActionResult> ConfirmRefund(int bookingId)
        {
            var result = await serviceManager.PaymentService.ConfirmManualRefundAsync(bookingId);

            if (!result)
                return BadRequestError("Failed to confirm refund. Check booking status.");

            return Success("Refund confirmed successfully. Tenant has been notified.");
        }

        [HttpPost("refund/{bookingId}/reject")]
        [Authorize(Roles = "Admin")] // 🔒 للأدمن فقط
        public async Task<ActionResult> RejectRefund(int bookingId, [FromBody] RejectRefundDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Reason))
                return BadRequestError("Rejection reason is required.");

            var result = await serviceManager.PaymentService.RejectManualRefundAsync(bookingId, request.Reason);

            if (!result)
                return BadRequestError("Failed to reject refund.");

            return Success("Refund rejected successfully. Tenant has been notified.");
        }
    }
    public class RejectRefundDto
    {
        public string Reason { get; set; }
    }
    // DTO صغير عشان الفرونت يبعت فيه سبب الرفض
}
