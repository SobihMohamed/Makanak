using Makanak.Abstraction.IServices;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Makanak.Services.Services
{
    public class AttachementServices : IAttachementServices
    {
        private readonly List<string> AllowedExtentions = new List<string>() { ".jpg", ".png", ".jpeg" };
        private const long _fileSizeLimit = 2 * 1024 * 1024; // 2 MB

        public async Task<string> UploadImageAsync(IFormFile? formFile, string subFolder)
        {
            if(formFile == null)
            {
                throw new ArgumentNullException(nameof(formFile), "File cannot be null");
            }
            // 1 - check allowed extentions
            var fileExtention = Path.GetExtension(formFile.FileName).ToLowerInvariant();
            if (!AllowedExtentions.Contains(fileExtention) || fileExtention is null)
                throw new ArgumentException("File type is not allowed");
            // 2 - check file size
            var fileSize = formFile.Length;
            if (fileSize > _fileSizeLimit || fileSize == 0)
                throw new Exception("This file is too large");

            // 3 - Get Root Path (wwwroot/uploads) + SubFolder
            var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            var folderPath = Path.Combine(webRootPath, subFolder);

            // 4 - check if folderPath Exist
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            // 5 - create unique file name
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtention}";


            // 6 - create Full Path 
            var fullPath = Path.Combine(folderPath, uniqueFileName);

            // 7 - Save inside HARDDISK
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await formFile.CopyToAsync(stream);
            }

            return Path.Combine("uploads",subFolder,uniqueFileName).Replace("\\", "/");
        }
        public async Task<bool> DeleteImage(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return true;
            }
            return false;
        }
    }
}
