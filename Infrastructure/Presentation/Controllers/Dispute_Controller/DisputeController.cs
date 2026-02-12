using Makanak.Abstraction.IServices.Manager;
using Makanak.Shared.Common;
using Makanak.Shared.Common.Params.Dispute_Params;
using Makanak.Shared.Dto_s.Dispute;
using Makanak.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Makanak.Presentation.Controllers.Dispute_Controller
{
    [Authorize]
    public class DisputeController(IServiceManager serviceManager) : AppBaseController
    {
        [HttpPost]
        public async Task<ActionResult<ApiResponse<DisputeDto>>> CreateDispute([FromForm] CreateDisputeDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await serviceManager.DisputeService.CreateDisputeAsync(dto, userId!);

            return Created(result, "Dispute created successfully and is under review.");
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<Pagination<DisputeDto>>>> GetAllDisputes([FromQuery] DisputeParams disputeParams)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // بنجيب الرول عشان السرفيس تحدد (أدمن يشوف كله، يوزر يشوف حاجته)
            var role = User.FindFirstValue(ClaimTypes.Role);

            var result = await serviceManager.DisputeService.GetAllDisputesAsync(disputeParams, userId!, role!);

            return Success(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<DisputeDto>>> GetDisputeById(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

            var result = await serviceManager.DisputeService.GetDisputeByIdAsync(id, userId!, role!);

            return Success(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("resolve")]
        public async Task<ActionResult<ApiResponse<string>>> ResolveDispute([FromBody] ResolveDisputeDto dto)
        {
            // مش محتاجين UserId هنا لأن الأدمن له صلاحية مطلقة
            var result = await serviceManager.DisputeService.ResolveDisputeAsync(dto);

            if (!result)
                return BadRequestError("Failed to resolve dispute.");

            return Success("Dispute resolved successfully.");
        }

        [HttpPatch("{id}/cancel")]
        public async Task<ActionResult<ApiResponse<string>>> CancelDispute(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await serviceManager.DisputeService.CancelDisputeAsync(id, userId!);

            if (!result)
                return BadRequestError("Failed to cancel dispute or it's not in pending status.");

            return Success("Dispute cancelled successfully.");
        }
    }
}
