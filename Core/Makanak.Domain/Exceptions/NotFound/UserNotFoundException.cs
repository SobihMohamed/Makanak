using System;

namespace Makanak.Domain.Exceptions.NotFound
{
    public class UserNotFoundException : NotFoundException_Base
    {
        private UserNotFoundException(string message) : base(message)
        {
        }

        public static UserNotFoundException ByEmail(string email)
        {
            return new UserNotFoundException($"User with email '{email}' was not found.");
        }

        public static UserNotFoundException ById(string id)
        {
            return new UserNotFoundException($"User with ID '{id}' was not found.");
        }
    }
}