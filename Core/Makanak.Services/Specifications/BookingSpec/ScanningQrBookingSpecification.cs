using Makanak.Domain.Models.BookingEntities;
using Makanak.Shared.EnumsHelper.Booking;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Services.Specifications.BookingSpec
{
    public class ScanningQrBookingSpecification : BaseSpecifications<Booking, int>
    {
        public ScanningQrBookingSpecification(string QrCode)
            : base(b =>
                b.CheckInQrCode == QrCode &&
                b.Status == BookingStatus.Confirmed
            )
        {
            
        }
    }
}
