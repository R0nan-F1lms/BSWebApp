using Microsoft.AspNetCore.Http;
namespace BSWebApp.Services
{
    public class FileUploadService : IFileUploadService
    {

        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png" };


        public async Task<string> UploadFile(IFormFile file)
        {
            string path = string.Empty;
            if (file == null || file.Length == 0)
            {
                // No file uploaded, do nothing
                return null;
            }
            try
            {

                if (file != null && ValidateFileExtension(file.FileName))
                {
                    path = Path.Combine("wwwroot","Images");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    var fileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(file.FileName)}";
                    var filePath = Path.Combine(path, fileName);

                    using (var filestream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(filestream);
                    }

                    return Path.Combine("Images", fileName);
                }
                return null;

            }
            catch(Exception ex)
            {
                throw new Exception("File could not be uploaded, "+ex.Message);
            }
        }
        private bool ValidateFileExtension(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return AllowedExtensions.Contains(extension);
        }
    }
}
