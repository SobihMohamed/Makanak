using Makanak.Domain.Models.BookingEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Services.Specifications.BookingSpec
{
    public class AdminBookingDetailsSpecifications : BaseSpecifications<Booking, int>
    {
        public AdminBookingDetailsSpecifications(int bookingId)
            : base(b => b.Id == bookingId)
        {
            AddInclude(b => b.Property);
            AddInclude(b => b.Property.PropertyImages);
            AddInclude(b => b.Tenant);
            AddInclude(b => b.Owner);
        }
    }
}
