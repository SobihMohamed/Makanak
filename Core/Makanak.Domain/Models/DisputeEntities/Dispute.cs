using Makanak.Domain.Models.BookingEntities;
using Makanak.Domain.Models.Identity;
using Makanak.Shared.EnumsHelper.Dispute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makanak.Domain.Models.DisputeEntities
{
    public class Dispute : BaseEntity<int>
    {

        public DisputeReason Reason { get; set; }
        public DisputeStatus Status { get; set; } = DisputeStatus.Pending;
        public string? Description { get; set; } // شرح اليوزر للمشكلة
        public DateTime? ResolvedAt { get; set; }
        public string? AdminComment { get; set; } // قرار الأدمن (ليه رفض أو قبل)
        public int BookingId { get; set; }
        public virtual Booking Booking { get; set; } = null!;
        public string ComplainantId { get; set; } = null!;
        public virtual ApplicationUser Complainant { get; set; } = null!;
        public ICollection<DisputeImage> DisputeImages { get; set; } = new List<DisputeImage>();

        [NotMapped]
        public string DefendantId
        {
            get
            {
                // لو الحجز مش متحمل (Null)، منقدرش نعرف (عشان نتجنب Error)
                if (Booking == null) return string.Empty;

                // لو الشاكي هو المستأجر -> يبقى الخصم هو المالك
                if (ComplainantId == Booking.TenantId) return Booking.OwnerId;

                // لو الشاكي هو المالك -> يبقى الخصم هو المستأجر
                return Booking.TenantId;
            }
        }
    }
}
