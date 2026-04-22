using Makanak.Abstraction.IServices.Manager;
using Makanak.Shared.Dto_s.Authentication;
using Makanak.Shared.Dto_s.Authentication.Password;
using Makanak.Shared.Dto_s.User;
using Makanak.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Makanak.Presentation.Controllers.Auth
{
    public class SecurityController(IServiceManager serviceManager) : AppBaseController
    {
        #region Password Management
        [EnableRateLimiting("OtpPolicy")]
        [HttpPost("forget-password")]
        public async Task<ActionResult<ApiResponse<string>>> ForgetPassword([FromBody] ForgetPasswordRequestDto forgetPasswordRequestDto)
        {
            await serviceManager.PasswordService.ForgetPasswordAsync(forgetPasswordRequestDto);
            return Success("Password reset instructions have been sent to your email.");
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult<ApiResponse<AuthModelDto>>> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var res = await serviceManager.PasswordService.ResetPasswordAsync(resetPasswordDto);
            return Success(res, "Password has been reset successfully.");
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<ActionResult<ApiResponse<AuthModelDto>>> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var res = await serviceManager.PasswordService.ChangePasswordAsync(changePasswordDto, email!);
            return Success(res, "Password changed successfully.");
        }
        #endregion

        #region Verification & Identity
        [EnableRateLimiting("OtpPolicy")]
        [HttpPost("verify-otp")]
        public async Task<ActionResult<ApiResponse<string>>> VerifyOtp([FromBody] VerifyOtpDto verifyOtpDto)
        {
            await serviceManager.VerificationService.VerifyOtpAsync(verifyOtpDto);
            return Success("OTP verified successfully.");
        }

        [Authorize]
        [HttpPost("verify-identity")]
        public async Task<ActionResult<ApiResponse<string>>> VerifyIdentity([FromForm] VerifyIdentityDto verifyIdentityDto)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            await serviceManager.VerificationService.VerifyIdentityAsync(verifyIdentityDto, email!);
            return Success("Documents uploaded successfully. Your account is pending admin approval.");
        }

        [Authorize]
        [HttpPost("initiate-email-change")]
        public async Task<ActionResult<ApiResponse<string>>> InitiateEmailChange([FromBody] ChangeEmailDto changeEmailDto)
        {
            var currentEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var result = await serviceManager.VerificationService.InitiateEmailChangeAsync(changeEmailDto, currentEmail!);
            return Success(result, "Email change initiated. Please verify using the link sent to your new email.");
        }

        [Authorize]
        [EnableRateLimiting("OtpPolicy")]
        [HttpPost("confirm-email-change")]
        public async Task<ActionResult<ApiResponse<string>>> ConfirmEmailChange([FromBody] VerifyOtpDto verifyOtpDto)
        {
            var currentEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            await serviceManager.VerificationService.ConfirmEmailChangeAsync(verifyOtpDto, currentEmail!);
            return Success("Email address updated successfully.");
        }
        #endregion
    }
}