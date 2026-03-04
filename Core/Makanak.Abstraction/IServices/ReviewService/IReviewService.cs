using Makanak.Shared.Common;
using Makanak.Shared.Common.Params;
using Makanak.Shared.Dto_s.Review;

namespace Makanak.Abstraction.IServices.ReviewService
{
    public interface IReviewService
    {
        Task<ReviewDto> AddReviewAsync(CreateReviewDto createReviewDto , string tenantId);

        Task<Pagination<ReviewDto>> GetPropertyReviewsAsync(int propertyId, BaseQueryParams queryParams);

        Task<bool> DeleteReviewAsync(int reviewId, string tenantId);
    }
}
