using Makanak.Domain.Models.BookingEntities;
using Makanak.Shared.EnumsHelper.Booking;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Services.Specifications.BookingSpec
{
    public class PendingPaymentExpiredSpecifications : BaseSpecifications<Booking,int>
    {
        public PendingPaymentExpiredSpecifications()
            : base(b => b.Status == BookingStatus.PendingPayment &&
                        b.PaymentDeadline.HasValue &&
                        b.PaymentDeadline.Value < DateTime.UtcNow)
        {
            AddInclude(b => b.Tenant);
        }
    }
}
