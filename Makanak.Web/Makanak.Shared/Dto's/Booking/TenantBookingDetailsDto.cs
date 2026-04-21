using Makanak.Shared.Dto_s.Property;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Shared.Dto_s.Booking
{
    public class TenantBookingDetailsDto
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public string PropertyName { get; set; }
        public string PropertyMainImage { get; set; }
        public string OwnerPhoneNumber { get; set; } // هيظهر بس لو الدفع تم
        public string ExactLocationUrl { get; set; } // هيظهر بس لو الدفع تم
        public string CheckInInstructions { get; set; } // هيظهر بس لو الدفع تم
        public string CheckInQrCode { get; set; } // المستأجر محتاجه عشان يوريه للمالك
        public bool IsQrScanned { get; set; }
        public List<PropertyImageDto> PropertyImages { get; set; } = new();
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int TotalDays { get; set; }
        public string Status { get; set; }

        public decimal TotalPrice { get; set; } // السعر الإجمالي اللي هيدفعه
        public string? SpecialRequests { get; set; }
    }
}
