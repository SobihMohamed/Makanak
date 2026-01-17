using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Domain.Exceptions
{
    public abstract class NotFoundException_Base(string message) 
        : Exception(message)
    {
    }
}