using Makanak.Shared.Dto_s.Authentication.Password;
using Makanak.Shared.Dto_s.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Abstraction.IServices.Auth
{
    public interface IVerificationService
    {
        Task<string> InitiateEmailChangeAsync(ChangeEmailDto changeEmailDto, string currentEmail);
        Task<bool> ConfirmEmailChangeAsync(VerifyOtpDto verifyOtpDto);
        Task<bool> VerifyIdentityAsync(VerifyIdentityDto verifyIdentityDto, string email);
        Task<bool> VerifyOtpAsync(VerifyOtpDto verifyOtpDto);
    }
}
