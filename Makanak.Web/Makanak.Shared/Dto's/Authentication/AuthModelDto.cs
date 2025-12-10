using Makanak.Shared.Dto_s.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Shared.Dto_s.Authentication
{
    public class AuthModelDto
    {
        public string Message { get; set; } = string.Empty;
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Token { get; set; } = null!;
        public DateTime ExpiresOn { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
}
