using Makanak.Shared.Responses;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppBaseController : ControllerBase
    {
        // Standardized with data success response
        protected IActionResult Success<T>(T data, string message = "Operation Successfully")
        {
            return Ok(new ApiResponse<T>(data, message));
        }

        // Standardized without data success response
        protected IActionResult Success(string message = "Operation Successfully")
        {
            return Ok(new ApiResponse<string>(null ,message, 200));
        }

        // Standardized Created response
        protected IActionResult Created<T>(T data, string message = "Created Successfully")
        {
            return StatusCode(201, new ApiResponse<T>(data, message, 201));
        }

        protected IActionResult BadRequestError(string message)
        {
            return StatusCode(400, new ApiResponse<object>(null, message, 400));
        }

        // 404 Not Found
        protected IActionResult NotFoundError(string message = "Resource not found")
        {
            return StatusCode(404, new ApiResponse<object>(null, message, 404));
        }

        // 401 Unauthorized 
        protected IActionResult UnauthorizedError(string message = "You are not authorized")
        {
            return StatusCode(401, new ApiResponse<object>(null, message, 401));
        }
    }
}
