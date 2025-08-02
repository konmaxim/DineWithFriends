using CalorieCounter.Services;

namespace CalorieCounter.Infrastructure
{
    public class FileSystemProfilePictureStore : IProfilePictureStore
    {
        private readonly string _uploadsFolder;
        public FileSystemProfilePictureStore(IWebHostEnvironment env)
        {
            _uploadsFolder = Path.Combine(env.WebRootPath, "pfps");
            Directory.CreateDirectory(_uploadsFolder);
        }

        public async Task<string> SaveAsync(IFormFile file, CancellationToken ct)
        {
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var fullPath = Path.Combine(_uploadsFolder, fileName);
            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream, ct);
            return $"/pfps/{fileName}";
        }
    }
}
