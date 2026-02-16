using Makanak.Shared.Common;
using Makanak.Shared.Common.Params.Property_Params;
using Makanak.Shared.Dto_s.Property;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Abstraction.IServices.PropertyService
{
    public interface IPropertyService
    {
        Task<PropertyDetailDto> CreatePropertyAsync(CreatePropertyDto dto, string ownerId);

        Task<Pagination<PropertyDto>> GetPropertiesByOwnerIdAsync(string ownerId, PropertyParams propertyParams);

        Task<PropertyDetailDto> GetPropertyDetailByIdAsync(int propertyId);
        
        Task<PropertyDetailDto> UpdatePropertyAsync(int Id ,UpdatePropertyDto dto, string ownerId);
        
        Task<bool> DeletePropertyAsync(int propertyId, string ownerId);

        Task<Pagination<PropertyDto>> GetAllAvailablePropertiesAsync(PropertyParams propertyParams);

        Task<Pagination<PropertyDto>> GetPropertiesForAdminAsync(AdminPropertyParams adminPropertyParams);
    }
}
