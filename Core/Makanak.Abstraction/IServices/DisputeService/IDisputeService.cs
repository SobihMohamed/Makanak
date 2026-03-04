using Makanak.Shared.Common;
using Makanak.Shared.Common.Params.Dispute_Params;
using Makanak.Shared.Dto_s.Dispute;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Abstraction.IServices.DisputeService
{
    public interface IDisputeService
    {
        Task<DisputeDto> CreateDisputeAsync(CreateDisputeDto dto, string userId);

        Task<DisputeDto> GetDisputeByIdAsync(int disputeId, string userId, string role);

        Task<Pagination<DisputeDto>> GetAllDisputesAsync(DisputeParams disputeParams, string userId, string role);

        Task<bool> ResolveDisputeAsync(ResolveDisputeDto dto);

        Task<bool> CancelDisputeAsync(int disputeId,string userId);
    }
}
