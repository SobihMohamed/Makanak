using Makanak.Abstraction.IServices.Auth;
using Makanak.Shared.Dto_s;
using Makanak.Shared.Dto_s.Authentication;
using Makanak.Shared.Dto_s.Authentication.Password;
using Makanak.Shared.Dto_s.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Makanak.Presentation.Controllers.Auth
{
    public class AuthController (IAuthService authService) : AppBaseController
    {
        [HttpPost("login")]  // POST: api/auth/login
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var result = await authService.LoginAsync(loginDto);
            return Success(result, "Login Successful");
        }

        [HttpPost("register")]  // POST: api/auth/register
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var result = await authService.RegisterAsync(registerDto);
            return Created(result, "Registration Successful");
        }

        [Authorize]
        [HttpGet("profile")]  // GET: api/auth/profile
        public async Task<IActionResult> GetProfile()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value; // get email from token
            var result = await authService.GetUserProfileAsync(email!);
            return Success(result);
        }

        [Authorize]
        [HttpPut("profile")]  // PUT: api/auth/profile
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileDto updateProfileDto)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value; // get email from token
            var result = await authService.UpdateProfileAsync(updateProfileDto,email!);
            return Success(result, "User Profile Updated Successfully");
        }

        [HttpPost("forget-password")]  // POST: api/auth/forget-password
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequestDto forgetPasswordRequestDto)
        {
            await authService.ForgetPasswordAsync(forgetPasswordRequestDto);
            return Success("Password reset instructions have been sent to your email.");
        }

        [HttpPost("verify-otp")] // POST: api/auth/verify-otp
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto verifyOtpDto)
        {
            await authService.VerifyOtpAsync(verifyOtpDto);
            return Success("OTP verified successfully.");
        }

        [HttpPost("reset-password")] // POST: api/auth/reset-password
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var res = await authService.ResetPasswordAsync(resetPasswordDto);
            return Success(res , "Password has been reset successfully.");
        }

        [Authorize]
        [HttpPost("verify-identity")] // POST: api/auth/verify-identity
        public async Task<IActionResult> VerifyIdentity([FromForm] VerifyIdentityDto verifyIdentityDto)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value; // get email from token
            await authService.VerifyIdentityAsync(verifyIdentityDto, email!);
            return Success("Documents uploaded successfully. Your account is pending admin approval.");
        }

        [Authorize]
        [HttpPost("initiate-email-change")]
        public async Task<IActionResult> InitiateEmailChange([FromBody] ChangeEmailDto changeEmailDto)
        {
            var currentEmail = User.FindFirst(ClaimTypes.Email)?.Value; // get email from token

            var result = await authService.InitiateEmailChangeAsync(changeEmailDto, currentEmail!);

            return Success(result, "Email change initiated. Please verify using the link sent to your new email.");
        }

        [Authorize]
        [HttpPost("confirm-email-change")]
        public async Task<IActionResult> ConfirmEmailChange([FromBody] VerifyOtpDto verifyOtpDto)
        {   
            await authService.ConfirmEmailChangeAsync(verifyOtpDto);

            return Success("Email address updated successfully.");
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok(new { message = "Logged out successfully. Please remove the token from client storage." });
        }
    }
}