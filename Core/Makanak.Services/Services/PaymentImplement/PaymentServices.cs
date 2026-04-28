using Makanak.Abstraction.IServices.PaymentService;
using Makanak.Domain.Exceptions;
using Makanak.Shared.Common.Settings;
using Makanak.Shared.Dto_s.Integration;
using Makanak.Shared.Dto_s.Integration.Paymob;
using Makanak.Shared.Dto_s.Integration.Paymob.Webhook;
using Makanak.Shared.Dto_s.Payment;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Makanak.Services.Services.PaymentImplement
{
    public class PaymentServices : IPaymentService
    {
        private readonly PaymobSettings _paymobSettings;
        private readonly HttpClient _httpClient;

        public PaymentServices(IOptions<PaymobSettings> paymobSettings, HttpClient httpClient)
        {
            _paymobSettings = paymobSettings.Value;
            _httpClient = httpClient;
        }

        public async Task<BookingPaymentDto> CreatePaymentIntentAsync(PaymentIntentInputDto input)
        {
            // 1. price in cents
            var amountInCents = (int)(input.AmountToPayOnline * 100);

            // 2. we need to prepare the payload for Paymob Intention API
            var paymobPayload = new PaymobIntentionRequestDto
            {
                Amount = amountInCents,
                Currency = "EGP",
                SpecialReference = input.BookingId.ToString(),

                PaymentMethods = new List<int>
                {
                    _paymobSettings.CardIntegrationId,
                    _paymobSettings.WalletIntegrationId
                },

                Items = new List<PaymobItem>
                {
                    new PaymobItem
                    {
                        Name = "Makanak Booking Commission",
                        Amount = amountInCents,
                        Description = $"Platform fee for booking: {input.PropertyTitle} ({input.TotalDays} nights)",
                        Quantity = 1
                    }
                },

                BillingData = new PaymobBillingData
                {
                    FirstName = input.TenantFirstName,
                    LastName = input.TenantLastName,
                    Email = input.TenantEmail,
                    PhoneNumber = input.TenantPhoneNumber
                }
            };

            // If Apple Pay is available, add it
            if (_paymobSettings.ApplePayIntegrationId.HasValue)
            {
                paymobPayload.PaymentMethods.Add(_paymobSettings.ApplePayIntegrationId.Value);
            }

            // 3. get the client secret from Paymob Intention API
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", _paymobSettings.SecretKey);

            // Send the request to Paymob Intention API
            var response = await _httpClient.PostAsJsonAsync(_paymobSettings.IntentionApiUrl, paymobPayload);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new BadRequestException($"Paymob Intention API Failed: {errorContent}");
            }

            // 4. get the response and deserialize it
            var paymobResponse = await response.Content.ReadFromJsonAsync<PaymobIntentionResponse>();

            if (paymobResponse == null || string.IsNullOrEmpty(paymobResponse.ClientSecret))
            {
                throw new BadRequestException("Paymob Intention API returned empty response.");
            }

            // 5. return the BookingPaymentDto with the client secret and intention ID
            return new BookingPaymentDto
            {
                PaymentIntentId = paymobResponse.IntentionId,
                ClientSecret = paymobResponse.ClientSecret,
                TotalAmount = input.AmountToPayOnline,
                Status = "pending"
            };
        }
        public class PaymobIntentionResponse
    {
        [JsonPropertyName("client_secret")]
        public string ClientSecret { get; set; } = string.Empty;

        [JsonPropertyName("id")]
        public string IntentionId { get; set; } = string.Empty;
    }

        public async Task<bool> RefundTransactionAsync(string transactionId, decimal amountToRefund)
        {
            if (amountToRefund <= 0) return true;

            var amountInCents = (int)(amountToRefund * 100);

            // Prepare the payload for the refund request
            var refundPayload = new
            {
                transaction_id = transactionId,
                amount_cents = amountInCents
            };

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", _paymobSettings.SecretKey);

            // Send the refund request to Paymob
            var response = await _httpClient.PostAsJsonAsync(_paymobSettings.RefundApiUrl, refundPayload);

            return response.IsSuccessStatusCode;
        }

        public async Task<PaymobWebhookResultDto> ProcessPaymobWebhookAsync(string jsonPayload, string hmac)
        {
            var result = new PaymobWebhookResultDto { IsValid = false };

            try
            {
                using var jsonDoc = JsonDocument.Parse(jsonPayload);
                var root = jsonDoc.RootElement;

                if (!root.TryGetProperty("obj", out var objElement))
                    return result; // Invalid format

                // 1. حساب الـ HMAC
                bool isValidSignature = VerifyPaymobHmac(objElement, hmac, _paymobSettings.HmacSecret);

                if (!isValidSignature)
                    return result; // Unauthorized

                result.IsValid = true;
                result.IntentionId = objElement.GetProperty("intention").GetString() ?? "";
                result.TransactionId = objElement.GetProperty("id").GetRawText();
                result.IsSuccess = objElement.GetProperty("success").GetBoolean();

                return result;
            }
            catch
            {
                return result;
            }
        }

        // 💡 ميثود الـ VerifyPaymobHmac هتنقلها هنا جوه الـ PaymentServices وتبقى Private (مكانها الطبيعي)
        private bool VerifyPaymobHmac(JsonElement obj, string receivedHmac, string hmacSecret)
        {
            // Helper function to safely get string representation matching Paymob's format
            string GetValue(string propName)
            {
                if (obj.TryGetProperty(propName, out var prop))
                {
                    if (prop.ValueKind == JsonValueKind.True) return "true";
                    if (prop.ValueKind == JsonValueKind.False) return "false";
                    return prop.ToString() ?? "";
                }
                return "";
            }

            string GetNestedValue(string parent, string child)
            {
                if (obj.TryGetProperty(parent, out var parentProp) && parentProp.ValueKind == JsonValueKind.Object)
                {
                    if (parentProp.TryGetProperty(child, out var childProp))
                        return childProp.ToString() ?? "";
                }
                return "";
            }

            // ⚠️ الترتيب ده إجباري من بيموب (Lexicographical Order)
            var sb = new StringBuilder();
            sb.Append(GetValue("amount_cents"));
            sb.Append(GetValue("created_at"));
            sb.Append(GetValue("currency"));
            sb.Append(GetValue("error_occured"));
            sb.Append(GetValue("has_parent_transaction"));
            sb.Append(GetValue("id"));
            sb.Append(GetValue("integration_id"));
            sb.Append(GetValue("is_3d_secure"));
            sb.Append(GetValue("is_auth"));
            sb.Append(GetValue("is_capture"));
            sb.Append(GetValue("is_refunded"));
            sb.Append(GetValue("is_standalone_payment"));
            sb.Append(GetValue("is_voided"));
            sb.Append(GetNestedValue("order", "id"));
            sb.Append(GetValue("owner"));
            sb.Append(GetValue("pending"));
            sb.Append(GetNestedValue("source_data", "pan"));
            sb.Append(GetNestedValue("source_data", "sub_type"));
            sb.Append(GetNestedValue("source_data", "type"));
            sb.Append(GetValue("success"));

            string concatenatedString = sb.ToString();

            // Hash the string using HMAC-SHA512
            using var hmacSha512 = new HMACSHA512(Encoding.UTF8.GetBytes(hmacSecret));
            byte[] hashBytes = hmacSha512.ComputeHash(Encoding.UTF8.GetBytes(concatenatedString));

            // Convert to lowercase Hex string
            string calculatedHmac = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            // Compare the calculated HMAC with the received one
            return calculatedHmac == receivedHmac.ToLower();
        }
    
    }
}