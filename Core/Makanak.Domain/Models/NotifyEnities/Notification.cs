using Makanak.Domain.EnumsHelper.Notification;
using Makanak.Domain.Models.Identity;

namespace Makanak.Domain.Models.NotifyEnities
{
    public class Notification : BaseEntity<int>
    {

        // Reciver of the notification
        public string UserId { get; set; } = null!;
        public virtual ApplicationUser ApplicationUser { get; set; } = null!;
        // If Null: It means a System Notification (e.g., "Your account is active").
        public string? SenderId { get; set; }
        public virtual ApplicationUser? Sender { get; set; }
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public NotificationType NotificationType { get; set; }
        // Stores the ID of the related entity (BookingId, PropertyId, etc.).
        // Used by Frontend to redirect the user when they click the notification.
        // Example: If Type is 'BookingRequest', ReferenceId = "55" (The Booking ID)
        public string? ReferenceId { get; set; }
        public bool IsRead { get; set; } = false;

    }
}
