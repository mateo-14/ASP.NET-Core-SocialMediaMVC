namespace SocialMediaMVC.Services.FileUploadService
{
    public interface IFileUploadService
    {
        public Task<string> UploadFile(IFormFile file);
        public void DeleteFile(string filePath);
    }
}
