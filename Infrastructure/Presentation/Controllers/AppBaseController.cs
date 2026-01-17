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
        protected IActionResult Created<T>(T data, string message = "Resource Created Successfully")
        {
            return StatusCode(201, new ApiResponse<T>(data, message, 201));
        }

    }
}
