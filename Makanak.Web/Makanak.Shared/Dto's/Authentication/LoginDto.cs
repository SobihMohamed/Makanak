using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makanak.Shared.Dto_s
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]{2,}@[a-zA-Z0-9.-]{2,}\.[a-zA-Z]{3,}$", ErrorMessage = "Invalid email format. Please use a valid domain (e.g., user@example.com)")]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
    }
}
