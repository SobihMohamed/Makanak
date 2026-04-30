using Makanak.Domain.Models.BookingEntities;
using Makanak.Shared.Common;
using Makanak.Shared.Common.Params.Booking_Params;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Services.Specifications.BookingSpec
{
    public class AdminBookingPaginationSpecifications : BaseSpecifications<Booking, int>
    {
        public AdminBookingPaginationSpecifications(BookingSpecParams bookingParams, bool isCount = false)
            : base(b =>
                (!bookingParams.Status.HasValue || b.Status == bookingParams.Status) &&
                (string.IsNullOrEmpty(bookingParams.Search) || b.Property.Title.ToLower().Contains(bookingParams.Search))
            )
        {
            if (!isCount)
            {
                AddInclude(b => b.Tenant);
                AddInclude(b => b.Owner);
                AddInclude(b => b.Property);

                ApplyPagenation(bookingParams.PageSize, bookingParams.PageIndex);

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
                else
                {
                    AddOrderByDesc(b => b.CreatedAt);
                }
            }
        }
    }
}
