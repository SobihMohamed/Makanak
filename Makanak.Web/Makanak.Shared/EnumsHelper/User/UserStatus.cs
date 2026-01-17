using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makanak.Domain.EnumsHelper.User
{
    public enum UserStatus
    {
        New = 1,       // default status when user register
        Pending = 2,   // waiting for admin to verify identity
        Active = 3,    // verified
        Rejected = 4,  // Not verified
        Banned = 5     // banned due to violations
    }
}
