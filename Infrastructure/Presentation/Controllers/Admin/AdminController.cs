using Makanak.Abstraction.IServices.Admin;
using Makanak.Abstraction.IServices.Manager;
using Makanak.Shared.Dto_s.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Makanak.Presentation.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminController(IServiceManager serviceManager) : AppBaseController
    {
        [HttpGet("users/pending")]
        public async Task<IActionResult> GetPendingUsers()
        {
            var pendingUsers = await serviceManager.AdminService.GetAllPendingUsersAsync();
            return Success(pendingUsers, "Pending users retrived successfully");
        }

        [HttpPut("users/status")]
        public async Task<IActionResult> UpdateUserStatus([FromBody] UpdateUserStatusDto dto)
        {
            var res = await serviceManager.AdminService.UpdateUserStatusAsync(dto);
            if (!res) return BadRequest("Failed to update user status");

            return Success("User status updated successfully");
        }

        [HttpGet("users/{userId}/verification-details")]
        public async Task<IActionResult> GetUserVerificationDetails([FromRoute] string userId)
        {
            var userDetails = await serviceManager.AdminService.GetUserVerificationDetails(userId);
            return Success(userDetails, "User verification details retrieved successfully");
        }

    }
}
