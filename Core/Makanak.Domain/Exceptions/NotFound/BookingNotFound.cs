using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Domain.Exceptions.NotFound
{
    public class BookingNotFound(int bookingId) : NotFoundException_Base($"Booking with Id : {bookingId} Not Found")
    {
    }
}
