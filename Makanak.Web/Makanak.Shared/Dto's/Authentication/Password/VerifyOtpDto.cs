using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Makanak.Shared.Dto_s.Authentication.Password
{
    public class VerifyOtpDto
    {
        public string Email { get; set; } = null!;
        [Required(ErrorMessage = "OTP is required")]
        [MinLength(6, ErrorMessage = "OTP must be 6 characters long")]
        public string Otp { get; set; } = null!;
    }
}
