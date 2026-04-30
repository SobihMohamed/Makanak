using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Makanak.Shared.Dto_s.Integration.Paymob
{
    public class PaymobItem
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("amount")]
        public int Amount { get; set; } // In cents

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
    }
}
