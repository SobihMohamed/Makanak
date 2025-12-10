using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Makanak.Shared.Dto_s.User
{
    public class UpdateProfileDto
    {
        [Required]
        public string? Name { get; set; } = null!;
        public string? ProfilePictureUrl { get; set; }
        public string? Address { get; set; }
        [Required]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; } = null!;

        public IFormFile? ProfilePicture { get; set; }

    }
}
