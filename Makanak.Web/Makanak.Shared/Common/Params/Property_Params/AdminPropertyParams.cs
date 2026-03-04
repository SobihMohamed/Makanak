using Makanak.Shared.EnumsHelper.Property;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Shared.Common.Params.Property_Params
{
    public class AdminPropertyParams : QueryParams
    {
        public PropertyStatus? Status { get; set; }
        public PropertyType? Type { get; set; }
        public int? GovernorateId { get; set; }
    }
}
