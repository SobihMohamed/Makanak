using Makanak.Shared.EnumsHelper.Booking;

namespace Makanak.Shared.Common.Params.Booking_Params
{
    public class BookingSpecParams : QueryParams
    {
        public BookingStatus? Status { get; set; }
    }
}
