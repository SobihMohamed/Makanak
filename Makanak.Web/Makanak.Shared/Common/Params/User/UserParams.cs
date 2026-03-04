using Makanak.Domain.EnumsHelper.User;
using Makanak.Shared.Common.Params;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Shared.Common.Params.User
{
    public class UserParams : QueryParams
    {
        public UserStatus? Status { get; set; }
        public UserTypes? Type { get; set; }
    }
}
