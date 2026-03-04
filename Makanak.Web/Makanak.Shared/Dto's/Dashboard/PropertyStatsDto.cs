using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Shared.Dto_s.Dashboard
{
    public class PropertyStatsDto
    {
        public int TotalProperties { get; set; }
        public int ActiveProperties { get; set; }
        public int PendingApprovalProperties { get; set; }
        public int RejectedProperties { get; set; }
    }
}
