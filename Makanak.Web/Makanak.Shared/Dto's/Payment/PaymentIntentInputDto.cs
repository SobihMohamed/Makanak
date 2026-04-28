using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Shared.Dto_s.Payment
{
    public class PaymentIntentInputDto
    {
        public int BookingId { get; set; }
        public decimal AmountToPayOnline { get; set; }
        public int TotalDays { get; set; }
        public string PropertyTitle { get; set; } = string.Empty;
        public string TenantFirstName { get; set; } = string.Empty;
        public string TenantLastName { get; set; } = string.Empty;
        public string TenantEmail { get; set; } = string.Empty;
        public string TenantPhoneNumber { get; set; } = string.Empty;
    }
}
