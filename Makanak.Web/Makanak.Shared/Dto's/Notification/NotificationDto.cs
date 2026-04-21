using Makanak.Domain.EnumsHelper.Notification;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Shared.Dto_s.Notification
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string? ReferenceId { get; set; } 
        public NotificationType NotificationType { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public string? About {  get; set; } // from Reference relation
        public string? SenderName { get; set; } = "System"; // from sender relation 
    }
}
