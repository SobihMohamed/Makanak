using AutoMapper;
using Makanak.Abstraction.IServices.ReviewService;
using Makanak.Domain.Contracts.UOW;
using Makanak.Domain.Exceptions;
using Makanak.Domain.Exceptions.NotFound;
using Makanak.Domain.Models.BookingEntities;
using Makanak.Domain.Models.PropertyEntities;
using Makanak.Domain.Models.ReviewEntities;
using Makanak.Services.Specifications.BookingSpec;
using Makanak.Services.Specifications.ReviewSpec;
using Makanak.Shared.Dto_s.Review;
using Makanak.Shared.EnumsHelper.Booking;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Services.Services.ReviewImplement
{
    public class ReviewService(IUnitOfWork unitOfWork, IMapper mapper) : IReviewService
    {
        public async Task<ReviewDto> AddReviewAsync(CreateReviewDto createReviewDto, string tenantId)
        {
            
            var bookingId = createReviewDto.BookingId;

            // check booking 
            var bookingRepo = unitOfWork.GetRepo<Booking, int>();
            // specification
            var spec = new BookingSpecifications(bookingId);

            var booking = await bookingRepo.GetByIdWithSpecificationsAsync(spec);

            if(booking == null) 
                throw new BookingNotFound(bookingId);

            // check the user
            if(booking.TenantId != tenantId)
                throw new BadRequestException("Can not add review by another account");

            // check status completed ?
            if(booking.Status != BookingStatus.Completed)
                throw new BadRequestException("Can not add review while the status not completed");

            var reviewSpec = new ReviewSpecifications(bookingId);

            var reviewRepo = unitOfWork.GetRepo<Review, int>();

            var reviewCount = await reviewRepo.CountAsync(reviewSpec);
            if(reviewCount != 0)
                throw new BadRequestException($"Can not Make 2 Review");

            var review = mapper.Map<Review>(createReviewDto);
            review.TenantId = tenantId;
            review.PropertyId = booking.PropertyId;

            reviewRepo.AddAsync(review);
            var result = await unitOfWork.SaveChangesAsync();

            if (result <= 0) throw new Exception("Failed to save review.");

            // update property average rating
            await UpdatePropertyAverageRatingAsync(booking.PropertyId);

            return mapper.Map<ReviewDto>(review);

        }
        public async Task<IReadOnlyList<ReviewDto>> GetPropertyReviewsAsync(int propertyId)
        {
            // get the repo of the review 
            var reviewRepo = unitOfWork.GetRepo<Review, int>();

            // get the specifications of the review 
            var reviewSpec = new ReviewSpecifications(propertyId , IsPropertyReviews:true);

            // get the reviews of the property 
            var propertyReviews = await reviewRepo.GetAllWithSpecificationAsync(reviewSpec);

            // return with the dto 

            return mapper.Map<IReadOnlyList<ReviewDto>>(propertyReviews);

        }
        public async Task<bool> DeleteReviewAsync(int reviewId, string tenantId)
        {
            var reviewRepo = unitOfWork.GetRepo<Review, int>();

            var review = await reviewRepo.GetByIdAsync(reviewId);

            if (review == null) 
                throw new ReviewNotFound(reviewId);

            if (review.TenantId != tenantId)
                throw new UnauthorizedAccessException("You are not allowed to delete this review.");

            var propertyId = review.PropertyId;

            reviewRepo.Delete(review);
            var result = await unitOfWork.SaveChangesAsync();

            if (result <= 0) 
                throw new BadRequestException("Failed to delete review.");

            await UpdatePropertyAverageRatingAsync(propertyId);

            return true;
        }
        private async Task UpdatePropertyAverageRatingAsync(int propertyId)
        {
            // get repos 
            var propertyRepo = unitOfWork.GetRepo<Property, int>();
            var reviewRepo = unitOfWork.GetRepo<Review, int>();

            // get all property review 
            var spec = new ReviewSpecifications(propertyId, IsPropertyReviews: true);
            var reviews = await reviewRepo.GetAllWithSpecificationAsync(spec);

            if (reviews.Any())
            {
                var average = reviews.Average(r => r.Rating);

                var property = await propertyRepo.GetByIdAsync(propertyId);

                property.AverageRating = Math.Round(average, 1);

                propertyRepo.Update(property);

                await unitOfWork.SaveChangesAsync();
            }

        }
    }
}
