using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Domain.Exceptions
{
    public class UnauthorizedException() : Exception("You are not authorized to access this resource.")
    {
    }
}
