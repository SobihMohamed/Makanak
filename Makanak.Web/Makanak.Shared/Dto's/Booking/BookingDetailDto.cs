using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Shared.Dto_s.Booking
{
    public class BookingDetailDto : BookingDto
    {
        // Money Details
        public decimal PricePerNight { get; set; }
        public decimal CommissionPaid { get; set; }
        public decimal AmountToPayToOwner { get; set; }

        // Full Images
        public List<string>? GalleryImages { get; set; }

        // Extra Info
        public int NumberOfGuests { get; set; }
        public string? SpecialRequests { get; set; }

        // Security & Actions
        public string? CheckInQrCode { get; set; } // بيظهر هنا بس
        public bool IsQrScanned { get; set; }

        public string OwnerId { get; set; } // محتاجينها هنا عشان المحادثة مثلاً
    }
}
