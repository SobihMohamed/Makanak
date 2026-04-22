using Makanak.Shared.Dto_s.Authentication;
using Makanak.Shared.Dto_s.Authentication.Password;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Abstraction.IServices.Auth
{
    public interface IPasswordService
    {
        Task<bool> ForgetPasswordAsync(ForgetPasswordRequestDto forgetPasswordRequestDto);
        Task<AuthModelDto> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<AuthModelDto> ChangePasswordAsync(ChangePasswordDto changePasswordDto, string email);
    }
}
