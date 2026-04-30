using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Shared.Dto_s.Integration.Paymob.Webhook
{
    public class PaymobWebhookObj
    {
        public int id { get; set; } // رقم العملية الفعلي (Transaction ID)
        public bool success { get; set; } // نجحت ولا فشلت
        public string intention { get; set; } // رقم النية اللي إحنا مخزنينه (PaymentIntentId)
    }
}
