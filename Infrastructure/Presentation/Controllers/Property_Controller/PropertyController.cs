using Makanak.Abstraction.IServices.Manager;
using Makanak.Abstraction.IServices.PropertyService;
using Makanak.Shared.Common;
using Makanak.Shared.Common.Params.Property_Params;
using Makanak.Shared.Dto_s.Property;
using Makanak.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Makanak.Presentation.Controllers.Property_Controller
{
    public class PropertyController(IServiceManager serviceManager) : AppBaseController
    {
        #region public EndPoints
        [HttpGet]
        public async Task<ActionResult<ApiResponse<Pagination<PropertyDto>>>> GetProperties([FromQuery] PropertyParams propertyParams)
        {
            var properties = await serviceManager.PropertyServices.GetAllAvailablePropertiesAsync(propertyParams);
            return Success(properties);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<PropertyDto>>> GetPropertyById(int id)
        {
            var property = await serviceManager.PropertyServices.GetPropertyDetailByIdAsync(id);
            return Success(property);
        }
        #endregion

        #region Owner EndPoints
        [Authorize(Roles = "Owner")]
        [HttpGet("my-properties")]
        public async Task<ActionResult<ApiResponse<Pagination<PropertyDto>>>> GetMyProperties([FromQuery] OwnerPropertyParams ownerParams)
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await serviceManager.PropertyServices.GetPropertiesByOwnerIdAsync(ownerId!, ownerParams);

            return Success(result);
        }

        [HttpPost]
        [Authorize(Roles = "Owner")]
        public async Task<ActionResult<ApiResponse<PropertyDto>>> CreateProperty([FromForm] CreatePropertyDto createPropertyDto)
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var propertyCreated = await serviceManager.PropertyServices.CreatePropertyAsync(createPropertyDto,ownerId!);
            
            return Created(propertyCreated, "Property Created Successfully");
        }

        [Authorize(Roles = "Owner")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<PropertyDto>>> UpdateProperty(int id, [FromForm] UpdatePropertyDto updatePropertyDto)
        {

            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var updatedProperty =  await serviceManager.PropertyServices.UpdatePropertyAsync(id ,updatePropertyDto, ownerId!);

            return Success(updatedProperty, "Property Updated Successfully");
        }

        [Authorize(Roles = "Owner")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteProperty(int id)
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var res = await serviceManager.PropertyServices.DeletePropertyAsync(id, ownerId!);
            
            if(!res)
                return BadRequestError("Unable to Delete Property");

            return Success("Property Deleted Successfully");
        }
        #endregion

        #region Admin
        [Authorize(Roles = "Admin")]
        [HttpGet("admin-all")]
        public async Task<ActionResult<ApiResponse<Pagination<AdminPropertyDto>>>> GetAllPropertiesForAdmin([FromQuery] AdminPropertyParams adminPropertyParams)
        {
            var properties = await serviceManager.PropertyServices.GetPropertiesForAdminAsync(adminPropertyParams);

            return Success(properties);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin/{id}")]
        public async Task<ActionResult<ApiResponse<AdminPropertyDetailDto>>> GetPropertyByIdForAdmin(int id)
        {
            var property = await serviceManager.PropertyServices.GetPropertyByIdForAdminAsync(id);
            return Success(property);
        }
        #endregion
    }
}
