using Makanak.Domain.Models.BookingEntities;
using Makanak.Shared.Common;
using Makanak.Shared.Common.Params.Booking_Params;
using Makanak.Shared.Common.Params.Property_Params;
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
            &&
            (string.IsNullOrEmpty(bookingParams.Search) ||
               x.Property.AreaName.ToLower().Contains(bookingParams.Search) ||
               x.Property.Title.ToLower().Contains(bookingParams.Search) ||
               x.Property.Description.ToLower().Contains(bookingParams.Search))
            )
        {
            if (!isCount) 
            {
                AddInclude(x => x.Property);
                AddInclude(x => x.Tenant);
                var propertyImnagesVar = $"{nameof(Booking.Property)}.{nameof(Booking.Property.PropertyImages)}";
                AddInclude(propertyImnagesVar);

                if (bookingParams.Sort.HasValue)
                {
                    switch (bookingParams.Sort)
                    {
                        case SortingOptionsEnum.PriceAsc:
                            AddOrderBy(x => x.TotalPrice);
                            break;
                        case SortingOptionsEnum.PriceDesc:
                            AddOrderByDesc(p => p.TotalPrice);
                            break;
                        case SortingOptionsEnum.DateCreatedAsc: // Oldest
                            AddOrderBy(p => p.CreatedAt);
                            break;
                        case SortingOptionsEnum.DateCreatedDesc: // Newest
                        default:
                            AddOrderByDesc(p => p.CreatedAt);
                            break;
                    }
                }

                ApplyPagenation(bookingParams.PageSize, bookingParams.PageIndex);
            }
            

        }
    }
}
