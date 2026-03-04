using Makanak.Domain.Models.ReviewEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Services.Specifications.ReviewSpec
{
    public class ReviewWithCountSpecification : BaseSpecifications<Review, int>
    {
        public ReviewWithCountSpecification(int propertyId)
            : base(r => r.PropertyId == propertyId)
        {
        }
    }
}
