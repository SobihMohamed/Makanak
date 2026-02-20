using Makanak.Shared.EnumsHelper.Property;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Shared.Common.Params.Property_Params
{
    public class OwnerPropertyParams : QueryParams
    {
        public PropertyStatus? FilterStatus { get; set; }
    }
}
