using Makanak.Domain.Contracts;
using Makanak.Domain.EnumsHelper.User;
using Makanak.Domain.Models.BookingEntities;
using Makanak.Domain.Models.DisputeEntities;
using Makanak.Domain.Models.NotifyEnities;
using Makanak.Domain.Models.PropertyEntities;
using Makanak.Domain.Models.ReviewEntities;
using Microsoft.AspNetCore.Identity;

namespace Makanak.Domain.Models.Identity
{
    public class ApplicationUser : IdentityUser, IEntity<string>
    {
        #region Personal Info
        public string Name { get; set; } = null!;
        public string? ProfilePictureUrl { get; set; }
        public string? Address { get; set; }
        public DateTime DateOfBirth { get; set; }

        #endregion

        #region Security Info
        // but user can't do any thing without verification by send national id images
        public string? NationalId { get; set; } 
        public string? NationalIdImageFrontUrl { get; set; } 
        public string? NationalIdImageBackUrl { get; set; } 

        #endregion
        public int StrikeCount { get; set; } = 0;
        public decimal NegativeBalance { get; set; } = 0;
        // User type : Tenant, Owner, Admin
        public UserTypes UserType { get; set; }
        // User status : Pending, Active, Rejected, Banned
        public UserStatus UserStatus { get; set; } = UserStatus.Pending;
        public string? RejectedReason { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        #region Navigation Properties
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public virtual ICollection<Property> OwnedProperties { get; set; } = new List<Property>();
        public virtual ICollection<Booking> TenantBookings { get; set; } = new List<Booking>();
        public virtual ICollection<Booking> IncomingBookings { get; set; } = new List<Booking>();

        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<Dispute> Disputes { get; set; } = new List<Dispute>();
        #endregion
    }
}
