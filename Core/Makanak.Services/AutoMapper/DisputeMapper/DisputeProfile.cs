using AutoMapper;
using Makanak.Domain.Models.DisputeEntities;
using Makanak.Shared.Dto_s.Dispute;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Services.AutoMapper.DisputeMapper
{
    public class DisputeProfile : Profile
    {
        public DisputeProfile()
        {
            CreateMap<CreateDisputeDto, Dispute>();

            CreateMap<Dispute, DisputeDto>()
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.Reason, o => o.MapFrom(s => s.Reason.ToString()))
                .ForMember(d => d.PropertyName, o => o.MapFrom(s => s.Booking.Property.Title))
                .ForMember(d => d.ComplainantName, o => o.MapFrom(s => s.Complainant.Name))
                .ForMember(d => d.Images, o => o.MapFrom(s => s.DisputeImages.Select(i => i.ImageUrl)))
                // 👇 حساب اسم الخصم
                .ForMember(d => d.DefendantName, o => o.MapFrom(s =>
                    s.ComplainantId == s.Booking.TenantId
                        ? s.Booking.Property.Owner.Name  // لو المشتكي مستأجر -> الخصم مالك
                        : s.Booking.Tenant.Name));       // العكس
        }
    }
}
