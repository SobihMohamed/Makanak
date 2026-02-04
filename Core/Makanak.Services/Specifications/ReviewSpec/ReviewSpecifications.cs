using Makanak.Domain.Models.ReviewEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Services.Specifications.ReviewSpec
{
    public class ReviewSpecifications : BaseSpecifications<Review,int>
    {
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
