using Makanak.Domain.Models.PropertyEntities;
using Makanak.Shared.EnumsHelper.Property;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Services.Specifications.Property_Spec
{
    public class PropertyDashboardSpecifications : BaseSpecifications<Property, int>
    {
        public PropertyDashboardSpecifications(PropertyStatus? status = null)
        : base(p => !status.HasValue || p.PropertyStatus == status)
        {
        }
    }
}
