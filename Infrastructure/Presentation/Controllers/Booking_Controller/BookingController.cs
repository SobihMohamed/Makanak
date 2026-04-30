using Makanak.Abstraction.IServices.Manager;
using Makanak.Shared.Common;
using Makanak.Shared.Common.Params.Booking_Params;
using Makanak.Shared.Dto_s.Admin;
using Makanak.Shared.Dto_s.Booking;
using Makanak.Shared.Dto_s.Payment;
using Makanak.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Makanak.Presentation.Controllers.Booking_Controller
{
    [Authorize]
    public class BookingController(IServiceManager serviceManager) : AppBaseController
    {

        [Authorize(Roles = "Tenant")]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<BookingDto>>> CreateBooking(CreateBookingDto dto)
        {
            var tenantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(tenantId)) return UnauthorizedError("User ID not found in token");

            var booking = await serviceManager.BookingService.CreateBookingAsync(dto, tenantId);

            if (booking == null) return BadRequestError("Problem creating booking");

            return Created(booking, "Booking created successfully");
        }

        [Authorize(Roles = "Tenant")]
        [HttpGet("tenant/{id}")]
        public async Task<ActionResult<ApiResponse<TenantBookingDetailsDto>>> GetBookingForTenant(int id)
        {
            var tenantId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await serviceManager.BookingService.GetTenantBookingByIdAsync(id, tenantId!);

            return Success(result);
        }

        [Authorize(Roles = "Owner")]
        [HttpGet("owner/{id}")]
        public async Task<ActionResult<ApiResponse<OwnerBookingDetailsDto>>> GetBookingForOwner(int id)
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await serviceManager.BookingService.GetOwnerBookingByIdAsync(id, ownerId!);

            return Success(result);
        }

        [Authorize(Roles = "Tenant")]
        [HttpGet("my-bookings")]
        public async Task<ActionResult<ApiResponse<Pagination<BookingDto>>>> GetMyBookingsAsTenant([FromQuery] BookingSpecParams bookingParams)
        {
            var tenantId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var bookings = await serviceManager.BookingService.GetTenantBookingsAsync(tenantId!, bookingParams);

            return Success(bookings);
        }

        [Authorize(Roles = "Owner")]
        [HttpGet("incoming-bookings")]
        public async Task<ActionResult<ApiResponse<Pagination<BookingDto>>>> GetIncomingBookingsAsOwner([FromQuery] BookingSpecParams bookingParams)
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var bookings = await serviceManager.BookingService.GetOwnerBookingsAsync(ownerId!, bookingParams);

            return Success(bookings);
        }

        [HttpPost("{bookingId}/cancel")]
        [Authorize] 
        public async Task<ActionResult> CancelBooking(int bookingId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role) ?? "User";

            if (string.IsNullOrEmpty(userId))
                return UnauthorizedError("User ID not found in token.");

            var result = await serviceManager.BookingService.CancelBookingAsync(bookingId, userId, role);

            if (!result)
                return BadRequestError("Failed to cancel the booking.");

            return Success("Booking cancelled successfully.");
        }
        [Authorize(Roles = "Owner")]
        [HttpPost("scan-qr")]
        public async Task<ActionResult<ApiResponse<BookingDto>>> ScanQrCode([FromBody] ScanQrRequestDto scanQrRequestDto)
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await serviceManager.BookingService.ScanQrCodeAsync(scanQrRequestDto.QrCode, ownerId!);


            return Success(result, "Check-in completed successfully! You can hand over the keys now.");
        }

        [Authorize(Roles = "Owner, Admin")]
        [HttpPatch("{id}/status")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateStatus(int id, [FromBody] UpdateBookingStatusDto updateBookingStatusDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

            var success = await serviceManager.BookingService.UpdateBookingStatusAsync(id, updateBookingStatusDto.Status, userId!, role!);

            if (!success) return BadRequestError("Update failed.");

            return Success($"Booking status updated to {updateBookingStatusDto.Status} successfully.");
        }

        [Authorize(Roles = "Tenant")]
        [HttpPost("{bookingId}/pay")] 
        public async Task<ActionResult<ApiResponse<BookingPaymentDto>>> InitiatePayment(int bookingId)
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await serviceManager.BookingService.CreateBookingPaymentAsync(bookingId, UserId!);

            if (result == null)
                return BadRequestError("Problem initiating payment");

            return Success(result, "Payment Initiated Successfully");
        }

        [HttpGet("admin/all")]
        [Authorize(Roles = "Admin")] // 🔒 حماية صارمة للأدمن فقط
        public async Task<ActionResult<Pagination<BookingDto>>> GetAllBookingsForAdmin([FromQuery] BookingSpecParams bookingParams)
        {
            var result = await serviceManager.BookingService.GetAllBookingsForAdminAsync(bookingParams);

            if (result == null)
                return BadRequestError("Error retrieving bookings for admin.");

            return Success(result, "Bookings retrieved successfully.");
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("admin/{id}")]
        public async Task<ActionResult<ApiResponse<AdminBookingDetailsDto>>> GetBookingForAdmin(int id)
        {
            var result = await serviceManager.BookingService.GetAdminBookingByIdAsync(id);

            return Success(result);
        }

    }
}
