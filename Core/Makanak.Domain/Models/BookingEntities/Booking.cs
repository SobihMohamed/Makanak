using Makanak.Domain.Models.DisputeEntities;
using Makanak.Domain.Models.Identity;
using Makanak.Domain.Models.PropertyEntities;
using Makanak.Domain.Models.ReviewEntities;
using Makanak.Shared.EnumsHelper.Booking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makanak.Domain.Models.BookingEntities
{
    public class Booking : BaseEntity<int>
    {
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }

        public int TotalDays { get; set; }

        public decimal PricePerNight { get; set; }
        public decimal TotalPrice { get; set; } 

        public decimal CommissionPaid { get; set; } // My Money 
        public decimal AmountToPayToOwner { get; set; } 

        public BookingStatus Status { get; set; } = BookingStatus.PendingPayment;

        public string? CheckInQrCode { get; set; }
        public bool IsQrScanned { get; set; } = false;

        public string? CancellationReason { get; set; } 
        public bool IsRefunded { get; set; } = false; // هل رجعت العربون للمستأجر؟
        public bool HasDispute { get; set; } = false; // هل فيه خناقة؟

        public int NumberOfGuests { get; set; }
        public string? SpecialRequests { get; set; } // "ملاحظات إضافية"
        public string? PaymentIntentId { get; set; } // رقم عملية الدفع


        public int PropertyId { get; set; }
        public virtual Property Property { get; set; } = null!;

        public string TenantId { get; set; } = null!;
        public virtual ApplicationUser Tenant { get; set; } = null!;

        public string OwnerId { get; set; } = null!;
        public virtual ApplicationUser Owner { get; set; } = null!;
        public virtual ICollection<Dispute> Disputes { get; set; } = new List<Dispute>();
         public virtual Review? Review { get; set; }
    }
}
