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
        Task<BookingDetailDto> CreateBookingAsync(CreateBookingDto dto, string tenantId);

        Task<bool> IsPropertyAvailableAsync(int propertyId, DateTime checkIn, DateTime checkOut);

        Task<IReadOnlyList<BookingDto>> GetTenantBookingsAsync(string tenantId);

        Task<IReadOnlyList<BookingDto>> GetOwnerBookingsAsync(string ownerId);

        Task<BookingDetailDto> GetBookingByIdAsync(int bookingId, string UserId, string role);

        Task<bool> CancelBookingAsync(int bookingId, string userId, string role);

        Task<ScanQrResponseDto> ScanQrCodeAsync(string qrCode, string ownerId);

        Task<bool> UpdateBookingStatusAsync(int bookingId, BookingStatus newStatus, string userId, string role);

        Task<BookingPaymentDto> CreateBookingPaymentAsync(int bookingId, string UserId);

        Task<bool> UpdateBookingStatusByIntentIdAsync(string paymentIntentId, BookingStatus newStatus);

        Task ProcessAutomatedStatusesAsync();
    }
}
