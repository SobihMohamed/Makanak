using Makanak.Domain.Models.BookingEntities;
using Makanak.Shared.EnumsHelper.Booking;


namespace Makanak.Services.Specifications.BookingSpec
{
    public class BookingSpecifications : BaseSpecifications<Booking,int>
    {
        public BookingSpecifications(int propertyId , DateTime checkIn , DateTime checkOut)
            :base(b=> b.PropertyId == propertyId && b.Status != BookingStatus.Cancelled &&
            (checkIn >= b.CheckInDate && checkIn < b.CheckOutDate))
        {
            
        }
        public BookingSpecifications(int bookingId)
            :base(b=> b.Id == bookingId)
        {
            AddInclude(b => b.Property);
            AddInclude(b => b.Tenant);
            AddInclude(b => b.Property.PropertyImages);
            AddInclude(b => b.Owner);
        }

        public BookingSpecifications(string OwnerId)
            : base(b=> b.OwnerId == OwnerId)
        {
            AddInclude(b => b.Property);
            AddInclude(b => b.Tenant);
            AddOrderByDesc(b=>b.CheckInDate);
        }
        public BookingSpecifications(string TenantId , bool isTenant)
            : base(b=> b.TenantId == TenantId)
        {
            AddInclude(b => b.Property);
            AddInclude(b => b.Tenant);
            AddInclude(b => b.Owner);
            AddOrderByDesc(b=>b.CheckInDate);
        }
    }
}