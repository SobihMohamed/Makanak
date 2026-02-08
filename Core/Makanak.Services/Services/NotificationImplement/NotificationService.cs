using AutoMapper;
using Makanak.Abstraction.IServices.NotificationService;
using Makanak.Abstraction.IServices.RealTimeNotifier;
using Makanak.Domain.Contracts.UOW;
using Makanak.Domain.Models.NotifyEnities;
using Makanak.Services.Specifications.NotificationSpec;
using Makanak.Shared.Dto_s.Notification;

namespace Makanak.Services.Services.NotificationImplement
{
    public class NotificationService(IUnitOfWork unitOfWork , IRealTimeNotifier realTimeNotifier , IMapper mapper) 
        : INotificationService
    {
        public async Task<int> GetUnreadCountAsync(string userId)
        {
            // get notify repo 
            var notificationRepo = unitOfWork.GetRepo<Notification, int>();
            
            // get notify spec 
            var spec = new NotificationSpecefication(userId, false);

            // get notification count unread 
            var unReadNot = await notificationRepo.CountAsync(spec);

            return unReadNot;
        }

        public async Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(string userId, bool? isReadonly = null)
        {
            // get notify repo 
            var notificationRepo = unitOfWork.GetRepo<Notification, int>();

            // get notify spec 
            var spec = new NotificationSpecefication(userId ,isReadonly);

            // get notifications
            var notifications = await notificationRepo.GetAllWithSpecificationAsync(spec);

            var notificationDtos = mapper.Map<IEnumerable<NotificationDto>>(notifications);

            return notificationDtos;
        }

        public async Task<bool> MarkAsReadAsync(int notificationId, string userId)
        {
            var notificationRepo = unitOfWork.GetRepo<Notification, int>();
            var notification = await notificationRepo.GetByIdAsync(notificationId);

            if (notification == null || notification.UserId != userId)
                return false; 

            if (notification.IsRead) return true;

            notification.IsRead = true;
            notificationRepo.Update(notification);

            var result = await unitOfWork.SaveChangesAsync();

            return result > 0;
        }

        public async Task<bool> SendNotificationAsync(CreateNotificationDto createNotificationDto)
        {
            var notification = mapper.Map<Notification>(createNotificationDto);

            var notificationRepo = unitOfWork.GetRepo<Notification, int>();
            notificationRepo.AddAsync(notification);
            await unitOfWork.SaveChangesAsync();

            var notificationDto = mapper.Map<NotificationDto>(notification);

            await realTimeNotifier.SendToUserAsync(createNotificationDto.UserId, "ReceiveNotification", notificationDto);

            return true;
        }
    }
}
