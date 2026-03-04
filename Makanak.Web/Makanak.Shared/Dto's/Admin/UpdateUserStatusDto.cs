using Makanak.Domain.EnumsHelper.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Makanak.Shared.Dto_s.Admin
{
    public class UpdateUserStatusDto
    {
        public string UserId { get; set; } = null!;
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserStatus NewStatus { get; set; }
        public string? RejectedReason { get; set; }
    }
}
