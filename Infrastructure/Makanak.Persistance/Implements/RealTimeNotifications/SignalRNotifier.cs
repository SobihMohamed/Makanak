using Makanak.Abstraction.IServices.RealTimeNotifier;
using Makanak.Persistance.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Makanak.Persistance.Implements.RealTimeNotifications
{
    public class SignalRNotifier(IHubContext<NotificationHub> hubContext) : IRealTimeNotifier
    {
        public async Task SendToUserAsync(string userId, string method, object data)
        {
            if (!string.IsNullOrEmpty(userId))
            {   
                await hubContext.Clients.User(userId).SendAsync(method, data);
            }
        }
    }
}