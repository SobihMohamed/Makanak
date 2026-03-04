using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Shared.Dto_s.Dashboard
{
    public class BookingStatsDto
    {
        public int TotalBookings { get; set; }
        public int PendingBookings { get; set; }
        public int PaymentReceived { get; set; }
        public int CompletedBookings { get; set; }
        public int CancelledBookings { get; set; }
        public int CheckedIn { get; set; } // الناس اللي في الشقق حالياً (CheckedIn)
    }
}
