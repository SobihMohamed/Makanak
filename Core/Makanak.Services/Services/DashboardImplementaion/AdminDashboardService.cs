using Makanak.Abstraction.IServices.DashboardService;
using Makanak.Domain.Contracts.UOW;
using Makanak.Domain.EnumsHelper.User;
using Makanak.Domain.Models.BookingEntities;
using Makanak.Domain.Models.Identity;
using Makanak.Domain.Models.PropertyEntities;
using Makanak.Services.Specifications;
using Makanak.Services.Specifications.BookingSpec;
using Makanak.Services.Specifications.Property_Spec;
using Makanak.Services.Specifications.User;
using Makanak.Shared.Common.Params.User;
using Makanak.Shared.Dto_s.Dashboard;
using Makanak.Shared.EnumsHelper.Booking;
using Makanak.Shared.EnumsHelper.Property;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Services.Services.DashboardImplementaion
{
    public class AdminDashboardService(IUnitOfWork _unitOfWork) : IAdminDashboardService
    {
        public async Task<FinancialStatsDto> GetFinancialStatsAsync()
        {
            var bookingRepo = _unitOfWork.GetRepo<Booking, int>();

            var spec = new BookingDashboardSpecifications();
            var successfulBookings = await bookingRepo.GetAllWithSpecificationAsync(spec);

            return new FinancialStatsDto
            {
                TotalBookingVolume = successfulBookings.Sum(b => b.TotalPrice),
                TotalPlatformEarnings = successfulBookings.Sum(b => b.CommissionPaid),
                TotalCashExpectedByOwners = successfulBookings.Sum(b => b.AmountToPayToOwner)
            };
        }
        public async Task<BookingStatsDto> GetBookingStatsAsync()
        {
            return new BookingStatsDto
            {
                TotalBookings = await CountBookingsAsync(),
                PendingBookings = await CountBookingsAsync(status: BookingStatus.PendingOwnerApproval),
                PaymentReceived = await CountBookingsAsync(status: BookingStatus.PaymentReceived),
                CheckedIn = await CountBookingsAsync(status: BookingStatus.CheckedIn),
                CompletedBookings = await CountBookingsAsync(status: BookingStatus.Completed),
                CancelledBookings = await CountBookingsAsync(status: BookingStatus.Cancelled)
            };
        }
        public async Task<PropertyStatsDto> GetPropertyStatsAsync()
        {
            return new PropertyStatsDto
            {
                TotalProperties = await CountPropertiesAsync(),
                ActiveProperties = await CountPropertiesAsync(status: PropertyStatus.Accepted),
                PendingApprovalProperties = await CountPropertiesAsync(status: PropertyStatus.Pending),
                RejectedProperties = await CountPropertiesAsync(status: PropertyStatus.Rejected)
            };
        }
        public async Task<UserStatsDto> GetUserStatsAsync()
        {
            return new UserStatsDto
            {
                TotalUsers = await CountUsersAsync(),
                PendingUsers = await CountUsersAsync(status: UserStatus.Pending),
                ActiveUsers = await CountUsersAsync(status: UserStatus.Active),
                RejectsCount = await CountUsersAsync(status: UserStatus.Rejected),
                BannedsCount = await CountUsersAsync(status: UserStatus.Banned),
                NewsCount = await CountUsersAsync(status: UserStatus.New),

                OwnersCount = await CountUsersAsync(type: UserTypes.Owner),
                AdminsCount = await CountUsersAsync(type: UserTypes.Admin),
                TenantsCount = await CountUsersAsync(type: UserTypes.Tenant)
            };
        }

        #region Helper Methods
        private async Task<int> CountUsersAsync(UserStatus? status = null, UserTypes? type = null)
        {
            var userRepo = _unitOfWork.GetRepo<ApplicationUser, string>();
            var parameters = new UserParams { Status = status, Type = type };
            var spec = new UserSpecifications(parameters, isCount: true);
            return await userRepo.CountAsync(spec);
        }

        private async Task<int> CountPropertiesAsync(PropertyStatus? status = null)
        {
            var propertyRepo = _unitOfWork.GetRepo<Property, int>();
            var spec = new PropertyDashboardSpecifications(status);
            return await propertyRepo.CountAsync(spec);
        }

        private async Task<int> CountBookingsAsync(BookingStatus? status = null)
        {
            var bookingRepo = _unitOfWork.GetRepo<Booking, int>();
            var spec = new BookingDashboardSpecifications(status);
            return await bookingRepo.CountAsync(spec);
        }

        #endregion
    }
}
