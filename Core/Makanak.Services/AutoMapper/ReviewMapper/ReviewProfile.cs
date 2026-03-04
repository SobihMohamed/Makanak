using AutoMapper;
using Makanak.Domain.Models.ReviewEntities;
using Makanak.Shared.Dto_s.Review;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Services.AutoMapper.ReviewMapper
{
    public class ReviewProfile : Profile 
    {
        public ReviewProfile()
        {
            CreateMap<CreateReviewDto, Review>();

            CreateMap<Review, ReviewDto>()
                .ForMember(d => d.ReviewerName, src => src.MapFrom(s => s.Tenant.Name))
                .ForMember(d => d.ReviewerPhotoUrl, src => src.MapFrom(s => s.Tenant.ProfilePictureUrl));
        }
    }
}
