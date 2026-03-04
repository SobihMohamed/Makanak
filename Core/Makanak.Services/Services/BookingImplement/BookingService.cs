using AutoMapper;
using Makanak.Abstraction.IServices.Booking;
using Makanak.Abstraction.IServices.NotificationService;
using Makanak.Abstraction.IServices.PaymentService;
using Makanak.Domain.Contracts.UOW;
using Makanak.Domain.EnumsHelper.User;
using Makanak.Domain.Exceptions;
using Makanak.Domain.Exceptions.NotFound;
using Makanak.Domain.Models.BookingEntities;
using Makanak.Domain.Models.Identity;
using Makanak.Domain.Models.PropertyEntities;
using Makanak.Services.Specifications.AutomatedNotificationSpec;
using Makanak.Services.Specifications.BookingSpec;
using Makanak.Services.Specifications.Property_Spec;
using Makanak.Shared.Common;
using Makanak.Shared.Common.Params.Booking_Params;
using Makanak.Shared.Dto_s.Booking;
using Makanak.Shared.Dto_s.Payment;
using Makanak.Shared.EnumsHelper.Booking;
using Makanak.Shared.EnumsHelper.Property;
using Makanak.Shared.HelpersFactory;
using Microsoft.AspNetCore.Identity;

namespace Makanak.Services.Services.BookingImplement
{
    public class BookingService(IPaymentService paymentService, IUnitOfWork unitOfWork, IMapper mapper, 
        UserManager<ApplicationUser> userManager, INotificationService notificationService)
        : IBookingService
    {

        public async Task<TenantBookingDetailsDto> CreateBookingAsync(CreateBookingDto dto, string tenantId)
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

            booking.Status = BookingStatus.PendingOwnerApproval;
            booking.CheckInQrCode = Guid.NewGuid().ToString();
            booking.IsQrScanned = false;

            // save to database
            var bookingRepo = unitOfWork.GetRepo<Booking, int>();
            bookingRepo.AddAsync(booking);

            var result = await unitOfWork.SaveChangesAsync();

            await notificationService.SendNotificationAsync(
                NotificationFactory.BookingRequest(booking.OwnerId, user.Name, property.Title ,booking.Id)
            );

            if (result <= 0)
                throw new Exception("Failed to create booking.");

            return mapper.Map<TenantBookingDetailsDto>(booking);
        }

        public async Task<Pagination<BookingDto>> GetOwnerBookingsAsync(string ownerId, BookingSpecParams bookingParams)
        {
            var bookingRepo = unitOfWork.GetRepo<Booking, int>();

            // (Count Specs)
            // isTenant = false (لأنه مالك), isCount = true
            var countSpec = new BookingPaginationSpecifications(ownerId, bookingParams, isTenant: false, isCount: true);
            var totalItems = await bookingRepo.CountAsync(countSpec);

            if (totalItems == 0)
                return new Pagination<BookingDto>(bookingParams.PageIndex, bookingParams.PageSize, 0, new List<BookingDto>());

            // (Data Specs)
            // isTenant = false, isCount = false
            var dataSpec = new BookingPaginationSpecifications(ownerId, bookingParams, isTenant: false, isCount: false);
            var bookings = await bookingRepo.GetAllWithSpecificationAsync(dataSpec);

            // Mapping & Return
            var data = mapper.Map<IReadOnlyList<BookingDto>>(bookings);

            return new Pagination<BookingDto>(bookingParams.PageIndex, bookingParams.PageSize, totalItems, data);
        }

        public async Task<Pagination<BookingDto>> GetTenantBookingsAsync(string tenantId, BookingSpecParams bookingParams)
        {
            var bookingRepo = unitOfWork.GetRepo<Booking, int>();

            // Count (isTenant = true)
            var countSpec = new BookingPaginationSpecifications(tenantId, bookingParams, isTenant: true, isCount: true);
            var totalItems = await bookingRepo.CountAsync(countSpec);

            if (totalItems == 0)
                return new Pagination<BookingDto>(bookingParams.PageIndex, bookingParams.PageSize, 0, new List<BookingDto>());

            // 2. Data (isTenant = true)
            var dataSpec = new BookingPaginationSpecifications(tenantId, bookingParams, isTenant: true, isCount: false);
            var bookings = await bookingRepo.GetAllWithSpecificationAsync(dataSpec);

            // 3. Return
            var data = mapper.Map<IReadOnlyList<BookingDto>>(bookings);

            return new Pagination<BookingDto>(bookingParams.PageIndex, bookingParams.PageSize, totalItems, data);
        }

