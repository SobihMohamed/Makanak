

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
        PaymentReminder = 9,       
        PaymentReceiptUploaded = 10, 
        PaymentApproved = 11,       
        PaymentRejected = 12,
        PaymentFailed = 13,

        // --- 4. Trust & Safety (Disputes) ---
        DisputeOpened = 14,     
        DisputeResolved = 15 ,
        DisputeCancelled = 16,

        // --- 5. Reviews
        ReviewReceived = 17,

        // --- 6. Admin
        NewPropertyListing = 18,       // عقار جديد مستني موافقة
        DocumentVerificationRequest = 19, // يوزر رفع هوية ومستني توثيق

        PropertyStatusChanged = 20,
        UserStatusChanged = 21,      

        // --- 7. Reminder
        BookingReminder = 22,

        // --- Background services
        PaymentWarning = 23,
        CheckInReminder = 24


    }
}
