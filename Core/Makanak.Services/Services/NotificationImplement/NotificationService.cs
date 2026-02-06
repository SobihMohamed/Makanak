using AutoMapper;
using Makanak.Abstraction.IServices.NotificationService;
using Makanak.Abstraction.IServices.RealTimeNotifier;
using Makanak.Domain.Contracts.UOW;
using Makanak.Domain.Exceptions.NotFound;
using Makanak.Domain.Models.Identity;
using Makanak.Domain.Models.NotifyEnities;
using Makanak.Services.Specifications.NotificationSpec;
using Makanak.Shared.Dto_s.Notification;
using System.Reflection;
using System.Windows.Markup;

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
            // get notify repo 
            var notificationRepo = unitOfWork.GetRepo<Notification, int>();

            var notification = await notificationRepo.GetByIdAsync(notificationId);

            if (notification == null)
                throw new NotificationNotFound(notificationId);
            if (notification == null || notification.UserId != userId)
                return false;

            notification.IsRead = true;

            notificationRepo.Update(notification);

            unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SendNotificationAsync(CreateNotificationDto createNotificationDto)
        {
            var notification = mapper.Map<Notification>(createNotificationDto);

            // 2. الحفظ في الداتابيز
            var notificationRepo = unitOfWork.GetRepo<Notification, int>();
            notificationRepo.AddAsync(notification);
            await unitOfWork.SaveChangesAsync();

            // 3. التحويل لـ NotificationDto (عشان نبعته للفرونت)
            // لاحظ: بنستخدم الـ notification اللي لسه محفوظ عشان ناخد الـ ID والوقت الحقيقي
            var notificationDto = mapper.Map<NotificationDto>(notification);

            // 4. الإرسال اللحظي (SignalR)
            await realTimeNotifier.SendToUserAsync(createNotificationDto.UserId, "ReceiveNotification", notificationDto);

            return true;
        }
    }
}
