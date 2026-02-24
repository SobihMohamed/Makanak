using Makanak.Abstraction.IServices.Manager;
using Makanak.Domain.EnumsHelper.User;
using Makanak.Shared.Common;
using Makanak.Shared.Dto_s.Governorate;
using Makanak.Shared.Dto_s.Lookup;
using Makanak.Shared.Dto_s.Property;
using Makanak.Shared.EnumsHelper;
using Makanak.Shared.EnumsHelper.Booking;
using Makanak.Shared.EnumsHelper.Dispute;
using Makanak.Shared.EnumsHelper.Property;
using Makanak.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Presentation.Controllers.Lookup_Controller
{
    [AllowAnonymous]
    public class LookupController(IServiceManager serviceManager) : AppBaseController
    {
        [HttpGet("governorates")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<GovernorateDto>>>> GetGovernorates()
        {
            var result = await serviceManager.GovernorateService.GetAllGovernoratesAsync();
            return Success(result);
        }

        [HttpGet("amenities")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<AmenityDto>>>> GetAmenities()
        {
            var result = await serviceManager.AmenityService.GetAllAmenitiesAsync();
            return Success(result);
        }
       
        
        // Enums 
        [HttpGet("property-types")]
        public ActionResult<ApiResponse<IEnumerable<EnumLookupDto>>> GetPropertyTypes()
            => Success(EnumResolver.GetEnumList<PropertyType>());

        [HttpGet("booking-statuses")]
        public ActionResult<ApiResponse<IEnumerable<EnumLookupDto>>> GetBookingStatuses()
            => Success(EnumResolver.GetEnumList<BookingStatus>());

        [HttpGet("owner-dispute-reasons")]
        public ActionResult<ApiResponse<IEnumerable<EnumLookupDto>>> GetOwnerDisputeReasons()
        {
            var allReasons = EnumResolver.GetEnumList<DisputeReason>();

            var ownerAllowedIds = new[]
            {
                (int)DisputeReason.DamageToProperty,
                (int)DisputeReason.Other
            };

            var ownerReasons = allReasons.Where(r => ownerAllowedIds.Contains(r.Id));
            return Success(ownerReasons);
        }

        [HttpGet("tenant-dispute-reasons")]
        public ActionResult<ApiResponse<IEnumerable<EnumLookupDto>>> GetTenantDisputeReasons()
        {
            var allReasons = EnumResolver.GetEnumList<DisputeReason>();

            var tenantAllowedIds = new[]
            {
                (int)DisputeReason.PropertyNotAsDescribed,
                (int)DisputeReason.CheckInIssue,
                (int)DisputeReason.CleanlinessIssue,
                (int)DisputeReason.HostUnreachable,
                (int)DisputeReason.Other
            };

            var tenantReasons = allReasons.Where(r => tenantAllowedIds.Contains(r.Id));
            return Success(tenantReasons);
        }

        [HttpGet("all-dispute-reasons")]
        public ActionResult<ApiResponse<IEnumerable<EnumLookupDto>>> GetAllDisputeReasons()
            =>Success(EnumResolver.GetEnumList<DisputeReason>());

        [HttpGet("property-statuses")]
        public ActionResult<ApiResponse<IEnumerable<EnumLookupDto>>> GetPropertyStatuses()
            => Success(EnumResolver.GetEnumList<PropertyStatus>());

        [HttpGet("user-statuses")]
        public ActionResult<ApiResponse<IEnumerable<EnumLookupDto>>> GetUserStatuses()
            => Success(EnumResolver.GetEnumList<UserStatus>());

        [HttpGet("user-types")]
        public ActionResult<ApiResponse<IEnumerable<EnumLookupDto>>> GetUserTypes()
            => Success(EnumResolver.GetEnumList<UserTypes>());

        [HttpGet("sorting-options")]
        public ActionResult<ApiResponse<IEnumerable<EnumLookupDto>>> GetSortingOptions()
            => Success(EnumResolver.GetEnumList<SortingOptionsEnum>());
    }
}