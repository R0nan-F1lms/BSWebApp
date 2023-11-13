using Microsoft.AspNetCore.Http;
namespace BSWebApp.Services
{
    public interface IFileUploadService
    {
        Task<string> UploadFile(IFormFile file);
    }
}
