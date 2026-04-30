using Makanak.Domain.Models.BookingEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Services.Specifications.BookingSpec
{
    public class BookingPaymentSpecififcations : BaseSpecifications<Booking,int>
    {
        public BookingPaymentSpecififcations(int bookingId)
            :base(b=> b.Id == bookingId)
        {
            AddInclude(b => b.Tenant);
            AddInclude(b => b.Property);
            AddInclude(b => b.Owner);
        }
    }
}
