using Makanak.Domain.Models.ResetPassword;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Makanak.Services.Specifications
{
    public class UserOtpSpecification : BaseSpecifications<UserOtp, int>
    {
        public UserOtpSpecification(string email) :
            base(U_Otp => U_Otp.Email == email && U_Otp.IsUsed == false)
        {
        }
    }
}