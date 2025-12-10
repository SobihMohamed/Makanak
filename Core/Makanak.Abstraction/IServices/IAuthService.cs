using Makanak.Shared.Dto_s;
using Makanak.Shared.Dto_s.Authentication;
using Makanak.Shared.Dto_s.Authentication.Password;
using Makanak.Shared.Dto_s.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Abstraction.IServices
{
    public interface IAuthService
    {
        Task<AuthModelDto> LoginAsync(LoginDto loginDto);
        Task<AuthModelDto> RegisterAsync(RegisterDto registerDto);

        Task<CurrentUserDto> GetUserProfileAsync(string email);
        
        Task<AuthModelDto> UpdateProfileAsync(UpdateProfileDto updateProfileDto , string email);
        
        Task<string> VerifyIdentityAsync(VerifyIdentityDto verifyIdentityDto , string email);
        
        Task<string> ForgetPasswordAsync(ForgetPasswordRequestDto forgetPasswordRequestDto);
        Task<bool> VerifyOtpAsync(VerifyOtpDto verifyOtpDto);
        Task<AuthModelDto> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);

    }
}
