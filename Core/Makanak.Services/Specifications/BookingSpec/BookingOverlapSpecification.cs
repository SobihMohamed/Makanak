using Makanak.Domain.Models.BookingEntities;
using Makanak.Shared.EnumsHelper.Booking;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Services.Specifications.BookingSpec
{
    public class BookingOverlapSpecification : BaseSpecifications<Booking,int>
    {
        public BookingOverlapSpecification(int propertyId, DateTime newCheckIn, DateTime newCheckOut)
            : base(b =>
                b.PropertyId == propertyId &&
                b.Status != BookingStatus.Cancelled &&
                (b.CheckInDate < newCheckOut && b.CheckOutDate > newCheckIn )
            )
        {
            
        }
    }
}
