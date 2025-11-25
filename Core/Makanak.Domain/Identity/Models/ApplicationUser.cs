using Makanak.Domain.Identity.EnumsHelper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makanak.Domain.Identity.Models
{
    public class ApplicationUser : IdentityUser
    {
        #region Personal Info
        public string Name { get; set; } = null!;
        public string? ProfilePictureUrl { get; set; }
        public string? Address { get; set; }
        public DateTime DateOfBirth { get; set; }

        #endregion

        #region Security Info
        // but user can't do any thing without verification by send national id images
        public string? NationalId { get; set; } = null!;
        public string? NationalIdImageFrontUrl { get; set; } = null!;
        public string? NationalIdImageBackUrl { get; set; } = null!;

        #endregion
        public int StrikeCount { get; set; } = 0;
        // User type : Tenant, Owner, Admin
        public UserTypes UserType { get; set; }
        // User status : Pending, Active, Rejected, Banned
        public UserStatus UserStatus { get; set; } = UserStatus.Pending;
        public string? RejectedReason { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
