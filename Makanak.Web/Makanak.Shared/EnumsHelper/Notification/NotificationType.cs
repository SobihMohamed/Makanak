

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

        // --- 3. Payment Cycle (Instapay & Manual) ---
        PaymentReminder = 7,       
        PaymentReceiptUploaded = 8, 
        PaymentApproved = 9,       
        PaymentRejected = 10,       

        // --- 4. Trust & Safety (Disputes) ---
        DisputeOpened = 11,     
        DisputeResolved = 12    
    }
}
