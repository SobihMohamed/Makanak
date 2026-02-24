using Makanak.Domain.EnumsHelper.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Shared.Common.Params
{
    public class QueryParams : BaseQueryParams  
    {
        public SortingOptionsEnum? Sort { get; set; }
    }
}
