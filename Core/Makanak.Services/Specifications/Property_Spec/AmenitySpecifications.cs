using Makanak.Domain.Models.PropertyEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Services.Specifications.Property_Spec
{
    public class AmenitySpecifications : BaseSpecifications<Amenity, int>
    {
        public AmenitySpecifications(List<int> amenityIds) 
            : base(Amenity => amenityIds.Contains(Amenity.Id))
        {
            
        }
    }
}
