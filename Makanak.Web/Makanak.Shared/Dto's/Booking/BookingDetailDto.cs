using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Shared.Dto_s.Booking
{
    public class BookingDetailDto : BookingDto
    {
        // Money Details
        public decimal PricePerNight { get; set; }

        // Full Images
        public List<string>? GalleryImages { get; set; }

        // Extra Info
        public int NumberOfGuests { get; set; }
        public string? SpecialRequests { get; set; }

        // Security & Actions
        public string? CheckInQrCode { get; set; } // بيظهر هنا بس
        public bool IsQrScanned { get; set; }

        // Sensitive Data (Locked until paid) 🔒
        public string? OwnerPhoneNumber { get; set; }
        public string? CheckInInstructions { get; set; }
        public string? ExactLocationUrl { get; set; }
    }
}
