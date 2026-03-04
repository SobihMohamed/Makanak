using Makanak.Domain.Models.BookingEntities;
using Makanak.Shared.EnumsHelper.Booking;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Services.Specifications.BookingSpec
{
    public class BookingDashboardSpecifications : BaseSpecifications<Booking, int>
    {
        public BookingDashboardSpecifications(BookingStatus? status = null)
            : base(b => !status.HasValue || b.Status == status)
        {
        }
        // for financial stats.
        public BookingDashboardSpecifications()
        : base(b => b.Status == BookingStatus.PaymentReceived ||
                    b.Status == BookingStatus.CheckedIn ||
                    b.Status == BookingStatus.Completed)
        {
        }
    }
}