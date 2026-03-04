using Makanak.Shared.Dto_s.Payment;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Abstraction.IServices.PaymentService
{
    public interface IPaymentService
    {
        Task<BookingPaymentDto> CreateOrUpdatePaymentIntent(string paymentIntentId, decimal amount);

        Task<BookingPaymentDto> MapWebhookEvent(string json, string signature);
    }
}
