using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Shared.Dto_s.Admin
{
    public class UserForApprovalDto
    {
        public string UserId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public int StrikeCount { get; set; }
        public string UserType { get; set; } = null!;
        public string UserStatus { get; set; } = null!;
        public DateTime JoinAt { get; set; }
    }
}
