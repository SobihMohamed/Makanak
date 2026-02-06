using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Domain.Exceptions.NotFound
{
    public class NotificationNotFound(int id) : NotFoundException_Base("Notification Not Found")
    {
    }
}
