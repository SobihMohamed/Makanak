using Makanak.Domain.Models.ResetPassword;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Services.Specifications.User
{
    public class VerifyUserOtpSpecification : BaseSpecifications<UserOtp,int>
    {
        public VerifyUserOtpSpecification(string email , string otpCode)
        : base(U_Otp => U_Otp.Email == email && U_Otp.OtpCode == otpCode && U_Otp.IsUsed == false )
        {     
        }
    }
}
