using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Shared.Dto_s.Integration.Paymob.Webhook
{
    public class PaymobWebhookResultDto
    {
        public bool IsValid { get; set; } // الـ HMAC سليم ولا لأ
        public bool IsSuccess { get; set; } // الدفع نجح ولا فشل
        public int BookingId { get; set; }
        public string TransactionId { get; set; } = string.Empty;
    }
}
