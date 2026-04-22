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
using Makanak.Shared.Dto_s.Authentication.Password;
using Makanak.Shared.Dto_s.User;
using Makanak.Shared.HelpersFactory;
using Microsoft.AspNetCore.Identity;

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
        public async Task<bool> ConfirmEmailChangeAsync(VerifyOtpDto verifyOtpDto, string currentEmail)
        {
            var user = await userManager.FindByEmailAsync(currentEmail);
            if (user == null) throw UserNotFoundException.ByEmail(currentEmail);

            // this line will verify the OTP and change the email if the OTP is valid and burn the OTP after successful verification
            var result = await userManager.ChangeEmailAsync(user, verifyOtpDto.Email, verifyOtpDto.Otp);
            if (!result.Succeeded) throw new BadRequestException("Invalid OTP");

            // make sure to update the username if your application uses email as the username
            await userManager.SetUserNameAsync(user, verifyOtpDto.Email);
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

            // generate OTP for email change using userManager's GenerateChangeEmailTokenAsync method
            var otp = await userManager.GenerateChangeEmailTokenAsync(user, changeEmailDto.NewEmail);

            await emailService.SendEmailAsync(currentEmail, "Security Alert", $"Email change requested to {changeEmailDto.NewEmail}.");
            await Task.Delay(2000);
            await emailService.SendEmailAsync(changeEmailDto.NewEmail, "Confirm New Email - Makanak", $"Your OTP code is: {otp}");

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
            var user = await userManager.FindByEmailAsync(verifyOtpDto.Email);
            if (user == null) throw UserNotFoundException.ByEmail(verifyOtpDto.Email);

            // we use the VerifyUserTokenAsync method to verify the OTP against the token
            // provider for password reset
            var isValid = await userManager.VerifyUserTokenAsync(
                user,
                userManager.Options.Tokens.PasswordResetTokenProvider,
                UserManager<ApplicationUser>.ResetPasswordTokenPurpose,
                verifyOtpDto.Otp);

            if (!isValid) throw new BadRequestException("Invalid or Expired OTP");

            return true;
        }

    }
}