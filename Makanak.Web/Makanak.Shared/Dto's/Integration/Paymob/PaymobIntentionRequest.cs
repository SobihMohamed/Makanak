using Makanak.Shared.Dto_s.Integration.Paymob;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Makanak.Shared.Dto_s.Integration
{
    public class PaymobIntentionRequestDto
    {
        [JsonPropertyName("amount")]
        public int Amount { get; set; } // Note: Paymob strictly takes cents! (e.g., 100 EGP = 10000)

        [JsonPropertyName("currency")]
        public string Currency { get; set; } = "EGP";

        [JsonPropertyName("payment_methods")]
        public List<int> PaymentMethods { get; set; } = new();

        [JsonPropertyName("items")]
        public List<PaymobItem> Items { get; set; } = new();

        [JsonPropertyName("billing_data")]
        public PaymobBillingData BillingData { get; set; } = new();

        [JsonPropertyName("special_reference")]
        public string SpecialReference { get; set; } // Your internal Booking ID in Makanak

        [JsonPropertyName("notification_url")]
        public string NotificationUrl { get; set; } // Webhook URL (Server-to-Server)

        [JsonPropertyName("redirection_url")]
        public string RedirectionUrl { get; set; } // Where the user goes after payment (React App)
    }
}
