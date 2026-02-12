using Makanak.Abstraction.IServices.Manager;
using Makanak.Shared.Dto_s.Review;
using Makanak.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Makanak.Presentation.Controllers.Reviews_Controller
{
    public class ReviewController(IServiceManager serviceManager) :  AppBaseController
    {
        [Authorize(Roles = "Tenant")]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<ReviewDto>>> CreateReview([FromBody] CreateReviewDto dto)
        {
            var tenantId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await serviceManager.ReviewService.AddReviewAsync(dto, tenantId);

            return Created(result, "Review submitted successfully.");
        }

        [AllowAnonymous] 
        [HttpGet("{propertyId}")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<ReviewDto>>>> GetPropertyReviews(int propertyId)
        {
            var result = await serviceManager.ReviewService.GetPropertyReviewsAsync(propertyId);

            return Success(result);
        }

        [Authorize(Roles = "Tenant")]
        [HttpDelete("{reviewId}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteReview(int reviewId)
        {
            var tenantId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await serviceManager.ReviewService.DeleteReviewAsync(reviewId, tenantId);

            return Success("Review deleted successfully.");
        }
    }
}
