using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makanak.Domain.EnumsHelper.User
{
    public enum UserStatus
    {
        Pending = 1,   //  (Default)
        Active = 2,     
        Rejected = 3,  // Not verified
        Banned = 4     // banned due to violations
    }
}
