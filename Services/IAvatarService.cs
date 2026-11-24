using Microsoft.AspNetCore.Http;

namespace DatingAppAPI.Services;

public interface IAvatarService
{
    Task<string?> UploadAvatarAsync(
        int userId,
        IFormFile file);

    Task<bool> DeleteAvatarAsync(int userId);
}