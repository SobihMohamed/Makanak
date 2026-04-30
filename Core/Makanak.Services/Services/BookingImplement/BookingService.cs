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
using Makanak.Shared.Dto_s.Admin;
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
            // 1. Get User & Property
            var user = await GetActiveUserAsync(tenantId);
            var property = await GetPropertyAsync(dto.PropertyId);

            // 2. Business Validations (Private Method)
            await ValidateBookingRulesAsync(dto, tenantId, property);

            // 3. Financial Calculations (Private Method)
            var totalDays = (dto.CheckOutDate.Date - dto.CheckInDate.Date).Days;
            var financials = CalculateFinancials(property.PricePerNight, totalDays);

            // 4. Create Entity & Map
            var booking = mapper.Map<Booking>(dto);
            booking.TenantId = tenantId;
            booking.OwnerId = property.OwnerId;
            booking.Property = property;
            booking.TotalDays = totalDays;
            booking.PricePerNight = property.PricePerNight;
            booking.TotalPrice = financials.TotalAmountToPay;
            booking.CommissionPaid = financials.Commission;
            booking.AmountToPayToOwner = financials.AmountToOwner;
            booking.Status = BookingStatus.PendingOwnerApproval;
            booking.CheckInQrCode = Guid.NewGuid().ToString();
            booking.IsQrScanned = false;

            // 5. Save & Notify
            var bookingRepo = unitOfWork.GetRepo<Booking, int>();
            bookingRepo.AddAsync(booking);

            var result = await unitOfWork.SaveChangesAsync();
            if (result <= 0) throw new Exception("Failed to create booking.");

            await notificationService.SendNotificationAsync(
                NotificationFactory.BookingRequest(booking.OwnerId, user.Name, property.Title, booking.Id)
            );

            return mapper.Map<TenantBookingDetailsDto>(booking);
        }
     
        #region Helper Methods

        private async Task<ApplicationUser> GetActiveUserAsync(string tenantId)
        {
            var user = await userManager.FindByIdAsync(tenantId);
            if (user == null || user.UserStatus != UserStatus.Active)
                throw new BadRequestException("Account not verified.");
            return user;
        }

        private async Task<Property> GetPropertyAsync(int propertyId)
        {
            var propertyRepo = unitOfWork.GetRepo<Property, int>();
            var specProperty = new PropertySpecifications(propertyId);
            var property = await propertyRepo.GetByIdWithSpecificationsAsync(specProperty);

            if (property == null) throw new PropertyNotFound(propertyId);
            return property;
        }

        private async Task ValidateBookingRulesAsync(CreateBookingDto dto, string tenantId, Property property)
        {
            if (dto.CheckInDate >= dto.CheckOutDate)
                throw new BadRequestException("Check-out date must be after check-in date.");

            if (dto.CheckInDate < DateTime.Today)
                throw new BadRequestException("Cannot book dates in the past.");

            if (property.PropertyStatus != PropertyStatus.Accepted)
                throw new BadRequestException("Property is not available for booking.");

            if (property.OwnerId == tenantId)
                throw new BadRequestException("Owners cannot book their own properties.");

            if (property.MaxGuests < dto.NumberOfGuests)
                throw new BadRequestException($"The property can accommodate a maximum of {property.MaxGuests} guests.");

            var isAvailable = await IsPropertyAvailableAsync(property.Id, dto.CheckInDate, dto.CheckOutDate);
            if (!isAvailable)
                throw new BadRequestException("Property is not available for the selected dates.");
        }

        private (decimal BaseAmount, decimal Commission, decimal AmountToOwner, decimal TotalAmountToPay) CalculateFinancials(decimal pricePerNight, int totalDays)
        {
            var baseAmount = totalDays * pricePerNight;
            var commissionPercentage = 0.10m;
            var commissionAmount = baseAmount * commissionPercentage;
            var amountToOwner = baseAmount;
            var totalAmountToPay = baseAmount + commissionAmount;

            return (baseAmount, commissionAmount, amountToOwner, totalAmountToPay);
        }

        #endregion
        public async Task<Pagination<BookingDto>> GetAllBookingsForAdminAsync(BookingSpecParams bookingParams)
        {
            var bookingRepo = unitOfWork.GetRepo<Booking, int>();

            // 1. (Count Specs)
            var countSpec = new AdminBookingPaginationSpecifications(bookingParams, isCount: true);
            var totalItems = await bookingRepo.CountAsync(countSpec);

            if (totalItems == 0)
                return new Pagination<BookingDto>(bookingParams.PageIndex, bookingParams.PageSize, 0, new List<BookingDto>());

            // 2. (Data Specs)
            var dataSpec = new AdminBookingPaginationSpecifications(bookingParams, isCount: false);
            var bookings = await bookingRepo.GetAllWithSpecificationAsync(dataSpec);

            // 3. Mapping & Return
            var data = mapper.Map<IReadOnlyList<BookingDto>>(bookings);

            return new Pagination<BookingDto>(bookingParams.PageIndex, bookingParams.PageSize, totalItems, data);
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
            var spec = new BookingSpecifications(bookingId);
            var bookingRepo = unitOfWork.GetRepo<Booking, int>();
            var booking = await bookingRepo.GetByIdWithSpecificationsAsync(spec);

            if (booking == null) throw new BookingNotFound(bookingId);

            if (userId != booking.TenantId && userId != booking.OwnerId && role != "Admin")
                throw new UnauthorizedAccessException("You do not have permission to cancel this booking.");

            var cancellableStatuses = new[] {
                BookingStatus.PendingOwnerApproval,
                BookingStatus.PendingPayment,
                BookingStatus.PaymentReceived
            };

            if (!cancellableStatuses.Contains(booking.Status))
                throw new BadRequestException("Only bookings that are not checked-in or completed can be canceled.");

            string cancelledBy = (userId == booking.TenantId) ? "Tenant" : (userId == booking.OwnerId ? "Owner" : "Admin");
            bool wasPaid = booking.Status == BookingStatus.PaymentReceived;
            decimal refundAmount = 0;

            // 💡 1. حساب الفلوس (بدون ما نكلم بيموب)
            if (wasPaid)
            {
                if (cancelledBy == "Owner" || cancelledBy == "Admin")
                    refundAmount = booking.CommissionPaid;
                else
                    refundAmount = CalculateRefundAmount(booking.CheckInDate, booking.CommissionPaid);
            }

            // 💡 2. تحديد الحالة بناءً على الفلوس
            if (refundAmount > 0)
            {
                // العميل ليه فلوس -> نخلي الحالة طلب استرداد عشان الأدمن يشوفها
                booking.Status = BookingStatus.RefundRequested;
                booking.RefundedAmount = refundAmount; // نسجل المبلغ المفروض يرجع
                booking.IsRefunded = false; // لسه مرجعش فعلياً
                booking.CancellationReason = $"Cancelled by {cancelledBy}. Refund of {refundAmount} EGP requested.";
            }
            else
            {
                // العميل ملوش فلوس -> نكنسل فوراً
                booking.Status = BookingStatus.Cancelled;
                booking.CancellationReason = $"Cancelled by {cancelledBy}. No refund applicable.";
            }

            bookingRepo.Update(booking);
            var result = await unitOfWork.SaveChangesAsync();

            // 💡 3. الإشعارات
            if (result > 0)
            {
                string targetId = (userId == booking.TenantId) ? booking.OwnerId : booking.TenantId;
                await notificationService.SendNotificationAsync(
                    NotificationFactory.BookingCancelled(targetId, cancelledBy, booking.Id)
                );

                if (refundAmount > 0)
                {
                    await notificationService.SendNotificationAsync(
                        NotificationFactory.RefundStatusNotification(booking.TenantId, false, booking.Id)
                    );

                    var adminIds = await GetAdminUserIdsAsync(); 

                    foreach (var adminId in adminIds)
                    {
                        await notificationService.SendNotificationAsync(
                            NotificationFactory.AdminManualRefundRequired(adminId, booking.Id, refundAmount)
                        );
                    }
                }
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
            var bookingRepo = unitOfWork.GetRepo<Booking, int>();
            var spec = new BookingSpecifications(bookingId);
            var booking = await bookingRepo.GetByIdWithSpecificationsAsync(spec);

            if (booking == null) throw new BookingNotFound(bookingId);

            if (booking.TenantId != UserId)
                throw new UnauthorizedAccessException("You do not have permission to pay for this booking.");

            if (booking.PaymentDeadline.HasValue && booking.PaymentDeadline.Value < DateTime.UtcNow)
            {
                booking.Status = BookingStatus.Cancelled;
                booking.CancellationReason = "Payment time expired (Auto-Cancelled).";

                bookingRepo.Update(booking);
                await unitOfWork.SaveChangesAsync();

                throw new BadRequestException("Time expired! You missed the payment window.");
            }

            if (booking.Status != BookingStatus.PendingPayment)
                throw new BadRequestException("You cannot pay for this booking until the owner approves it.");

            // 1. Prepare PaymentIntentInputDto
            var paymentIntentRequest = new PaymentIntentInputDto
            {
                BookingId = booking.Id,
                AmountToPayOnline = booking.CommissionPaid, // commission only, owner will get the rest
                TotalDays = booking.TotalDays,
                PropertyTitle = booking.Property?.Title ?? "Makanak Property",
                TenantFirstName = booking.Tenant?.Name?.Split(' ').FirstOrDefault() ?? "Guest",
                TenantLastName = booking.Tenant?.Name?.Split(' ').Skip(1).FirstOrDefault() ?? "User",
                TenantEmail = booking.Tenant?.Email ?? "dummy@email.com",
                TenantPhoneNumber = booking.Tenant?.PhoneNumber ?? "01000000000"
            };

            // 2. send to payment service to create PaymentIntent
            var paymentDto = await paymentService.CreatePaymentIntentAsync(paymentIntentRequest);

            // 3. save the PaymentIntentId and ClientSecret to the booking
            booking.PaymentIntentId = paymentDto.PaymentIntentId;
            booking.ClientSecret = paymentDto.ClientSecret;

            bookingRepo.Update(booking);
            await unitOfWork.SaveChangesAsync();

            // 4. return the paymentDto with bookingId and status
            paymentDto.BookingId = booking.Id;
            paymentDto.Status = booking.Status.ToString();
            return paymentDto;
        }

        public async Task<bool> UpdateBookingStatusByBookingIdAsync(int bookingId, BookingStatus newStatus, string? transactionId = null)
        {
            var spec = new BookingPaymentSpecififcations(bookingId);
            var bookingRepo = unitOfWork.GetRepo<Booking, int>();
            var booking = await bookingRepo.GetByIdWithSpecificationsAsync(spec);

            if (booking == null)
            {
                throw new BookingNotFound(bookingId);
            }

            booking.Status = newStatus;

            if (!string.IsNullOrWhiteSpace(transactionId))
                booking.TransactionId = transactionId;

            if (newStatus == BookingStatus.Completed)
                booking.IsQrScanned = true;

            bookingRepo.Update(booking);
            var result = await unitOfWork.SaveChangesAsync();

            if (result > 0 && newStatus == BookingStatus.PaymentReceived)
            {
                try
                {
                    var propertyTitle = booking.Property?.Title ?? "العقار";
                    await notificationService.SendNotificationAsync(
                        NotificationFactory.PaymentSuccess_ToTenant(booking.TenantId, propertyTitle, booking.Id)
                    );

                    var tenantName = booking.Tenant?.Name ?? "عميل";
                    await notificationService.SendNotificationAsync(
                        NotificationFactory.PaymentSuccess_ToOwner(booking.OwnerId, tenantName, booking.Id)
                    );
                }
                catch (Exception ex)
                {
                    throw new BadRequestException("Error sending payment success notifications.");
                }
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
        public async Task<AdminBookingDetailsDto> GetAdminBookingByIdAsync(int bookingId)
        {
            var spec = new AdminBookingDetailsSpecifications(bookingId);
            var bookingRepo = unitOfWork.GetRepo<Booking, int>();
            var booking = await bookingRepo.GetByIdWithSpecificationsAsync(spec);

            if (booking == null)
                throw new BookingNotFound(bookingId);

            var dto = mapper.Map<AdminBookingDetailsDto>(booking);

            return dto;
        }
        public async Task<OwnerBookingDetailsDto> GetOwnerBookingByIdAsync(int bookingId, string ownerId)
        {
            var spec = new BookingSpecifications(bookingId);
            var booking = await unitOfWork.GetRepo<Booking, int>().GetByIdWithSpecificationsAsync(spec);

            // 1. تصليح الـ Null
            if (booking == null) throw new BookingNotFound(bookingId);

            if (booking.Property.OwnerId != ownerId) throw new UnauthorizedAccessException("Not your property");

            // 2. المابينج مرة واحدة بس
            var dto = mapper.Map<OwnerBookingDetailsDto>(booking);

            // 3. تصليح شرط الإخفاء (&& بدل ||) + يفضل نضيف حالة "مؤكد" عشان يقدروا يتواصلوا
            if (booking.Status != BookingStatus.Completed &&
                booking.Status != BookingStatus.CheckedIn &&
                booking.Status != BookingStatus.PaymentReceived) // (Confirmed = بعد الدفع)
            {
                dto.TenantName = null;
                dto.TenantPhoneNumber = null;
                dto.TenantImage = null;
                dto.TenantIdentityImage = null;
            }

            // 4. نرجع الـ dto اللي عدلنا عليه
            return dto;
        }

        private decimal CalculateRefundAmount(DateTime checkInDate, decimal commissionPaid)
        {
            var timeUntilCheckIn = checkInDate.Date - DateTime.UtcNow.Date;
            if (timeUntilCheckIn.TotalDays >= 4)
                return commissionPaid;
            else if (timeUntilCheckIn.TotalHours >= 48)
                return commissionPaid * 0.5m;
            else
                return 0m;
        }
        private async Task<List<string>> GetAdminUserIdsAsync()
        {
            var admins = await userManager.GetUsersInRoleAsync("Admin");

            var adminIds = admins.Select(admin => admin.Id).ToList();

            return adminIds;
        }
    }
}
