using Makanak.Domain.Models.BookingEntities;
using Makanak.Shared.EnumsHelper.Booking;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Services.Specifications.AutomatedNotificationSpec
{
    public class UpcomingCheckInSpecification : BaseSpecifications<Booking,int>
    {
        public UpcomingCheckInSpecification()
        : base(b =>
            b.Status == BookingStatus.PaymentReceived &&
            b.CheckInDate.Date == DateTime.UtcNow.Date.AddDays(1) &&
            !b.IsCheckInReminderSent
        )
            {
                AddInclude(b => b.Property);
            }
    }
}
