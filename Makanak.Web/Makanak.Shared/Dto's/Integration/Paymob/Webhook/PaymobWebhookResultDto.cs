using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Shared.Dto_s.Integration.Paymob.Webhook
{
    public class PaymobWebhookResultDto
    {
        public bool IsValid { get; set; } // الـ HMAC سليم ولا لأ
        public bool IsSuccess { get; set; } // الدفع نجح ولا فشل
        public string IntentionId { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
    }
}
