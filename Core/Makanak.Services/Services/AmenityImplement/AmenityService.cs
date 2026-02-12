using AutoMapper;
using Makanak.Abstraction.IServices.AmenityService;
using Makanak.Abstraction.IServices.Cashing;
using Makanak.Domain.Contracts.UOW;
using Makanak.Domain.Models.PropertyEntities;
using Makanak.Shared.Dto_s.Property;
using System.Text.Json;

namespace Makanak.Services.Services.AmenityImplement
{
    public class AmenityService(ICacheService cacheService, IUnitOfWork unitOfWork, IMapper mapper) : IAmenityService
    {
        public async Task<IReadOnlyList<AmenityDto>> GetAllAmenitiesAsync()
        {
            const string cacheKey = "lookups_amenities"; 

            var cachedData = await cacheService.GetCacheResponseAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                return JsonSerializer.Deserialize<IReadOnlyList<AmenityDto>>(cachedData)!;
            }

            var repo = unitOfWork.GetRepo<Amenity, int>();
            var amenities = await repo.GetAllAsync();
            var mappedAmenities = mapper.Map<IReadOnlyList<AmenityDto>>(amenities);

            await cacheService.SetCacheResponseAsync(cacheKey, mappedAmenities, TimeSpan.FromDays(30));

            return mappedAmenities;
        }
    }
}
