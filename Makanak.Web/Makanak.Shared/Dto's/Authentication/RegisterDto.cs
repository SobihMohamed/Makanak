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
        [EmailAddress,Required]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
        [Required,Phone]
        public string PhoneNumber { get; set; } = null!;
        [Required]
        public UserTypes UserType { get; set; }
    }
}
