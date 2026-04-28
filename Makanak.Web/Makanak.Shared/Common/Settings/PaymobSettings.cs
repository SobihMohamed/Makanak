namespace Makanak.Shared.Common.Settings
{
    public class PaymobSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public string PublicKey { get; set; } = string.Empty;
        public int WalletIntegrationId { get; set; }
        public int CardIntegrationId { get; set; }
        public int? ApplePayIntegrationId { get; set; }

        public string HmacSecret { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
        public string IntentionApiUrl { get; set; } = string.Empty;
        public string RefundApiUrl { get; set; } = string.Empty;
    }
}