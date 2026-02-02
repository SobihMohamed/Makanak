using Makanak.Abstraction.IServices.Booking;
using Makanak.Abstraction.IServices.Manager;
using Makanak.Shared.Dto_s.Booking;
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
        public async Task<IActionResult> CreateBooking(CreateBookingDto dto)
        {
            var tenantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(tenantId)) return UnauthorizedError("User ID not found in token");

            var booking = await serviceManager.BookingService.CreateBookingAsync(dto, tenantId);

            if (booking == null) return BadRequestError("Problem creating booking");

            return Created(booking, "Booking created successfully");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookingById(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var booking = await serviceManager.BookingService.GetBookingByIdAsync(id, userId, userRole);

            if (booking == null) return NotFoundError($"Booking with ID {id} not found");

            return Success(booking);
        }

        [Authorize(Roles = "Tenant")]
        [HttpGet("my-bookings")]
        public async Task<IActionResult> GetMyBookingsAsTenant()
        {
            var tenantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var bookings = await serviceManager.BookingService.GetTenantBookingsAsync(tenantId);

            return Success(bookings);
        }

        [Authorize(Roles = "Owner")]
        [HttpGet("incoming-bookings")]
        public async Task<IActionResult> GetIncomingBookingsAsOwner()
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var bookings = await serviceManager.BookingService.GetOwnerBookingsAsync(ownerId);
            return Success(bookings);
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var result = await serviceManager.BookingService.CancelBookingAsync(id, userId, userRole);

            if (!result) return BadRequestError("Failed to cancel booking");

            return Success("Booking cancelled successfully");
        }

        [Authorize(Roles = "Owner")]
        [HttpPost("scan-qr")]
        public async Task<IActionResult> ScanQrCode([FromBody] ScanQrRequestDto request)
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await serviceManager.BookingService.ScanQrCodeAsync(request.QrCode, ownerId);

            if (!result) return BadRequestError("Failed to scan QR Code or code invalid");

            return Success("Check-in successful! ✅");
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateStatus(UpdateBookingStatusDto dto)
        {
            // مش محتاجين Enum Parsing لأن الـ DTO مظبوط
            var res =  await serviceManager.BookingService.UpdateBookingStatusAsync(dto.BookingId, dto.Status);

            if (!res) return BadRequest("Failed to update booking status");

            return Success("Status updated successfully");
        }
    }
}
