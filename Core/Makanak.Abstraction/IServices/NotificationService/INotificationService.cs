using Makanak.Shared.Dto_s.Notification;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Makanak.Abstraction.IServices.NotificationService
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(string userId, bool? isReadonly = null);

        Task<int> GetUnreadCountAsync(string userId);

        Task<bool> MarkAsReadAsync(int notificationId, string userId);

        Task<bool> SendNotificationAsync(CreateNotificationDto model);
    }
}
