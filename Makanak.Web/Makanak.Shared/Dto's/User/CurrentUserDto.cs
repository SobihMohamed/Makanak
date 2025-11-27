using Makanak.Domain.EnumsHelper.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makanak.Shared.Dto_s.User
{
    public class CurrentUserDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? ProfilePictureUrl { get; set; }
        public string? Address { get; set; }
        public int Age { get; set; } // Get age instead of date of birth
        //public DateTime DateOfBirth { get; set; }


        public string? NationalId { get; set; } = null!;
        public string? NationalIdImageFrontUrl { get; set; } = null!;
        public string? NationalIdImageBackUrl { get; set; } = null!;

        public int StrikeCount { get; set; } = 0;
        public string UserType { get; set; } = string.Empty;
        public string UserStatus { get; set; } = string.Empty;
        public string? RejectedReason { get; set; } 
        public DateTime JoinAt { get; set; } // CreatedAt of ApplicationUser

    }
}
