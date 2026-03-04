using Makanak.Abstraction.IServices.Manager;
using Makanak.Shared.Dto_s.Notification;
using Makanak.Shared.Responses;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Makanak.Presentation.Controllers.Notification_Controller
{
    public class NotificationsController(IServiceManager serviceManager) : AppBaseController
    {
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<NotificationDto>>>> GetUserNotifications([FromQuery] bool? isRead)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // isRead = null  -> All 
            // isRead = false -> Not Read
            // isRead = true  -> Read
            var notifications = await serviceManager.NotificationService.GetUserNotificationsAsync(userId!, isRead);

            return Success(notifications, "Notifications retrieved successfully");
        }

        [HttpGet("count")]
        public async Task<ActionResult<ApiResponse<NotificationCountDto>>> GetUnreadCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var count = await serviceManager.NotificationService.GetUnreadCountAsync(userId);

            return Success(new { UnreadCount = count });
        }

        [HttpPut("{id}/read")]
        public async Task<ActionResult<ApiResponse<string>>> MarkAsRead(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await serviceManager.NotificationService.MarkAsReadAsync(id, userId!);

           if (!result)
               return BadRequestError("Failed to mark notification as read, or it doesn't belong to you.");

           return Success("Notification marked as read");
        }

        //[HttpPost("test-send")] // 👈 دي هنمسحها بعدين
        //public async Task<IActionResult> TestSend([FromBody] CreateNotificationDto dto)
        //{
        //    // بنجيب الـ Id بتاعك أنت عشان تبعت لنفسك وتشوف النتيجة
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        //    // بنعدل الـ DTO عشان يروح لليوزر الحالي
        //    dto.UserId = userId;

        //    // بننادي السرفيس اللي تعبنا فيها
        //    var result = await serviceManager.NotificationService.SendNotificationAsync(dto);

        //    return Success("Notification Simulated Successfully!");
        //}
    }
}
