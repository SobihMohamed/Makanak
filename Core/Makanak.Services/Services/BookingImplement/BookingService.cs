using AutoMapper;
using Makanak.Abstraction.IServices.Booking;
using Makanak.Domain.Contracts.UOW;
using Makanak.Domain.EnumsHelper.User;
using Makanak.Domain.Exceptions;
using Makanak.Domain.Exceptions.NotFound;
using Makanak.Domain.Models.BookingEntities;
using Makanak.Domain.Models.Identity;
using Makanak.Domain.Models.PropertyEntities;
using Makanak.Services.Specifications.BookingSpec;
using Makanak.Services.Specifications.Property_Spec;
using Makanak.Shared.Dto_s.Booking;
using Makanak.Shared.EnumsHelper.Booking;
using Makanak.Shared.EnumsHelper.Property;
using Microsoft.AspNetCore.Identity;



namespace Makanak.Services.Services.BookingImplement
{
    public class BookingService(IUnitOfWork unitOfWork, IMapper mapper , UserManager<ApplicationUser> userManager) : IBookingService
    {
        public async Task<BookingDetailDto> CreateBookingAsync(CreateBookingDto dto, string tenantId)
        {
            #region check user status
            var user = await userManager.FindByIdAsync(tenantId);

            if (user == null || user.UserStatus != UserStatus.Active)
                throw new BadRequestException("Account not verified.");

            #endregion
            
            if (dto.CheckInDate >= dto.CheckOutDate)
                throw new BadRequestException("Check-out date must be after check-in date.");

            if (dto.CheckInDate < DateTime.Today)
                throw new BadRequestException("Cannot book dates in the past.");

            // validation of the property availability for the given dates
            var propertyId = dto.PropertyId;

            // check exist 
            var propertyRepo = unitOfWork.GetRepo<Property, int>();

            // generate specification
            var specProperty = new PropertySpecifications(propertyId);

            // get property 
            var property = await propertyRepo.GetByIdWithSpecificationsAsync(specProperty);

            #region Validations           
            if (property == null)
                throw new PropertyNotFound(propertyId);

            if(property.PropertyStatus != PropertyStatus.Accepted)
                throw new BadRequestException("Property is not available for booking.");

            if(property.OwnerId == tenantId)
                throw new BadRequestException("Owners cannot book their own properties.");

            if(property.MaxGuests < dto.NumberOfGuests)
                throw new BadRequestException($"The property can accommodate a maximum of {property.MaxGuests} guests.");

            var IsPropertyAvailable = await IsPropertyAvailableAsync(propertyId, dto.CheckInDate, dto.CheckOutDate);
            
            if(!IsPropertyAvailable)
                throw new BadRequestException("Property is not available for the selected dates.");
            #endregion

            // financial calculations
            var totalDays = (dto.CheckOutDate.Date - dto.CheckInDate.Date).Days;
            var totalAmount = totalDays * property.PricePerNight;

            var commissionPercentage = 0.10m; 
            var commissionAmount = totalAmount * commissionPercentage;
            var amountToOwner = totalAmount - commissionAmount;

            // create booking entity
            var booking = mapper.Map<Booking>(dto);

            booking.TenantId = tenantId;
            booking.OwnerId = property.OwnerId;
            booking.Property = property;

            booking.TotalDays = totalDays;
            booking.TotalPrice = totalAmount;
            booking.PricePerNight = property.PricePerNight;
            booking.CommissionPaid = commissionAmount;
            booking.AmountToPayToOwner = amountToOwner;

            booking.Status = BookingStatus.PendingPayment;
            booking.CheckInQrCode = Guid.NewGuid().ToString();
            booking.IsQrScanned = false;

            // save to database
            var bookingRepo = unitOfWork.GetRepo<Booking, int>();
            bookingRepo.AddAsync(booking);

            var result = await unitOfWork.SaveChangesAsync();

            if (result <= 0)
                throw new Exception("Failed to create booking.");

            return mapper.Map<BookingDetailDto>(booking);
        }

        public async Task<BookingDetailDto> GetBookingByIdAsync(int bookingId , string UserId ,string role)
        {
            // generate specification
            var spec = new BookingSpecifications(bookingId);

            // get booking repo
            var bookingRepo = unitOfWork.GetRepo<Booking, int>();

            // get booking with specification
            var booking = await bookingRepo.GetByIdWithSpecificationsAsync(spec);
            
            if (booking == null)
                throw new BookingNotFound(bookingId);

            // authorization check
            if (booking.TenantId != UserId && booking.OwnerId != UserId && role != "Admin")
                throw new UnauthorizedAccessException("You do not have permission to view this booking.");

            return mapper.Map<BookingDetailDto>(booking);

        }

