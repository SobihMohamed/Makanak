using Makanak.Shared.EnumsHelper.Booking;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Shared.Dto_s.Booking
{
    public class UpdateBookingStatusDto
    {
        public int BookingId { get; set; }
        public BookingStatus Status { get; set; } 
    }
}
