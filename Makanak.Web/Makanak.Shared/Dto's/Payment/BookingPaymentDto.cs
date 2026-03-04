using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Shared.Dto_s.Payment
{
    public class BookingPaymentDto
    {
        public int BookingId { get; set; }
        public string? PaymentIntentId { get; set; } 
        public string? ClientSecret { get; set; }    
        public decimal TotalAmount { get; set; }     
        public string Status { get; set; }           
    }
}
