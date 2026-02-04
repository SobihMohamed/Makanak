using Makanak.Shared.Dto_s.Review;

namespace Makanak.Abstraction.IServices.ReviewService
{
    public interface IReviewService
    {
        Task<ReviewDto> AddReviewAsync(CreateReviewDto createReviewDto , string tenantId);

        Task<IReadOnlyList<ReviewDto>> GetPropertyReviewsAsync(int propertyId);

        Task<bool> DeleteReviewAsync(int reviewId, string tenantId);
    }
}
