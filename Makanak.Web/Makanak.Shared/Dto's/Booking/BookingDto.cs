using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Shared.Dto_s.Booking
{
    public class BookingDto
    {
        public int Id { get; set; }

        // Dates & Status
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int TotalDays { get; set; }
        public string Status { get; set; }

        // Money 
        public decimal TotalPrice { get; set; } // 3000
        public decimal CommissionPaid { get; set; } // 300
        public decimal AmountToPayToOwner { get; set; } // 2700

        // Property Info (Basic)
        public int PropertyId { get; set; }
        public string PropertyName { get; set; }
        public string? PropertyMainImage { get; set; } 

        // Tenant Info (Basic)
        public string TenantName { get; set; }
        public string? TenantImage { get; set; } 
    }
}
