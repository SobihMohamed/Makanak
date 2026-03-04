using Makanak.Shared.EnumsHelper.Dispute;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Shared.Dto_s.Dispute
{
    public class ResolveDisputeDto
    {
        public int DisputeId { get; set; }
        public DisputeStatus Decision { get; set; } // Resolved / Rejected
        public string AdminComment { get; set; } = null!; // لازم يكتب سبب
    }
}
