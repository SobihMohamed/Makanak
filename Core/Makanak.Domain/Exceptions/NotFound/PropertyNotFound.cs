using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Domain.Exceptions.NotFound
{
    public class PropertyNotFound(int id) : NotFoundException_Base($"Property with ID {id} Not Found")
    {
    }
}
