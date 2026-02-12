using Makanak.Abstraction.IServices.Manager;
using Makanak.Domain.EnumsHelper.User;
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

        [HttpGet("dispute-reasons")]
        public ActionResult<ApiResponse<IEnumerable<EnumLookupDto>>> GetDisputeReasons()
            => Success(EnumResolver.GetEnumList<DisputeReason>());

        [HttpGet("booking-statuses")]
        public ActionResult<ApiResponse<IEnumerable<EnumLookupDto>>> GetBookingStatuses()
            => Success(EnumResolver.GetEnumList<BookingStatus>());

        [HttpGet("dispute-statuses")]
        public ActionResult<ApiResponse<IEnumerable<EnumLookupDto>>> GetDisputeStatuses()
            => Success(EnumResolver.GetEnumList<DisputeStatus>());

        [HttpGet("property-statuses")]
        public ActionResult<ApiResponse<IEnumerable<EnumLookupDto>>> GetPropertyStatuses()
            => Success(EnumResolver.GetEnumList<PropertyStatus>());

        [HttpGet("user-statuses")]
        public ActionResult<ApiResponse<IEnumerable<EnumLookupDto>>> GetUserStatuses()
            => Success(EnumResolver.GetEnumList<UserStatus>());

        [HttpGet("user-types")]
        public ActionResult<ApiResponse<IEnumerable<EnumLookupDto>>> GetUserTypes()
            => Success(EnumResolver.GetEnumList<UserTypes>());

    }
}