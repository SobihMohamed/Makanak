using Makanak.Abstraction.IServices.Manager;
using Makanak.Shared.Common.Params.Property_Params;
using Makanak.Shared.Dto_s.Property;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Makanak.Presentation.Controllers.Property_Controller
{
    public class PropertyController(IServiceManager serviceManager) : AppBaseController
    {
        #region public EndPoints
        [HttpGet]
        public async Task<IActionResult> GetProperties([FromQuery] PropertyParams propertyParams)
        {
            var properties = await serviceManager.PropertyServices.GetAllAvailablePropertiesAsync(propertyParams);
            return Success(properties);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPropertyById(int id)
        {
            var property = await serviceManager.PropertyServices.GetPropertyDetailByIdAsync(id);
            return Success(property);
        }
        #endregion

        #region Owner EndPoints
        [HttpGet("my-properties")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> GetMyProperties()
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var properties = await serviceManager.PropertyServices.GetPropertiesByOwnerId(ownerId!);
            return Success(properties);
        }

        [HttpPost]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> CreateProperty([FromForm] CreatePropertyDto createPropertyDto)
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var propertyCreated = await serviceManager.PropertyServices.CreatePropertyAsync(createPropertyDto,ownerId!);
            
            return Created(propertyCreated, "Property Created Successfully");
        }

        [Authorize(Roles = "Owner")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProperty(int id,[FromForm] UpdatePropertyDto updatePropertyDto)
        {

            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var updatedProperty =  await serviceManager.PropertyServices.UpdatePropertyAsync(id ,updatePropertyDto, ownerId!);

            return Success(updatedProperty, "Property Updated Successfully");
        }

        [Authorize(Roles = "Owner")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProperty(int id)
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var res = await serviceManager.PropertyServices.DeletePropertyAsync(id, ownerId!);
            
            if(!res)
                return BadRequest("Unable to Delete Property");

            return Success("Property Deleted Successfully");
        }
        #endregion
    }
}
