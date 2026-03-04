using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Shared.Responses
{
    public class ApiResponse<T>
    {
        public int StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }

        // Constructor for Success
        public ApiResponse(T data, string message = "Operation Successful", int statusCode = 200)
        {
            IsSuccess = true;
            Message = message;
            Data = data;
            StatusCode = statusCode;
            Errors = null;
        }

        // Constructor for Failure
        public ApiResponse(string message, int statusCode = 400, List<string>? errors = null)
        {
            IsSuccess = false;
            Message = message;
            Data = default;
            StatusCode = statusCode;
            Errors = errors;
        }
    }
}