        public async Task<bool> IsPropertyAvailableAsync(int propertyId, DateTime checkIn, DateTime checkOut)
        {
            var spec = new BookingOverlapSpecification(propertyId, checkIn, checkOut);

            var bookingRepo = unitOfWork.GetRepo<Booking, int>();

            var count = await bookingRepo.CountAsync(spec);

            return count == 0;
        }

        public async Task<ScanQrResponseDto> ScanQrCodeAsync(string qrCode, string ownerId)
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
            //if (DateTime.Today < booking.CheckInDate.Date || DateTime.Now.Date > booking.CheckOutDate.Date)
            //    throw new BadRequestException("QR code can only be scanned on the check-in or check-out date.");

            booking.IsQrScanned = true;
            booking.Status = BookingStatus.CheckedIn;

            bookingRepo.Update(booking);
            await unitOfWork.SaveChangesAsync();

            return mapper.Map<ScanQrResponseDto>(booking);
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

            //   أي حالة قبل السكن ينفع تتلغي (طلب موافقة، مستني دفع، أو اندفع فعلاً
            var cancellableStatuses = new[] {
                BookingStatus.PendingOwnerApproval,
                BookingStatus.PendingPayment,
                BookingStatus.PaymentReceived 
            };

            if (!cancellableStatuses.Contains(booking.Status))
                throw new BadRequestException("Only bookings that are not checked-in or completed can be canceled.");

            //   لو الحالة PaymentReceived، لازم نرجع الفلوس (Refund)
            if (booking.Status == BookingStatus.PaymentReceived)
            {
                // هنا هتحط كود الـ Stripe Refund لاحقاً
                booking.IsRefunded = true;
            }

            booking.Status = BookingStatus.Cancelled;

