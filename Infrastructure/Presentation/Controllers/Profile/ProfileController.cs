using Makanak.Abstraction.IServices.Manager;
using Makanak.Shared.Dto_s.User;
using Makanak.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Makanak.Presentation.Controllers.Profile
{
    public class ProfileController(IServiceManager serviceManager) : AppBaseController
    {
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<CurrentUserDto>>> GetProfile()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var result = await serviceManager.UserProfileService.GetUserProfileAsync(email!);
            return Success(result);
        }

        [Authorize]
        [HttpPut]
        public async Task<ActionResult<ApiResponse<CurrentUserDto>>> UpdateProfile([FromForm] UpdateProfileDto updateProfileDto)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var result = await serviceManager.UserProfileService.UpdateProfileAsync(updateProfileDto, email!);
            return Success(result, "User Profile Updated Successfully");
        }
    }
}
