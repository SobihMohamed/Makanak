using Makanak.Domain.Models.BookingEntities;
using Makanak.Shared.EnumsHelper.Booking;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Services.Specifications.BookingSpec
{
    public class BookingOverlapSpecification : BaseSpecifications<Booking,int>
    {
        public BookingOverlapSpecification(int propertyId, DateTime newCheckIn, DateTime newCheckOut)
            : base(b =>
                b.PropertyId == propertyId &&
                (b.CheckInDate < newCheckOut && b.CheckOutDate > newCheckIn ) &&
                (
                    // الحالات دي كلها تقفل التاريخ 
                    b.Status == BookingStatus.PaymentReceived ||
                    b.Status == BookingStatus.CheckedIn ||
                    b.Status == BookingStatus.Completed ||

                    //الطلبات اللي المالك لسه مقيمهاش او موافقش او رفض عليها
                    b.Status == BookingStatus.PendingOwnerApproval ||

                    // deadline 
                    (b.Status == BookingStatus.PendingPayment && b.PaymentDeadline > DateTime.UtcNow)
                )
            )
        {
            
        }
    }
}
