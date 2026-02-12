using Makanak.Shared.Dto_s.Governorate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Abstraction.IServices.GovernorateService
{
    public interface IGovernorateService
    {
        Task<IReadOnlyList<GovernorateDto>> GetAllGovernoratesAsync();
    }
}
