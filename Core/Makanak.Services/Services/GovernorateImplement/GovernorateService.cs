using AutoMapper;
using Makanak.Abstraction.IServices.Cashing;
using Makanak.Abstraction.IServices.GovernorateService;
using Makanak.Domain.Contracts.UOW;
using Makanak.Domain.Models.LocationEntities;
using Makanak.Shared.Dto_s.Governorate;
using Stripe;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Makanak.Services.Services.GovernorateImplement
{
    public class GovernorateService(ICacheService cacheService ,
        IUnitOfWork unitOfWork , IMapper mapper) : IGovernorateService
    {
        public async Task<IReadOnlyList<GovernorateDto>> GetAllGovernoratesAsync()
        {
            const string cacheKey = "lookups_governorates";

            var cachedData = await cacheService.GetCacheResponseAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedData))
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                return JsonSerializer.Deserialize<IReadOnlyList<GovernorateDto>>(cachedData, options)!;
            }

            var repo = unitOfWork.GetRepo<Governorate, int>();
            var governorates = await repo.GetAllAsync();
            var mappedGovernorates = mapper.Map<IReadOnlyList<GovernorateDto>>(governorates);

            await cacheService.SetCacheResponseAsync(cacheKey, mappedGovernorates, TimeSpan.FromDays(30));

            return mappedGovernorates;
        }
    }
}
