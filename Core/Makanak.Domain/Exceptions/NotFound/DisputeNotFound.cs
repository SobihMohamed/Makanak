using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Domain.Exceptions.NotFound
{
    public class DisputeNotFound(int id) : NotFoundException_Base($"Dispute With booking Id : {id} Not Found")
    {
    }
}
