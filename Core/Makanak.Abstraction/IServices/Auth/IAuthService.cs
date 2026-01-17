using Makanak.Shared.Dto_s;
using Makanak.Shared.Dto_s.Authentication;
using Makanak.Shared.Dto_s.Authentication.Password;
using Makanak.Shared.Dto_s.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Abstraction.IServices.Auth
{
    public interface IAuthService
    {
        Task<AuthModelDto> LoginAsync(LoginDto loginDto);
        
        Task<AuthModelDto> RegisterAsync(RegisterDto registerDto);
        
        Task<bool> Logout(string email);
        
        Task<CurrentUserDto> GetUserProfileAsync(string email);

        Task<string> InitiateEmailChangeAsync(ChangeEmailDto changeEmailDto, string currentEmail);

        Task<bool> ConfirmEmailChangeAsync(VerifyOtpDto verifyOtpDto);

        Task<CurrentUserDto> UpdateProfileAsync(UpdateProfileDto updateProfileDto , string email);
        
        Task<bool> VerifyIdentityAsync(VerifyIdentityDto verifyIdentityDto , string email);
        
        Task<bool> ForgetPasswordAsync(ForgetPasswordRequestDto forgetPasswordRequestDto);

        Task<bool> VerifyOtpAsync(VerifyOtpDto verifyOtpDto);
        
        Task<AuthModelDto> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);

    }
}
