using AutoMapper;
using Makanak.Abstraction.IServices;
using Makanak.Abstraction.IServices.DisputeService;
using Makanak.Abstraction.IServices.NotificationService;
using Makanak.Domain.Contracts.UOW;
using Makanak.Domain.EnumsHelper.User;
using Makanak.Domain.Exceptions;
using Makanak.Domain.Exceptions.NotFound;
using Makanak.Domain.Models.BookingEntities;
using Makanak.Domain.Models.DisputeEntities;
using Makanak.Domain.Models.Identity;
using Makanak.Services.Specifications.DisputesSpec;
using Makanak.Shared.Common;
using Makanak.Shared.Common.Params.Dispute_Params;
using Makanak.Shared.Dto_s.Dispute;
using Makanak.Shared.EnumsHelper.Dispute;
using Makanak.Shared.HelpersFactory;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Services.Services.DisputeImplement
{
    public class DisputeService(IUnitOfWork unitOfWork , IMapper mapper,
        IAttachementServices attachementServices , UserManager<ApplicationUser> userManager,
        INotificationService notificationService) : IDisputeService
    {
        public async Task<DisputeDto> CreateDisputeAsync(CreateDisputeDto dto, string userId)
        {
            // get booking 
            var bookingRepo = unitOfWork.GetRepo<Booking, int>();
            var booking = await bookingRepo.GetByIdAsync(dto.BookingId);
            if (booking == null)
                throw new BookingNotFound(dto.BookingId);

            if(userId != booking.Owner.Id &&  userId != booking.TenantId)
                throw new UnauthorizedException();

            var dispute = mapper.Map<Dispute>(dto);
            dispute.ComplainantId = userId;
            dispute.Status = DisputeStatus.Pending;

            if (dto.Images != null && dto.Images.Count > 0)
            {
                // uploads/Disputes/Booking_55
                string folderName = $"Booking_{dto.BookingId}";
                string path = Path.Combine("Disputes", folderName);

                foreach (var file in dto.Images)
                {
                    var url = await attachementServices.UploadImageAsync(file, path);
                    dispute.DisputeImages.Add(new DisputeImage
                    {
                        ImageUrl = url
                    });
                }
            }

            // dispute repo 
            var disputeRepo = unitOfWork.GetRepo<Dispute, int>();
            disputeRepo.AddAsync(dispute);
            await unitOfWork.SaveChangesAsync();

            var admins = await userManager.GetUsersInRoleAsync("Admin");
            var complainantUser = await userManager.FindByIdAsync(userId);
            string complainantName = complainantUser?.Name ?? "Unknown User";

            foreach (var admin in admins)
            {
                await notificationService.SendNotificationAsync(
                NotificationFactory.NewDisputeCreated(
                    admin.Id,           
                    complainantName,    
                    dispute.BookingId,  
                    dispute.Id          
                )
            );
            }

            return mapper.Map<DisputeDto>(dispute);
        }

        public async Task<Pagination<DisputeDto>> GetAllDisputesAsync(DisputeParams disputeParams, string userId, string role)
        {
            var repo = unitOfWork.GetRepo<Dispute, int>();

            // (Paged Data)
            var spec = new DisputeSpecifications(disputeParams, userId, role);
            var disputes = await repo.GetAllWithSpecificationAsync(spec);

            // (Total Count) 
            var countSpec = new DisputeWithCountSpecification(disputeParams, userId, role);
            var totalItems = await repo.CountAsync(countSpec);

            var data = mapper.Map<IReadOnlyList<DisputeDto>>(disputes);

            return new Pagination<DisputeDto>(disputeParams.PageIndex, disputeParams.PageSize, totalItems, data);
        }

        public async Task<DisputeDto> GetDisputeByIdAsync(int disputeId, string userId , string role)
        {
            var repo = unitOfWork.GetRepo<Dispute, int>();
            var spec = new DisputeSpecifications(disputeId);
            var dispute = await repo.GetByIdWithSpecificationsAsync(spec);

            bool isAuthorized = (userId == dispute.Booking.OwnerId) ||
                        (userId == dispute.Booking.TenantId) ||
                        (role == "Admin");

            if (!isAuthorized)
                throw new UnauthorizedException();

            return mapper.Map<DisputeDto>(dispute);
        }

        public async Task<bool> ResolveDisputeAsync(ResolveDisputeDto dto)
        {
            var repo = unitOfWork.GetRepo<Dispute, int>();
            var spec = new DisputeSpecifications(dto.DisputeId);
            var dispute = await repo.GetByIdWithSpecificationsAsync(spec);

            if (dispute == null) throw new DisputeNotFound(dto.DisputeId);

            dispute.Status = dto.Decision;
            dispute.AdminComment = dto.AdminComment;
            dispute.ResolvedAt = DateTime.UtcNow;

            repo.Update(dispute);
            await unitOfWork.SaveChangesAsync();

            // message
            try
            {
                // 1. detect the complainant and defendant
                string complainantId = dispute.ComplainantId;

                string defendantId = (dispute.ComplainantId == dispute.Booking.TenantId)
                                     ? dispute.Booking.OwnerId
                                     : dispute.Booking.TenantId;

                // (Complainant)
                await notificationService.SendNotificationAsync(
                    NotificationFactory.DisputeConcluded(
                        complainantId,
                        dispute.BookingId,
                        dispute.Id,
                        dto.Decision,
                        dto.AdminComment
                    )
                );

                // (Defendant)
                await notificationService.SendNotificationAsync(
                    NotificationFactory.DisputeConcluded(
                        defendantId,
                        dispute.BookingId,
                        dispute.Id,
                        dto.Decision,
                        dto.AdminComment
                    )
                );
            }
            catch (Exception ex)
            {
                // Log error but don't stop the process
                // _logger.LogError($"Failed to send dispute notifications: {ex.Message}");
            }

            return true;
        }
       
        public async Task<bool> CancelDisputeAsync(int disputeId, string userId)
        {
            var disputeRepo = unitOfWork.GetRepo<Dispute, int>();

            var dispute = await disputeRepo.GetByIdAsync(disputeId);

            if (dispute == null)
                throw new DisputeNotFound(disputeId);

            if (dispute.ComplainantId != userId)
                throw new UnauthorizedException();

            if (dispute.Status != DisputeStatus.Pending)
                throw new BadRequestException("You cannot cancel a dispute that has already been resolved or processed.");

            dispute.Status = DisputeStatus.Cancelled;

            dispute.AdminComment = "Cancelled by the complainant.";

            disputeRepo.Update(dispute);

            var result = await unitOfWork.SaveChangesAsync();

            // message
            if (result > 0)
            {
                try
                {
                    var user = await userManager.FindByIdAsync(userId);
                    string userName = user?.Name ?? "Unknown User";

                    var admins = await userManager.GetUsersInRoleAsync("Admin");

                    foreach (var admin in admins)
                    {
                        await notificationService.SendNotificationAsync(
                            NotificationFactory.DisputeCancelled(
                                admin.Id,
                                userName,
                                dispute.BookingId,
                                dispute.Id
                            )
                        );
                    }
                }
                catch (Exception){}

                return true;
            }
            return result > 0;
        }

    }
}
