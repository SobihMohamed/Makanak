using Makanak.Shared.Common;
using Makanak.Shared.Common.Params.Booking_Params;
using Makanak.Shared.Dto_s.Admin;
using Makanak.Shared.Dto_s.Booking;
using Makanak.Shared.Dto_s.Payment;
using Makanak.Shared.EnumsHelper.Booking;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Abstraction.IServices.Booking
{
    public interface IBookingService
    {
        Task<TenantBookingDetailsDto> CreateBookingAsync(CreateBookingDto dto, string tenantId);

        Task<bool> IsPropertyAvailableAsync(int propertyId, DateTime checkIn, DateTime checkOut);

        Task<Pagination<BookingDto>> GetTenantBookingsAsync(string tenantId, BookingSpecParams bookingParams);
        Task<Pagination<BookingDto>> GetAllBookingsForAdminAsync(BookingSpecParams bookingParams);

        Task<Pagination<BookingDto>> GetOwnerBookingsAsync(string ownerId, BookingSpecParams bookingParams);
        Task<TenantBookingDetailsDto> GetTenantBookingByIdAsync(int bookingId, string tenantId);
        Task<AdminBookingDetailsDto> GetAdminBookingByIdAsync(int bookingId);
        Task<OwnerBookingDetailsDto> GetOwnerBookingByIdAsync(int bookingId, string ownerId);
        Task<bool> CancelBookingAsync(int bookingId, string userId, string role);

        Task<ScanQrResponseDto> ScanQrCodeAsync(string qrCode, string ownerId);

        Task<bool> UpdateBookingStatusAsync(int bookingId, BookingStatus newStatus, string userId, string role);

        Task<BookingPaymentDto> CreateBookingPaymentAsync(int bookingId, string UserId);

        Task<bool> UpdateBookingStatusByBookingIdAsync(int bookingId, BookingStatus newStatus, string? transactionId = null);

        Task ProcessAutomatedStatusesAsync();
    }
}