            bookingRepo.Update(booking);
            var result = await unitOfWork.SaveChangesAsync();
            if (result > 0)
            {
                string targetId = (userId == booking.TenantId) ? booking.OwnerId : booking.TenantId;
                string cancelledBy = (userId == booking.TenantId) ? "Tenant" : "Owner";

                if (role == "Admin") cancelledBy = "Admin";
                booking.CancellationReason = cancelledBy;


                await notificationService.SendNotificationAsync(
                    NotificationFactory.BookingCancelled(targetId, cancelledBy, booking.Id)
                );
            }
            return result > 0;
        }

        public async Task<bool> UpdateBookingStatusAsync(int bookingId, BookingStatus newStatus, string userId, string role)
        {
            var spec = new BookingSpecifications(bookingId);
            var bookingRepo = unitOfWork.GetRepo<Booking, int>();
            var booking = await bookingRepo.GetByIdWithSpecificationsAsync(spec);

            if (booking == null) throw new BookingNotFound(bookingId);

            if (booking.Property.OwnerId != userId && role != "Admin")
                throw new UnauthorizedAccessException("You don't have permission to update this booking.");

            if (newStatus == BookingStatus.PendingPayment && booking.Status != BookingStatus.PendingOwnerApproval)
                throw new BadRequestException("You can only approve bookings that are pending your approval.");
            
            
            if (newStatus == BookingStatus.PendingPayment)
            {
                booking.PaymentDeadline = DateTime.UtcNow.AddMinutes(30);
            }
            booking.Status = newStatus;

            if (newStatus == BookingStatus.Completed)
            {
                booking.IsQrScanned = true;
            }

            bookingRepo.Update(booking);
            var result = await unitOfWork.SaveChangesAsync();
            if (result > 0)
            {
                if (newStatus == BookingStatus.PendingPayment)
                {
                    // المالك وافق -> ابعت للمستأجر عشان يدفع
                    await notificationService.SendNotificationAsync(
                        NotificationFactory.BookingApprovedForPayment(booking.TenantId, booking.Property.Title, booking.Id)
                    );
                }
                else if (newStatus == BookingStatus.RejectedByOwner)
                {
                    // المالك رفض -> ابعت للمستأجر
                    await notificationService.SendNotificationAsync(
                        NotificationFactory.BookingRejected(booking.TenantId, booking.Property.Title, booking.Id)
                    );
                }
            }
            return result > 0;
        }

        public async Task<BookingPaymentDto> CreateBookingPaymentAsync(int bookingId, string UserId)
        {
            // get booking repo 
            var bookingRepo = unitOfWork.GetRepo<Booking, int>();

            //get booking 
            var booking = await bookingRepo.GetByIdAsync(bookingId);

            if (booking == null) throw new BookingNotFound(bookingId);

            // check user 
            if(booking.TenantId != UserId)
                throw new UnauthorizedAccessException("You do not have permission to pay for this booking.");

            if (booking.PaymentDeadline.HasValue && booking.PaymentDeadline.Value < DateTime.UtcNow)
            {
                booking.Status = BookingStatus.Cancelled;
                booking.CancellationReason = "Payment time expired (Auto-Cancelled).";
                
                bookingRepo.Update(booking);
                await unitOfWork.SaveChangesAsync();

                throw new BadRequestException("Time expired! You missed the payment window.");
            }

            // comission only paied online
            var amountToPay = booking.CommissionPaid;

            // chcek on booking status 
            if (booking.Status != BookingStatus.PendingPayment)
                throw new BadRequestException("You cannot pay for this booking until the owner approves it.");

            // call payment service to send intent id old exist
            var paymentDto = await paymentService.CreateOrUpdatePaymentIntent(booking.PaymentIntentId!, amountToPay);
            
            paymentDto.BookingId = booking.Id;       
            paymentDto.Status = booking.Status.ToString(); 
            
            booking.PaymentIntentId = paymentDto.PaymentIntentId;
            booking.ClientSecret = paymentDto.ClientSecret;

            bookingRepo.Update(booking);
            await unitOfWork.SaveChangesAsync();

            return paymentDto;
        }

        public async Task<bool> UpdateBookingStatusByIntentIdAsync(string paymentIntentId, BookingStatus newStatus)
        {
            // generate specification
            var spec = new BookingPaymentSpecififcations(paymentIntentId);

            // get repo 
            var bookingRepo = unitOfWork.GetRepo<Booking, int>();

            // get booking with specification
            var booking = await bookingRepo.GetByIdWithSpecificationsAsync(spec);
        
            if (booking == null)
                throw new BookingNotFoundByPaymentIntentId(paymentIntentId);

            booking.Status = newStatus;

            // if new status is completed , set IsQrScanned to true
            if (newStatus == BookingStatus.Completed)
                booking.IsQrScanned = true;

            bookingRepo.Update(booking);
            var result = await unitOfWork.SaveChangesAsync();

            if (result > 0 && newStatus == BookingStatus.PaymentReceived)
            {
                // 1. إشعار للمستأجر: مبروك، خد العنوان والتفاصيل
                await notificationService.SendNotificationAsync(
                    NotificationFactory.PaymentSuccess_ToTenant(booking.TenantId, booking.Property.Title, booking.Id)
                );

                // 2. إشعار للمالك: فيه فلوس وحجز اتأكد
                // (تأكد إن الـ Spec محملة بيانات الـ TenantName)
                await notificationService.SendNotificationAsync(
                    NotificationFactory.PaymentSuccess_ToOwner(booking.OwnerId, booking.Tenant.Name, booking.Id)
                );
            }

            return result > 0;
        }

        public async Task ProcessAutomatedStatusesAsync()
        {
            var bookingRepo = unitOfWork.GetRepo<Booking, int>();

            // (Reminders)

            // A. Payment Warning
            var paymentWarningSpec = new PaymentWarningSpecification();
            var bookingsToWarn = await bookingRepo.GetAllWithSpecificationAsync(paymentWarningSpec);

            foreach (var booking in bookingsToWarn)
            {
                try
                {
                    var minutesLeft = (int)(booking.PaymentDeadline.Value - DateTime.UtcNow).TotalMinutes;
                    if (minutesLeft <= 0) minutesLeft = 1;

                    await notificationService.SendNotificationAsync(
                        NotificationFactory.PaymentDeadlineWarning(booking.TenantId, booking.Id, minutesLeft)
                    );

                    booking.IsPaymentWarningSent = true;
                    bookingRepo.Update(booking);
                }
                catch (Exception) { continue; }
            }

            // B. Check-In Reminder
            var checkInSpec = new UpcomingCheckInSpecification(); 
            var bookingsToRemind = await bookingRepo.GetAllWithSpecificationAsync(checkInSpec);

            foreach (var booking in bookingsToRemind)
            {
                try
                {
                    await notificationService.SendNotificationAsync(
                        NotificationFactory.CheckInReminder(booking.TenantId, booking.Property.Title, booking.Id)
                    );

                    booking.IsCheckInReminderSent = true;
                    bookingRepo.Update(booking);
                }
                catch (Exception) { continue; }
            }

            // Save Reminders Changes
            if (bookingsToWarn.Any() || bookingsToRemind.Any())
            {
                await unitOfWork.SaveChangesAsync();
            }

            //  (Actions) - Complete & Cancel

            var completedSpec = new ExpiredBookingSpecifications();
            var bookingsToComplete = await bookingRepo.GetAllWithSpecificationAsync(completedSpec);
            if (bookingsToComplete.Any())
            {
                foreach (var booking in bookingsToComplete)
                {
                    try
                    {
                        booking.Status = BookingStatus.Completed;
                        bookingRepo.Update(booking);
                        await notificationService.SendNotificationAsync(
                            NotificationFactory.BookingCompleted(booking.TenantId, booking.Id)
                        );
                    }
                    catch (Exception) { continue; }
                }
            }

            var pendingSpec = new PendingPaymentExpiredSpecifications();
            var bookingsToCancel = await bookingRepo.GetAllWithSpecificationAsync(pendingSpec);
            if (bookingsToCancel.Any())
            {
                foreach (var booking in bookingsToCancel)
                {
                    try
                    {
                        booking.Status = BookingStatus.Cancelled;
                        booking.CancellationReason = "Auto-Cancelled: Payment deadline expired.";
                        bookingRepo.Update(booking);
                        await notificationService.SendNotificationAsync(
                            NotificationFactory.BookingExpired(booking.TenantId, booking.Id)
                        );
                    }
                    catch (Exception) { continue; }
                }
            }

            if (bookingsToComplete.Any() || bookingsToCancel.Any())
            {
                await unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<TenantBookingDetailsDto> GetTenantBookingByIdAsync(int bookingId, string tenantId)
        {
            var spec = new BookingSpecifications(bookingId);
            var booking = await unitOfWork.GetRepo<Booking, int>().GetByIdWithSpecificationsAsync(spec);

            if (booking == null) throw new BookingNotFound(booking!.Id);
            if (booking.TenantId != tenantId) throw new UnauthorizedAccessException("Not your booking");

            var mappedData = mapper.Map<TenantBookingDetailsDto>(booking);

            // لو لسه مدفعش، نخفي الداتا الحساسة
            if (booking.Status == BookingStatus.PendingPayment || booking.Status == BookingStatus.PendingOwnerApproval)
            {
                mappedData.CheckInInstructions = "سيتم إظهار التعليمات بعد إتمام الدفع";
                mappedData.ExactLocationUrl = null;
                mappedData.OwnerPhoneNumber = null;
                mappedData.CheckInQrCode = null;
            }

            return mappedData;
        }

        public async Task<OwnerBookingDetailsDto> GetOwnerBookingByIdAsync(int bookingId, string ownerId)
        {
            var spec = new BookingSpecifications(bookingId);
            var booking = await unitOfWork.GetRepo<Booking, int>().GetByIdWithSpecificationsAsync(spec);

            if (booking == null) throw new BookingNotFound(booking!.Id);
            if (booking.Property.OwnerId != ownerId) throw new UnauthorizedAccessException("Not your property");

            return mapper.Map<OwnerBookingDetailsDto>(booking);
        }
    }
}
