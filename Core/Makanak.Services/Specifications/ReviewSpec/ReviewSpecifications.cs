using Makanak.Domain.Models.ReviewEntities;
using Makanak.Shared.Common.Params;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Services.Specifications.ReviewSpec
{
    public class ReviewSpecifications : BaseSpecifications<Review,int>
    {
        // Constructor 1: للـ Pagination في عرض التقييمات
        public ReviewSpecifications(int propertyId, BaseQueryParams queryParams)
            : base(r => r.PropertyId == propertyId)
        {
            // AddIncludes
            AddInclude(r => r.Tenant);
            AddOrderByDesc(r => r.CreatedAt);
            //  Pagination
            ApplyPagenation(queryParams.PageSize,queryParams.PageIndex);
        }
        public ReviewSpecifications(int bookingId)
            : base(r => r.BookingId == bookingId)
        {
            
        }
        public ReviewSpecifications(int propertyId , bool IsPropertyReviews)
            : base(r => r.PropertyId == propertyId)
        {
            AddOrderByDesc(r => r.CreatedAt);

            AddInclude(r => r.Tenant);
        }
    }
}
