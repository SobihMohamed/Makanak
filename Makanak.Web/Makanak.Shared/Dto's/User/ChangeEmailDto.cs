using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Makanak.Shared.Dto_s.User
{
    public class ChangeEmailDto
    {
        [Required, EmailAddress]
        public string NewEmail { get; set; }

        [Required]
        public string CurrentPassword { get; set; }
    }
}
