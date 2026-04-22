using Makanak.Abstraction.IServices;
using Makanak.Abstraction.IServices.Auth;
using Makanak.Abstraction.IServicesContract.Token;
using Makanak.Domain.Exceptions;
using Makanak.Domain.Exceptions.NotFound;
using Makanak.Domain.Models.Identity;
using Makanak.Shared.Dto_s.Authentication;
using Makanak.Shared.Dto_s.Authentication.Password;
using Makanak.Shared.Dto_s.Token;
using Microsoft.AspNetCore.Identity;

namespace Makanak.Services.Services.Auth
{
    public class PasswordService(
        UserManager<ApplicationUser> userManager,
        IEmailService emailService,
        ITokenService tokenService) : IPasswordService
    {
        public async Task<AuthModelDto> ChangePasswordAsync(ChangePasswordDto changePasswordDto, string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) throw UserNotFoundException.ByEmail(email);

            var result = await userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
            if (!result.Succeeded) throw new BadRequestException("Password Change Failed", result.Errors.Select(e => e.Description));

            return await GenerateAuthResponse(user);
        }

        public async Task<bool> ForgetPasswordAsync(ForgetPasswordRequestDto forgetPasswordRequestDto)
        {
            var user = await userManager.FindByEmailAsync(forgetPasswordRequestDto.Email);
            if (user == null) throw UserNotFoundException.ByEmail(forgetPasswordRequestDto.Email);

            // generate OTP by take user and generate password reset token using userManager
            var otp = await userManager.GeneratePasswordResetTokenAsync(user);

            await emailService.SendEmailAsync(user.Email!, "Password Reset OTP", $"Your OTP code is: {otp}");
            return true;
        }

        public async Task<AuthModelDto> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var user = await userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null) throw UserNotFoundException.ByEmail(resetPasswordDto.Email);

            var result = await userManager.ResetPasswordAsync
                (user, resetPasswordDto.Otp, resetPasswordDto.NewPassword);

            if (!result.Succeeded) 
                throw new BadRequestException("Invalid OTP or Password Reset Failed",
                    result.Errors.Select(e => e.Description));

            return await GenerateAuthResponse(user);
        }

        private async Task<AuthModelDto> GenerateAuthResponse(ApplicationUser user)
        {
            var roles = await userManager.GetRolesAsync(user);
            var tokenRequest = new TokenRequestDto { 
                UserId = user.Id,
                UserName = user.UserName!,
                Email = user.Email!, 
                Roles = roles.ToList() 
            };
            var tokenResponse = await tokenService.CreateTokenAsync(tokenRequest);

            return new AuthModelDto
            {
                Message = "Operation Successful",
                Name = user.UserName!,
                Email = user.Email!,
                IsAuthenticated = true,
                Token = tokenResponse.Token,
                ExpiresOn = tokenResponse.ExpireOn,
                Roles = roles.ToList()
            };
        }
    }
}