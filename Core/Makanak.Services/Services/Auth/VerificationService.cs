using Makanak.Abstraction.IServices;
using Makanak.Abstraction.IServices.Auth;
using Makanak.Abstraction.IServices.NotificationService;
using Makanak.Domain.Contracts.UOW;
using Makanak.Domain.EnumsHelper.User;
using Makanak.Domain.Exceptions;
using Makanak.Domain.Exceptions.NotFound;
using Makanak.Domain.Models.Identity;
using Makanak.Domain.Models.ResetPassword;
using Makanak.Services.Specifications.User;
using Makanak.Shared.Dto_s.Authentication; // مضاف لـ VerifyOtpDto
using Makanak.Shared.Dto_s.Authentication.Password;
using Makanak.Shared.Dto_s.User;
using Makanak.Shared.HelpersFactory;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq; // عشان الـ Select
using System.Threading.Tasks;

namespace Makanak.Services.Services.Auth
{
    public class VerificationService(
        UserManager<ApplicationUser> userManager,
        IUnitOfWork unitOfWork,
        IAttachementServices attachementServices,
        INotificationService notificationService,
        IEmailService emailService 
        ) : IVerificationService
    {
        public async Task<bool> ConfirmEmailChangeAsync(VerifyOtpDto verifyOtpDto)
        {
            var otpRecord = await VerifyAndBurnOtpAsync(verifyOtpDto.Email, verifyOtpDto.Otp, true);

            var user = await userManager.FindByIdAsync(otpRecord.UserId);
            if (user == null) throw UserNotFoundException.ByEmail(otpRecord.Email);

            user.Email = otpRecord.Email;
            user.UserName = otpRecord.Email;
            user.EmailConfirmed = true;
            user.NormalizedEmail = userManager.NormalizeEmail(user.Email);
            user.NormalizedUserName = userManager.NormalizeName(user.UserName);

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                throw new BadRequestException("Email Change Failed", errors);
            }
            return true;
        }

        public async Task<string> InitiateEmailChangeAsync(ChangeEmailDto changeEmailDto, string currentEmail)
        {
            var user = await userManager.FindByEmailAsync(currentEmail);
            if (user == null) throw UserNotFoundException.ByEmail(currentEmail);

            var isPasswordValid = await userManager.CheckPasswordAsync(user, changeEmailDto.CurrentPassword);
            if (!isPasswordValid) throw new BadRequestException("Incorrect current password");

            var isEmailExist = await userManager.FindByEmailAsync(changeEmailDto.NewEmail);
            if (isEmailExist != null) throw new BadRequestException("Email is already in use");

            var otp = await GenerateAndSaveOtpAsync(user.Id, changeEmailDto.NewEmail);

            await emailService.SendEmailAsync(
                changeEmailDto.NewEmail,
                "Confirm New Email - Makanak",
                $"Use this code to verify your new email: {otp}");

            await emailService.SendEmailAsync(
                currentEmail,
                "Security Alert: Email Change Requested",
                $"Hello,\nWe received a request to change the email associated with your Makanak account to " +
                $"({changeEmailDto.NewEmail}).\n\nIf you made this request, you can safely ignore this email." +
                $"\nIf you did NOT make this request, please contact our support team immediately to secure your account."
            );
            return otp;
        }

        public async Task<bool> VerifyIdentityAsync(VerifyIdentityDto verifyIdentityDto, string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) throw UserNotFoundException.ByEmail(email);

            if (user.UserStatus == UserStatus.Pending || user.UserStatus == UserStatus.Active)
                throw new BadRequestException("You cannot update your identity while it is pending or Active.");

            var userRepository = unitOfWork.GetRepo<ApplicationUser, string>();
            var userSpecification = new UserSpecifications(verifyIdentityDto.NationalId!, user.Id);

            var isDuplicated = await userRepository.GetByIdWithSpecificationsAsync(userSpecification);
            if (isDuplicated != null) throw new BadRequestException("National ID is already in use by another user.");

            if (!string.IsNullOrEmpty(user.NationalIdImageFrontUrl))
                await attachementServices.DeleteImage(user.NationalIdImageFrontUrl);

            if (!string.IsNullOrEmpty(user.NationalIdImageBackUrl))
                await attachementServices.DeleteImage(user.NationalIdImageBackUrl);

            var ImagePath = Path.Combine("Users", user.Id, "Identity");

            string frontImagePath = await attachementServices.UploadImageAsync(verifyIdentityDto.NationalIdImageFrontUrl!, ImagePath);
            string backImagePath = await attachementServices.UploadImageAsync(verifyIdentityDto.NationalIdImageBackUrl!, ImagePath);

            user.NationalId = verifyIdentityDto.NationalId;
            user.NationalIdImageFrontUrl = frontImagePath;
            user.NationalIdImageBackUrl = backImagePath;
            user.UserStatus = UserStatus.Pending;

            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                throw new BadRequestException("Identity Verification Failed: ", errors);
            }

            var admins = await userManager.GetUsersInRoleAsync("Admin");

            foreach (var admin in admins)
            {
                await notificationService.SendNotificationAsync(
                    NotificationFactory.DocumentVerificationRequest(
                        admin.Id,
                        user.Name,
                        user.Id
                    )
                );
            }
            return true;
        }

        public async Task<bool> VerifyOtpAsync(VerifyOtpDto verifyOtpDto)
        {
            await VerifyAndBurnOtpAsync(verifyOtpDto.Email, verifyOtpDto.Otp, true);
            return true;
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

            return newOtp;
        }

        private async Task<UserOtp> VerifyAndBurnOtpAsync(string email, string otp, bool burnIt)
        {
            var userOtpRepo = unitOfWork.GetRepo<UserOtp, int>();
            var specification = new UserOtpSpecifications(email);
            var existOtp = await userOtpRepo.GetByIdWithSpecificationsAsync(specification);

            if (existOtp == null) throw new BadRequestException("Invalid Otp");

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