using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Shared.Dto_s.Integration.Paymob.Webhook
{
    public class PaymobWebhookPayload
    {
        public string type { get; set; }
        public PaymobWebhookObj obj { get; set; }
    }
}
