using Makanak.Domain.Models.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Makanak.Domain.Models.ResetPassword
{
    public class UserOtp : BaseEntity<int>
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string OtpCode { get; set; } = string.Empty;
        public DateTime ExpirationTime { get; set; }
        public bool IsUsed { get; set; } = false;

        public string UserId { get; set; } = null!;
        public virtual ApplicationUser User { get; set; } = null!;

    }
}