        public async Task<IReadOnlyList<BookingDto>> GetOwnerBookingsAsync(string ownerId)
        {
            // generate specification 
            var spec = new BookingSpecifications(ownerId);

            // get booking repo
            var bookingRepo = unitOfWork.GetRepo<Booking, int>();

            // get bookings with specification
            var bookings = await bookingRepo.GetAllWithSpecificationAsync(spec);

            // map to dto
            return mapper.Map<IReadOnlyList<BookingDto>>(bookings);
        }

        public async Task<IReadOnlyList<BookingDto>> GetTenantBookingsAsync(string tenantId)
        {
            var spec = new BookingSpecifications(tenantId, isTenant: true);

            var bookingRepo = unitOfWork.GetRepo<Booking, int>();

            var bookings = await bookingRepo.GetAllWithSpecificationAsync(spec);

            return mapper.Map<IReadOnlyList<BookingDto>>(bookings);
        }

        public async Task<bool> IsPropertyAvailableAsync(int propertyId, DateTime checkIn, DateTime checkOut)
        {
            var spec = new BookingOverlapSpecification(propertyId, checkIn, checkOut);

            var bookingRepo = unitOfWork.GetRepo<Booking, int>();

            var count = await bookingRepo.CountAsync(spec);

            return count == 0;
        }

        public async Task<bool> ScanQrCodeAsync(string qrCode, string ownerId)
        {
            // generate specification
            var spec = new ScanningQrBookingSpecification(qrCode);

            // get booking repo
            var bookingRepo = unitOfWork.GetRepo<Booking, int>();

            // get booking with specification
            var booking = await bookingRepo.GetByIdWithSpecificationsAsync(spec);

            if (booking == null)
                throw new BookingNotFoundByQrCode(qrCode);

            // authorization check
            if(booking.OwnerId != ownerId)
                throw new UnauthorizedAccessException("You do not have permission to scan this QR code.");

            // isQrScanned ? 
            if (booking.IsQrScanned)
                throw new BadRequestException("This QR code has already been scanned.");

            // time check 
            if (DateTime.Today < booking.CheckInDate.Date || DateTime.Now.Date > booking.CheckOutDate.Date)
                throw new BadRequestException("QR code can only be scanned on the check-in or check-out date.");

            booking.IsQrScanned = true;
            booking.Status = BookingStatus.CheckedIn;

            bookingRepo.Update(booking);
            var result = await unitOfWork.SaveChangesAsync();
            if (result <= 0)
                throw new Exception("Failed to scan QR code.");
            return result > 0;
        }
  
        public async Task<bool> CancelBookingAsync(int bookingId, string userId, string role)
        {
            // generate specification
            var spec = new BookingSpecifications(bookingId);

            // get booking repo
            var bookingRepo = unitOfWork.GetRepo<Booking, int>();

            // get booking with specification
            var booking = await bookingRepo.GetByIdWithSpecificationsAsync(spec);
            
            if (booking == null)
                throw new BookingNotFound(bookingId);

            // authorization check 
            if(userId != booking.TenantId && userId != booking.OwnerId && role != "Admin")
                throw new UnauthorizedAccessException("You do not have permission to cancel this booking.");

            // business rule: only pending or confirmed bookings can be canceled
            if(booking.Status != BookingStatus.PendingPayment && booking.Status != BookingStatus.Confirmed)
                throw new BadRequestException("Only bookings with status 'Pending Payment' or 'Confirmed' can be canceled.");

            // so we need to send mony back to tenant if the booking is completed
            if (booking.Status == BookingStatus.Confirmed)
            {
                // initiate refund process here => stripe 
                booking.IsRefunded = true;
            }
            booking.Status = BookingStatus.Cancelled;
            booking.CancellationReason = "Cancelled by " + (userId == booking.TenantId ? "Tenant" : "Owner");

            bookingRepo.Update(booking);
            var result = await unitOfWork.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> UpdateBookingStatusAsync(int bookingId, BookingStatus newStatus)
        {
            var spec = new BookingSpecifications(bookingId);
            var bookingRepo = unitOfWork.GetRepo<Booking, int>();
            var booking = await bookingRepo.GetByIdWithSpecificationsAsync(spec);

            if (booking == null)
                throw new BookingNotFound(bookingId); 

            booking.Status = newStatus;

            if (newStatus == BookingStatus.Completed)
            {
                booking.IsQrScanned = true;
            }

            bookingRepo.Update(booking);
            var result = await unitOfWork.SaveChangesAsync();
            return result > 0;
        }
    }
}
