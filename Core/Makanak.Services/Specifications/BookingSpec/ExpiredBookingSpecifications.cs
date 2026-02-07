using Makanak.Domain.Models.BookingEntities;
using Makanak.Shared.EnumsHelper.Booking;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Services.Specifications.BookingSpec
{
    public class ExpiredBookingSpecifications : BaseSpecifications<Booking,int>
    {
        public ExpiredBookingSpecifications()
            :base (b=> b.CheckOutDate < DateTime.UtcNow && 
                (b.Status == BookingStatus.CheckedIn || b.Status == BookingStatus.PaymentReceived)
            )
        {
            AddInclude(b => b.Tenant);  
            AddInclude(b => b.Property);
        }
    }
}
