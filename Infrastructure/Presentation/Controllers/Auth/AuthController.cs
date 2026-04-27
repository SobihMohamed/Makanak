using Makanak.Abstraction.IServices.Manager;
using Makanak.Shared.Dto_s;
using Makanak.Shared.Dto_s.Authentication;
using Makanak.Shared.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Makanak.Presentation.Controllers.Auth
{
    public class AuthController(IServiceManager serviceManager) : AppBaseController
    {
        #region Basic Actions
        [HttpPost("login")]  // POST: api/auth/login
        public async Task<ActionResult<ApiResponse<AuthModelDto>>> Login([FromBody] LoginDto loginDto)
        {
            var result = await serviceManager.AuthService.LoginAsync(loginDto);
            return Success(result, "Login Successful");
        }

        [HttpPost("register")]  // POST: api/auth/register
        public async Task<ActionResult<ApiResponse<AuthModelDto>>> Register([FromBody] RegisterDto registerDto)
        {
            var result = await serviceManager.AuthService.RegisterAsync(registerDto);
            return Created(result, "Registration Successful");
        }

        [HttpPost("logout")]
        public ActionResult<ApiResponse<string>> Logout()
        {
            return Success("Logged out successfully. Please remove the token from client storage."); 
        }
        #endregion
    }
}