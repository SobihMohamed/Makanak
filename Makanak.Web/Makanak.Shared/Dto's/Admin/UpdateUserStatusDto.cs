using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Shared.Dto_s.Admin
{
    public class UpdateUserStatusDto
    {
        public string UserId { get; set; } = null!;
        public string NewStatus { get; set; } = null!;
        public string? RejectedReason { get; set; }
    }
}
