namespace Makanak.Shared.Common.Settings
{
    public class PaymobSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public string PublicKey { get; set; } = string.Empty;
        public int CardIntegrationId { get; set; }
        public string BaseUrl { get; set; } = string.Empty;
        public string NotificationUrl { get; set; } = string.Empty;
        public string RedirectionUrl {  get; set; } = string.Empty;
        public string HmacSecret { get; set; } = string.Empty;
        public string IntentionApiUrl { get; set; } = string.Empty;
    }
}