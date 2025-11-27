using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makanak.Shared.Dto_s
{
    public class UserPublicDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? ProfilePictureUrl { get; set; }
        public DateTime JoinAt { get; set; } // CreatedAt of ApplicationUser
    }
}