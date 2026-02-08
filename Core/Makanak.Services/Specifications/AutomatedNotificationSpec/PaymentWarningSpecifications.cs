using Makanak.Domain.Models.BookingEntities;
using Makanak.Shared.EnumsHelper.Booking;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Services.Specifications.AutomatedNotificationSpec
{
    public class PaymentWarningSpecification : BaseSpecifications<Booking, int>
    {
        public PaymentWarningSpecification()
        : base(b =>
            b.Status == BookingStatus.PendingPayment &&
            b.PaymentDeadline <= DateTime.UtcNow.AddMinutes(10) &&
            b.PaymentDeadline > DateTime.UtcNow &&
            !b.IsPaymentWarningSent
        )
            {
            }
    }
}
