using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Shared.Dto_s.Booking
{
    public class ScanQrResponseDto
    {
        public int BookingId { get; set; }
        public string Status { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }

        
        public string TenantName { get; set; }
        public string? FrontIdentityImage { get; set; }

        
        public string PropertyName { get; set; }
        public string Message { get; set; } = "Scan Successful! You can hand over the keys.";
    }
}
