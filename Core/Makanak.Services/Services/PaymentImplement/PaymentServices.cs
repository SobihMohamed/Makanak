using Makanak.Abstraction.IServices.NotificationService;
using Makanak.Abstraction.IServices.PaymentService;
using Makanak.Domain.Contracts.UOW;
using Makanak.Domain.Exceptions;
using Makanak.Domain.Models.BookingEntities;
using Makanak.Services.Services.NotificationImplement;
using Makanak.Shared.Common.Settings;
using Makanak.Shared.Dto_s.Integration.Paymob.Webhook;
using Makanak.Shared.Dto_s.Payment;
using Makanak.Shared.EnumsHelper.Booking;
using Makanak.Shared.HelpersFactory;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public PaymentServices(IOptions<PaymobSettings> paymobSettings, IHttpClientFactory httpClientFactory, IUnitOfWork unitOfWork , INotificationService notificationService)
        {
            _paymobSettings = paymobSettings.Value;
            _httpClient = httpClientFactory.CreateClient("PaymobClient");
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;

        }

        public async Task<BookingPaymentDto> CreatePaymentIntentAsync(PaymentIntentInputDto input)
        {
            var amountInCents = (int)(input.AmountToPayOnline * 100);

            var payload = new
            {
                amount = amountInCents,
                currency = "EGP",
                payment_methods = new[] { _paymobSettings.CardIntegrationId },
                special_reference = input.BookingId.ToString(),
                notification_url = _paymobSettings.NotificationUrl,
                redirection_url = _paymobSettings.RedirectionUrl,
                merchant_order_id = input.BookingId.ToString(),

                billing_data = new
                {
                    // ✅ الاعتماد على الداتا الحقيقية للعميل
                    first_name = input.TenantFirstName ?? "Unknown",
                    last_name = input.TenantLastName ?? "Unknown",
                    email = input.TenantEmail ?? "no-email@makanak.com",
                    phone_number = input.TenantPhoneNumber ?? "N/A",
                    street = "N/A",
                    building = "N/A",
                    apartment = "N/A",
                    floor = "N/A",
                    city = "N/A",
                    state = "N/A",
                    country = "Egypt"
                },
                customer = new
                {
                    first_name = input.TenantFirstName ?? "Unknown",
                    last_name = input.TenantLastName ?? "Unknown",
                    email = input.TenantEmail ?? "no-email@makanak.com"
                },
                items = new[]
                {
                    new
                    {
                        name = "Makanak Booking Commission",
                        amount = amountInCents,
                        description = "Platform fee for booking",
                        quantity = 1
                    }
                }
            };

            // في CreatePaymentIntentAsync
            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://accept.paymob.com/v1/intention/"
            );

            request.Headers.TryAddWithoutValidation(
                "Authorization",
                $"Token {_paymobSettings.SecretKey.Trim()}"
            );

            request.Content = JsonContent.Create(payload);
            // قبل الـ SendAsync مباشرة
            Console.WriteLine($"SECRET KEY: [{_paymobSettings.SecretKey}]");
            Console.WriteLine($"AUTH HEADER: {request.Headers.Authorization}");
            var response = await _httpClient.SendAsync(request);


            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new BadRequestException($"Paymob Intention API Failed: {errorContent}");
            }

            var paymobResponse = await response.Content.ReadFromJsonAsync<PaymobIntentionResponse>();

            if (paymobResponse == null || string.IsNullOrEmpty(paymobResponse.ClientSecret))
                throw new BadRequestException("Paymob Intention API returned empty response.");

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

        public async Task<PaymobWebhookResultDto> ProcessPaymobWebhookAsync(string jsonPayload, string hmac)
        {
            var result = new PaymobWebhookResultDto { IsValid = false };

            try
            {
                using var jsonDoc = JsonDocument.Parse(jsonPayload);
                var root = jsonDoc.RootElement;

                if (!root.TryGetProperty("obj", out var objElement))
                    return result;

                // 1. التحقق من الـ HMAC (مهم جداً)
                bool isValidSignature = VerifyPaymobHmac(objElement, hmac, _paymobSettings.HmacSecret);
                if (!isValidSignature)
                    throw new BadRequestException("Invalid Secure Operation in HMAC.");

                result.IsValid = true;
                string bookingIdStr = "";

                if (objElement.TryGetProperty("order", out var order) &&
                    order.TryGetProperty("merchant_order_id", out var merchantOrderId) &&
                    merchantOrderId.ValueKind != JsonValueKind.Null)
                {
                    bookingIdStr = merchantOrderId.ToString();
                }

                if (string.IsNullOrEmpty(bookingIdStr))
                    return result;

                bookingIdStr = bookingIdStr.Replace("booking_", "");
                result.BookingId = int.Parse(bookingIdStr);

                result.TransactionId = objElement.TryGetProperty("id", out var idProp) ? idProp.ToString() : "";
                result.IsSuccess = objElement.TryGetProperty("success", out var succProp) && succProp.GetBoolean();

                return result;
            }
            catch (Exception ex)
            {
                throw new BadRequestException($"Error processing Paymob webhook: {ex.Message}");
            }
        }
        public async Task<bool> ConfirmManualRefundAsync(int bookingId)
        {
            var bookingRepo = _unitOfWork.GetRepo<Booking, int>();
            var booking = await bookingRepo.GetByIdAsync(bookingId);

            if (booking == null) return false;

            // لازم يكون الحجز فعلاً طالب استرداد
            if (booking.Status != BookingStatus.RefundRequested)
                throw new BadRequestException("This booking is not pending a refund.");

            // الأدمن بيكد إن الفلوس رجعت خلاص
            booking.Status = BookingStatus.Refunded; 
            booking.IsRefunded = true;

            bookingRepo.Update(booking);
            var result = await _unitOfWork.SaveChangesAsync();

            if (result > 0)
            {
                // نفرح العميل إن الفلوس اتبعتت لحسابه البنكي
                await _notificationService.SendNotificationAsync(
                    NotificationFactory.RefundStatusNotification(booking.TenantId, true, booking.Id)
                );
            }

            return result > 0;
        }
        public async Task<bool> RejectManualRefundAsync(int bookingId, string rejectionReason)
        {
            var bookingRepo = _unitOfWork.GetRepo<Booking, int>();
            var booking = await bookingRepo.GetByIdAsync(bookingId);

            if (booking == null) return false;

            if (booking.Status != BookingStatus.RefundRequested)
                throw new BadRequestException("This booking is not pending a refund.");

            // بنكنسل الحجز نهائي بس بنقول إن الفلوس مرجعتش
            booking.Status = BookingStatus.Cancelled;
            booking.IsRefunded = false;
            booking.CancellationReason += $" | Refund Rejected by Admin: {rejectionReason}";

            bookingRepo.Update(booking);
            var result = await _unitOfWork.SaveChangesAsync();

            if (result > 0)
            {
                await _notificationService.SendNotificationAsync(
                    NotificationFactory.RefundRejected(booking.TenantId, booking.Id, rejectionReason)
                );
            }

            return result > 0;
        }
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