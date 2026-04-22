using Makanak.Abstraction.IServices;
using Makanak.Abstraction.IServices.Auth;
using Makanak.Abstraction.IServicesContract.Token;
using Makanak.Domain.Contracts.UOW;
using Makanak.Domain.Exceptions;
using Makanak.Domain.Exceptions.NotFound;
using Makanak.Domain.Models.Identity;
using Makanak.Domain.Models.ResetPassword;
using Makanak.Services.Specifications.User;
using Makanak.Shared.Dto_s.Authentication;
using Makanak.Shared.Dto_s.Authentication.Password;
using Makanak.Shared.Dto_s.Token;
using Microsoft.AspNetCore.Identity;

namespace Makanak.Services.Services.Auth
{
    public class PasswordService(
        UserManager<ApplicationUser> userManager,
        IEmailService emailService,
        IUnitOfWork unitOfWork,
        ITokenService tokenService) : IPasswordService
    {
        public async Task<AuthModelDto> ChangePasswordAsync(ChangePasswordDto changePasswordDto, string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
                throw UserNotFoundException.ByEmail(email);

            var result = await userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                throw new BadRequestException("Password Change Failed", errors);
            }

            // 1. Generate Token using TokenService
            var roles = await userManager.GetRolesAsync(user);
            var tokenRequest = new TokenRequestDto
            {
                UserId = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                Roles = roles.ToList()
            };
            var tokenResponse = await tokenService.CreateTokenAsync(tokenRequest);

            return new AuthModelDto
            {
                Message = "Password Changed Successfully",
                Name = user.UserName!,
                Email = user.Email!,
                IsAuthenticated = true,
                Token = tokenResponse.Token,
                ExpiresOn = tokenResponse.ExpireOn, 
                Roles = roles.ToList()
            };
        }

        public async Task<bool> ForgetPasswordAsync(ForgetPasswordRequestDto forgetPasswordRequestDto)
        {
            var user = await userManager.FindByEmailAsync(forgetPasswordRequestDto.Email);
            if (user == null) throw UserNotFoundException.ByEmail(forgetPasswordRequestDto.Email);

            var newOtp = await GenerateAndSaveOtpAsync(user.Id, forgetPasswordRequestDto.Email);

            await emailService.SendEmailAsync(user.Email!, "Password Reset OTP", $"Your OTP code is: {newOtp}");

            return true;
        }

        public async Task<AuthModelDto> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var user = await userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
            {
                throw UserNotFoundException.ByEmail(resetPasswordDto.Email);
            }

            await VerifyAndBurnOtpAsync(resetPasswordDto.Email, resetPasswordDto.Otp, true);

            if (await userManager.HasPasswordAsync(user!))
            {
                await userManager.RemovePasswordAsync(user!);
            }
            var resetPassResult = await userManager.AddPasswordAsync(user!, resetPasswordDto.NewPassword);

            if (!resetPassResult.Succeeded)
            {
                var errors = resetPassResult.Errors.Select(e => e.Description);
                throw new BadRequestException("Password Reset Failed", errors);
            }

            // 2. Generate Token using TokenService
            var roles = await userManager.GetRolesAsync(user);
            var tokenRequest = new TokenRequestDto
            {
                UserId = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                Roles = roles.ToList()
            };
            var tokenResponse = await tokenService.CreateTokenAsync(tokenRequest);

            return new AuthModelDto
            {
                Message = "Password Reset Successful",
                Name = user!.UserName!,
                Email = user.Email!,
                IsAuthenticated = true,
                Token = tokenResponse.Token,
                ExpiresOn = tokenResponse.ExpireOn,
                Roles = roles.ToList()
            };
        }

        private async Task<string> GenerateAndSaveOtpAsync(string UserId, string email)
        {
            var Specification = new UserOtpSpecifications(email);
            var userOtpRepo = unitOfWork.GetRepo<UserOtp, int>();

            var oldUserOtp = await userOtpRepo.GetAllWithSpecificationAsync(Specification);

            foreach (var otp in oldUserOtp)
            {
                otp.IsUsed = true;
                userOtpRepo.Update(otp);
            }

            var newOtp = new Random().Next(100000, 999999).ToString();

            var userOtp = new UserOtp
            {
                Email = email,
                OtpCode = newOtp,
                ExpirationTime = DateTime.UtcNow.AddMinutes(5), 
                IsUsed = false,
                UserId = UserId,
                LastModifiedBy = UserId,
                UpdatedAt = DateTime.UtcNow
            };

            userOtpRepo.AddAsync(userOtp);
            await unitOfWork.SaveChangesAsync();

            return "Otp Generated and sent";
        }

        private async Task<UserOtp> VerifyAndBurnOtpAsync(string email, string otp, bool burnIt)
        {
            var userOtpRepo = unitOfWork.GetRepo<UserOtp, int>();
            var specification = new UserOtpSpecifications(email);
            var existOtp = await userOtpRepo.GetByIdWithSpecificationsAsync(specification);

            if (existOtp == null)
            {
                throw new BadRequestException("Invalid Otp");
            }

            if (existOtp.ExpirationTime < DateTime.UtcNow)
            {
                existOtp.IsUsed = true;
                userOtpRepo.Update(existOtp);
                await unitOfWork.SaveChangesAsync();
                throw new BadRequestException("Expired Otp");
            }

            if (existOtp.OtpCode != otp)
            {
                existOtp.FailedAttempts += 1;
                if (existOtp.FailedAttempts >= 3)
                {
                    existOtp.IsUsed = true;
                    userOtpRepo.Update(existOtp);
                    await unitOfWork.SaveChangesAsync();
                    throw new BadRequestException("Maximum attempts reached. Please request a new OTP.");
                }
                userOtpRepo.Update(existOtp);
                await unitOfWork.SaveChangesAsync();
                throw new BadRequestException($"Invalid Otp. You have {3 - existOtp.FailedAttempts} attempts left.");
            }

            existOtp.IsUsed = burnIt;
            userOtpRepo.Update(existOtp);
            await unitOfWork.SaveChangesAsync();

            return existOtp;
        }
    }
}