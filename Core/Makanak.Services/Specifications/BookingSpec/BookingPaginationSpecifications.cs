using Makanak.Domain.Models.BookingEntities;
using Makanak.Shared.Common.Params.Booking_Params;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Services.Specifications.BookingSpec
{
    public class BookingPaginationSpecifications : BaseSpecifications<Booking,int>
    {
        public BookingPaginationSpecifications(string userId, BookingSpecParams bookingParams, bool isTenant, bool isCount)
        : base(x =>
            (isTenant ? x.TenantId == userId : x.Property.OwnerId == userId) 
            &&
            (!bookingParams.Status.HasValue || x.Status == bookingParams.Status)
        )
        {
            if (!isCount) 
            {
                AddInclude(x => x.Property);
                AddInclude(x => x.Tenant);

                AddOrderByDesc(x => x.CreatedAt);

                ApplyPagenation(bookingParams.PageSize, bookingParams.PageIndex);
            }
        }
    }
}
