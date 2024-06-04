namespace SocialMediaMVC.Services.FileUploadService
{
    public class LocalFileUploadService : IFileUploadService
    {
        public readonly string _uploadsDirectory = "wwwroot";
        public async Task<string> UploadFile(IFormFile file)
        {
            if (!Directory.Exists(Path.Combine(_uploadsDirectory, "uploads")))
            {
                Directory.CreateDirectory(Path.Combine(_uploadsDirectory, "uploads"));
            }

            var uuid = Guid.NewGuid().ToString();
            var ext = Path.GetExtension(file.FileName);

            var key = Path.Combine("uploads", uuid + ext).Replace("\\", "/");
            var filePath = Path.Combine(_uploadsDirectory, key);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/" + key;
        }

        public void DeleteFile(string filePath)
        {
            filePath = Path.Combine(_uploadsDirectory, filePath.TrimStart('/'));
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
