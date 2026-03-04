using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Abstraction.IServices.RealTimeNotifier
{
    public interface IRealTimeNotifier
    {
        Task SendToUserAsync(string userId, string method, object data);
    }
}
