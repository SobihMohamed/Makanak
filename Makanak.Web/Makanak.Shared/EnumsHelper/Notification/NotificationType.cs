

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makanak.Domain.EnumsHelper.Notification
{
    public enum NotificationType
    {
        // --- 1. System & Account ---
        General = 1,            // إشعار عام من الأدمن للكل
        AccountAlert = 2,       // تغيير باسورد، دخول من جهاز جديد

        // --- 2. Booking Cycle  ---
        BookingRequest = 3,     
        BookingApproved = 4,    
        BookingCancelled = 5,   
        BookingCompleted = 6,
        BookingRejected = 7,
        BookingConfirmed =8,

        // --- 3. Payment Cycle (Instapay & Manual) ---
        PaymentReminder = 8,       
        PaymentReceiptUploaded = 9, 
        PaymentApproved = 10,       
        PaymentRejected = 11,
        PaymentFailed = 12,

        // --- 4. Trust & Safety (Disputes) ---
        DisputeOpened = 12,     
        DisputeResolved = 13    
    }
}
