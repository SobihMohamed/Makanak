using Makanak.Domain.Models.BookingEntities;
using Makanak.Domain.Models.Identity;
using Makanak.Shared.EnumsHelper.Dispute;
using System;
using System.Collections.Generic;
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
        public string? AdminComment { get; set; } // قرار الأدمن (ليه رفض أو قبل)
        public int BookingId { get; set; }
        public virtual Booking Booking { get; set; } = null!;
        public string ComplaintId { get; set; } = null!;
        public virtual ApplicationUser Complaint { get; set; } = null!;
        public ICollection<DisputeImage> DisputeImages { get; set; } = new List<DisputeImage>();
    }
}
