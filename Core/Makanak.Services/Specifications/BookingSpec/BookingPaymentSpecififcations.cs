using Makanak.Domain.Models.BookingEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Services.Specifications.BookingSpec
{
    public class BookingPaymentSpecififcations : BaseSpecifications<Booking,int>
    {
        public BookingPaymentSpecififcations(string PaymentIntentId)
            :base(b=> b.PaymentIntentId == PaymentIntentId)
        {
            
        }
    }
}
