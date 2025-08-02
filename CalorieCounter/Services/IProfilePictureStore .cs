namespace CalorieCounter.Services
{
    public interface IProfilePictureStore
    {
        Task<string> SaveAsync(IFormFile file, CancellationToken ct);
    }
}
