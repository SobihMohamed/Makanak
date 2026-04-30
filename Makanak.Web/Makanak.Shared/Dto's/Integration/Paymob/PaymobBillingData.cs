using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Makanak.Shared.Dto_s.Integration.Paymob
{
    public class PaymobBillingData
    {
        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("phone_number")]
        public string PhoneNumber { get; set; }

        // Dummy fields required by Paymob
        [JsonPropertyName("apartment")] public string Apartment { get; set; } = "dummy";
        [JsonPropertyName("street")] public string Street { get; set; } = "dummy";
        [JsonPropertyName("building")] public string Building { get; set; } = "dummy";
        [JsonPropertyName("city")] public string City { get; set; } = "dummy";
        [JsonPropertyName("country")] public string Country { get; set; } = "EG";
        [JsonPropertyName("floor")] public string Floor { get; set; } = "dummy";
        [JsonPropertyName("state")] public string State { get; set; } = "dummy";
    }
}
