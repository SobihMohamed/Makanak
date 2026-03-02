using Makanak.Abstraction.IServices.Admin;
using Makanak.Abstraction.IServices.Manager;
using Makanak.Shared.Common;
using Makanak.Shared.Common.Params;
using Makanak.Shared.Common.Params.User;
using Makanak.Shared.Dto_s.Admin;
using Makanak.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Makanak.Presentation.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminController(IServiceManager serviceManager) : AppBaseController
    {
        [HttpGet("users")]
        public async Task<ActionResult<Pagination<UserForApprovalDto>>> GetUsers([FromQuery] UserParams userParams)
        {
            var pendingUsers = await serviceManager.AdminService.GetAllUsersAsync(userParams);
            return Success(pendingUsers, "users retrived successfully");
        }

        [HttpPut("users/status")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateUserStatus([FromBody] UpdateUserStatusDto dto)
        {
            var res = await serviceManager.AdminService.UpdateUserStatusAsync(dto);
            if (!res) return BadRequest("Failed to update user status");

            return Success("User status updated successfully");
        }

        [HttpPut("properties/status")]
        public async Task<ActionResult<ApiResponse<string>>> UpdatePropertyStatus([FromBody] UpdatePropertyStatusDto dto)
        {
            var res = await serviceManager.AdminService.UpdatePropertyStatus(dto);
            if (!res) return BadRequest("Failed to update Property status");

            return Success("Property status updated successfully");
        }

        [HttpGet("users/{userId}/verification-details")]
        public async Task<ActionResult<ApiResponse<UserVerificationDetailsDto>>> GetUserVerificationDetails([FromRoute] string userId)
        {
            var userDetails = await serviceManager.AdminService.GetUserVerificationDetails(userId);
            return Success(userDetails, "User verification details retrieved successfully");
        }

        [HttpPost("users/{userId}/strike")]
        public async Task<ActionResult<ApiResponse<string>>> AddStrike([FromRoute] string userId)
        {
            var res = await serviceManager.AdminService.AddStrikeToUserAsync(userId);
            if (!res) return BadRequest("Failed to add strike to user. Please try again.");

            return Success("Strike added to user successfully.");
        }

        // (Remove Strike)
        [HttpDelete("users/{userId}/strike")]
        public async Task<ActionResult<ApiResponse<string>>> RemoveStrike([FromRoute] string userId)
        {
            var res = await serviceManager.AdminService.RemoveStrikeFromUserAsync(userId);
            if (!res) return BadRequest("Failed to remove strike from user. Please try again.");

            return Success("Strike removed from user successfully.");
        }
    }
}
