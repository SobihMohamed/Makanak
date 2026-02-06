using Makanak.Domain.Models.NotifyEnities;

namespace Makanak.Services.Specifications.NotificationSpec
{
    public class NotificationSpecefication : BaseSpecifications<Notification,int>
    {
        public NotificationSpecefication(string userId, bool? isRead)
        : base(n =>
            n.UserId == userId &&
            (!isRead.HasValue || n.IsRead == isRead.Value)
          )
        {
            AddInclude(n => n.Sender);
            AddOrderByDesc(n => n.CreatedAt);
        }
        public NotificationSpecefication(string userId , bool isRead)
            : base(n => n.UserId == userId && n.IsRead == isRead)
        {
        }
    }
}