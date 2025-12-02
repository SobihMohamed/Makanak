using Makanak.Domain.Models.BookingEntities;
using Makanak.Domain.Models.Identity;
using Makanak.Domain.Models.PropertyEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makanak.Domain.Models.ReviewEntities
{
    public class Review : BaseEntity<int>
    {
        public int Rating { get; set; } // 1 to 5
        public string? Comment { get; set; } = null!;
        public int BookingId { get; set; }
        public virtual Booking Booking { get; set; } = null!;
        public int PropertyId { get; set; }
        public virtual Property Property { get; set; } = null!;
        public string TenantId { get; set; } = null!;
        public virtual ApplicationUser Tenant { get; set; } = null!;

    }
}
