using Makanak.Abstraction.IServices.Auth;
using Makanak.Abstraction.IServices.Manager;
using Makanak.Shared.Dto_s;
using Makanak.Shared.Dto_s.Authentication;
using Makanak.Shared.Dto_s.Authentication.Password;
using Makanak.Shared.Dto_s.User;
using Makanak.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        #region User Profile Actions
        [Authorize]
        [HttpGet("profile")]  // GET: api/auth/profile
        public async Task<ActionResult<ApiResponse<CurrentUserDto>>> GetProfile()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value; // get email from token
            var result = await serviceManager.AuthService.GetUserProfileAsync(email!);
            return Success(result);
        }

        [Authorize]
        [HttpPut("profile")]  // PUT: api/auth/profile
        public async Task<ActionResult<ApiResponse<CurrentUserDto>>> UpdateProfile([FromForm] UpdateProfileDto updateProfileDto)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value; // get email from token
            var result = await serviceManager.AuthService.UpdateProfileAsync(updateProfileDto, email!);
            return Success(result, "User Profile Updated Successfully");
        }

        #endregion
       
        #region Password
        [HttpPost("forget-password")]  // POST: api/auth/forget-password
        public async Task<ActionResult<ApiResponse<string>>> ForgetPassword([FromBody] ForgetPasswordRequestDto forgetPasswordRequestDto)
        {
            await serviceManager.AuthService.ForgetPasswordAsync(forgetPasswordRequestDto);
            return Success("Password reset instructions have been sent to your email.");
        }

        [HttpPost("verify-otp")] // POST: api/auth/verify-otp
        public async Task<ActionResult<ApiResponse<string>>> VerifyOtp([FromBody] VerifyOtpDto verifyOtpDto)
        {
            await serviceManager.AuthService.VerifyOtpAsync(verifyOtpDto);
            return Success("OTP verified successfully.");
        }

        [HttpPost("reset-password")] // POST: api/auth/reset-password
        public async Task<ActionResult<ApiResponse<AuthModelDto>>> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var res = await serviceManager.AuthService.ResetPasswordAsync(resetPasswordDto);
            return Success(res, "Password has been reset successfully.");
        }

        [HttpPost("change-password")] // POST: api/auth/change-password
        public async Task<ActionResult<ApiResponse<AuthModelDto>>> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value; // get email from token
            var res = await serviceManager.AuthService.ChangePasswordAsync(changePasswordDto, email!);
            return Success(res,"Password changed successfully.");
        }
        #endregion

        #region Verifying Identity
        [Authorize]
        [HttpPost("verify-identity")] // POST: api/auth/verify-identity
        public async Task<ActionResult<ApiResponse<string>>> VerifyIdentity([FromForm] VerifyIdentityDto verifyIdentityDto)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value; // get email from token
            await serviceManager.AuthService.VerifyIdentityAsync(verifyIdentityDto, email!);
            return Success("Documents uploaded successfully. Your account is pending admin approval.");
        }

        [Authorize]
        [HttpPost("initiate-email-change")]
        public async Task<ActionResult<ApiResponse<string>>> InitiateEmailChange([FromBody] ChangeEmailDto changeEmailDto)
        {
            var currentEmail = User.FindFirst(ClaimTypes.Email)?.Value; // get email from token

            var result = await serviceManager.AuthService.InitiateEmailChangeAsync(changeEmailDto, currentEmail!);

            return Success(result, "Email change initiated. Please verify using the link sent to your new email.");
        }

        [Authorize]
        [HttpPost("confirm-email-change")]
        public async Task<ActionResult<ApiResponse<string>>> ConfirmEmailChange([FromBody] VerifyOtpDto verifyOtpDto)
        {
            await serviceManager.AuthService.ConfirmEmailChangeAsync(verifyOtpDto);

            return Success("Email address updated successfully.");
        }
        #endregion

    }
}