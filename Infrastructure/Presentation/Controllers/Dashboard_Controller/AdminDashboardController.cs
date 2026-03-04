using Makanak.Abstraction.IServices.Manager;
using Makanak.Shared.Dto_s.Dashboard;
using Makanak.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Presentation.Controllers.Dashboard_Controller
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminDashboardController(IServiceManager _serviceManager) : AppBaseController 
    {
        [HttpGet("users-stats")]
        public async Task<ActionResult<ApiResponse<UserStatsDto>>> GetUsersStats()
        => Success(await _serviceManager.AdminDashboardService.GetUserStatsAsync());

        [HttpGet("properties-stats")]
        public async Task<ActionResult<ApiResponse<PropertyStatsDto>>> GetPropertiesStats()
            => Success(await _serviceManager.AdminDashboardService.GetPropertyStatsAsync());

        [HttpGet("bookings-stats")]
        public async Task<ActionResult<ApiResponse<BookingStatsDto>>> GetBookingsStats()
            => Success(await _serviceManager.AdminDashboardService.GetBookingStatsAsync());

        [HttpGet("financial-stats")]
        public async Task<ActionResult<ApiResponse<FinancialStatsDto>>> GetFinancialStats()
            => Success(await _serviceManager.AdminDashboardService.GetFinancialStatsAsync());
    }
}
