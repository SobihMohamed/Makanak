using Makanak.Shared.Dto_s.Integration;
using Makanak.Shared.Dto_s.Integration.Paymob.Webhook;
using Makanak.Shared.Dto_s.Payment;

namespace Makanak.Abstraction.IServices.PaymentService
{
    public interface IPaymentService
    {
        Task<BookingPaymentDto> CreatePaymentIntentAsync(PaymentIntentInputDto request);
        Task<PaymobWebhookResultDto> ProcessPaymobWebhookAsync(string jsonPayload, string hmac);
        Task<bool> ConfirmManualRefundAsync(int bookingId);
        Task<bool> RejectManualRefundAsync(int bookingId, string rejectionReason);

    }
}
