using Makanak.Abstraction.IServices.Manager;
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
        [HttpGet("my-properties")]
        [Authorize(Roles = "Owner")]
        public async Task<ActionResult<ApiResponse<Pagination<PropertyDto>>>> GetMyProperties([FromQuery] PropertyParams propertyParams)
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await serviceManager.PropertyServices.GetPropertiesByOwnerIdAsync(ownerId!, propertyParams);

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
    }
}
