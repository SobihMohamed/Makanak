using Makanak.Shared.Dto_s.Property;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Shared.Dto_s.Admin
{
    public class AdminBookingDetailsDto
    {
        public int Id { get; set; }
        public string Status { get; set; } // عشان ترجع كـ String
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public DateTime CreatedAt { get; set; }

        // 💰 Financial Details (مهمة جداً للأدمن)
        public decimal TotalPrice { get; set; }
        public decimal CommissionPaid { get; set; }
        public string? TransactionId { get; set; }
        public bool IsRefunded { get; set; }
        public decimal? RefundedAmount { get; set; }
        public string? CancellationReason { get; set; }

        // 🏠 Property Details
        public int PropertyId { get; set; }
        public string PropertyTitle { get; set; }
        public string? PropertyMainImage { get; set; }
        // Full Images
        public List<PropertyImageDto> PropertyImages { get; set; } = new();

        // 👤 Tenant Details (بدون أي إخفاء)
        public string TenantId { get; set; }
        public string TenantName { get; set; }
        public string TenantPhoneNumber { get; set; }

        // 👑 Owner Details
        public string OwnerId { get; set; }
        public string OwnerName { get; set; }
        public string OwnerPhoneNumber { get; set; }
    }
}
