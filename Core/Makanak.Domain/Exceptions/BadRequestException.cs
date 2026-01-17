using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Domain.Exceptions
{
    public class BadRequestException: Exception
    {
        public readonly IEnumerable<string>? _errors;

        public BadRequestException(string message ,IEnumerable<string>? errors = null) 
            : base(message)
        {
            _errors = errors;
        }

    }
}