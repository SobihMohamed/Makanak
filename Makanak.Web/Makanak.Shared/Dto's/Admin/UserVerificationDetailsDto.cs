using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Shared.Dto_s.Admin
{
    public class UserVerificationDetailsDto : UserForApprovalDto
    {
        public string? NationalId { get; set; } = null!;
        public string? NationalIdImageFrontUrl { get; set; } = null!;
        public string? NationalIdImageBackUrl { get; set; } = null!;

        public DateTime DateOfBirth { get; set; }
        public string? Address { get; set; } = null!;
        public int StrikeCount { get; set; } = 0;

        public string? ProfilePictureUrl { get; set; }
    }
}
