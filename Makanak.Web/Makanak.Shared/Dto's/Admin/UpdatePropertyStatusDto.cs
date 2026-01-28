using Makanak.Shared.EnumsHelper.Property;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Makanak.Shared.Dto_s.Admin
{
    public class UpdatePropertyStatusDto
    {
        public int PropertyId { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PropertyStatus NewStatus { get; set; }
        public string? RejectedReason { get; set; }
    }
}
