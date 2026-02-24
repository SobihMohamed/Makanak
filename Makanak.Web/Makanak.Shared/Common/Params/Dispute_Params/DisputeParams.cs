using Makanak.Shared.EnumsHelper.Dispute;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Shared.Common.Params.Dispute_Params
{
    public class DisputeParams : BaseQueryParams
    {
        public DisputeStatus? Status { get; set; }
        public int? BookingId { get; set; }

        public DisputeSortEnum? Sort { get; set; }
    }
}
