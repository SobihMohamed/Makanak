using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Abstraction.IServices
{
    public interface IAttachementServices
    {
        Task<string> UploadImageAsync(IFormFile? formFile , string folderName);
        public Task<bool> DeleteImage(string filePath);
    }
}
