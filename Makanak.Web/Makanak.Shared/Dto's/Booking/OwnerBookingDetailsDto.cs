using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Shared.Dto_s.Booking
{
    public class OwnerBookingDetailsDto
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public string PropertyName { get; set; }

        // بيانات المستأجر مهمة جداً للمالك
        public string TenantName { get; set; }
        public string TenantImage { get; set; }
        public string TenantPhoneNumber { get; set; }
        public string TenantIdentityImage { get; set; } // عشان يطابقها وهو بيسلمه المفتاح

        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int TotalDays { get; set; }
        public string Status { get; set; }
        public bool IsQrScanned { get; set; }

        // الفلوس (المالك لازم يشوف تفاصيل مكسبه)
        public decimal TotalPrice { get; set; }
        public decimal CommissionPaid { get; set; } // عمولة الموقع
        public decimal AmountToPayToOwner { get; set; } // الصافي اللي هيدخله

        public string? SpecialRequests { get; set; }
    }
}
