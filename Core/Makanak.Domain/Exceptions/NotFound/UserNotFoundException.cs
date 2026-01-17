using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Domain.Exceptions.NotFound
{
    public class UserNotFoundException(string email) 
        : NotFoundException_Base($"User with email {email} was not found.")
    {
    }
}
