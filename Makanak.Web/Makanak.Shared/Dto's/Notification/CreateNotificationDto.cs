using Makanak.Domain.EnumsHelper.Notification;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Shared.Dto_s.Notification
{
    public class CreateNotificationDto
    {
        public string UserId { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;

        public NotificationType NotificationType { get; set; }
        public string? ReferenceId { get; set; }
    }
}
