using Makanak.Domain.EnumsHelper.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makanak.Shared.Dto_s
{
    public class RegisterDto
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]{2,}@[a-zA-Z0-9.-]{2,}\.[a-zA-Z]{3,}$", ErrorMessage = "Invalid email format. Please use a valid domain (e.g., user@example.com)")]

        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string Password { get; set; } = null!;
        [Required,Phone]
        public string PhoneNumber { get; set; } = null!;
        [Required]
        public DateTime DateOfBirth { get; set; } // => to calculate the age
        [Required]
        public UserTypes UserType { get; set; }
    }
}
