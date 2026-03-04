using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Domain.Exceptions.NotFound
{
    public class BookingNotFoundByPaymentIntentId(string paymnetIntentId) 
        : NotFoundException_Base($"Booking With Payment Intent {paymnetIntentId} Not Found")
    {
    }
}
