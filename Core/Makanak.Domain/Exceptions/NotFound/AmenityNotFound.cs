using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Domain.Exceptions.NotFound
{
    public class AmenityNotFound(int id ) : NotFoundException_Base($"Amenitiy with Id {id} Not Found")
    {
    }
}
