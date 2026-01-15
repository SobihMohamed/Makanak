using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Makanak.Shared.Dto_s.User
{
    public class VerifyIdentityDto
    {

        [Required(ErrorMessage = "National ID is required")]
        [StringLength(14, ErrorMessage = "National ID must be 14 characters")]
        public string? NationalId { get; set; } = null!;

        [Required(ErrorMessage = "National ID front image URL is required")]
        public IFormFile? NationalIdImageFrontUrl { get; set; } = null!;
        [Required(ErrorMessage = "National ID back image URL is required")]
        public IFormFile? NationalIdImageBackUrl { get; set; } = null!;
    }
}
