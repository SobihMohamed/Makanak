

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

        // --- 5. Reviews
        ReviewReceived = 16,

        // --- 6. Admin
        NewPropertyListing = 17,       // عقار جديد مستني موافقة
        DocumentVerificationRequest = 18, // يوزر رفع هوية ومستني توثيق

        PropertyStatusChanged = 19,
        UserStatusChanged = 20,      

        // --- 7. Reminder
        BookingReminder = 21,

        // --- Background services
        PaymentWarning = 22,
        CheckInReminder = 23


    }
}
