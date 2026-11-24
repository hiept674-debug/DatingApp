
using DatingAppAPI.Data;
using DatingAppAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace DatingAppAPI.Services;

public class AvatarService : IAvatarService
{
    private readonly AppDbContext _context;

    private const long MaxFileSize = 512 * 1024;

    public AvatarService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<string?> UploadAvatarAsync(
        int userId,
        IFormFile file)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u =>
                u.Id == userId && u.IsActive);

        if (user == null)
            return null;

        // Kiểm tra và xử lý ảnh
        var content = await ProcessImageAsync(file);

        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                System.Data.IsolationLevel.Serializable);

        // Lấy các avatar cũ của chính tài khoản
        var oldFiles = await _context.StoredFiles
            .Where(f =>
                f.OwnerId == userId &&
                f.Purpose == "Avatar")
            .ToListAsync();

        var storedFile = new StoredFile
        {
            OwnerId = userId,

            FileName = $"{Guid.NewGuid():N}.jpg",

            ContentType = "image/jpeg",

            Content = content,

            Length = content.LongLength,

            Purpose = "Avatar",

            UploadedAt = DateTime.UtcNow
        };

        await _context.StoredFiles.AddAsync(storedFile);

        await _context.SaveChangesAsync();

        // Cập nhật đường dẫn avatar
        user.AvatarUrl = $"/api/files/{storedFile.Id}";

        user.UpdatedAt = DateTime.UtcNow;

        // Xóa avatar cũ
        _context.StoredFiles.RemoveRange(oldFiles);

        await _context.SaveChangesAsync();

        await transaction.CommitAsync();

        return user.AvatarUrl;
    }

    public async Task<bool> DeleteAvatarAsync(int userId)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                System.Data.IsolationLevel.Serializable);

        var user = await _context.Users
            .FirstOrDefaultAsync(u =>
                u.Id == userId && u.IsActive);

        if (user == null)
            return false;

        var avatarFiles = await _context.StoredFiles
            .Where(f =>
                f.OwnerId == userId &&
                f.Purpose == "Avatar")
            .ToListAsync();

        user.AvatarUrl = null;

        user.UpdatedAt = DateTime.UtcNow;

        _context.StoredFiles.RemoveRange(avatarFiles);

        await _context.SaveChangesAsync();

        await transaction.CommitAsync();

        return true;
    }

    private static async Task<byte[]> ProcessImageAsync(
        IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException(
                "Vui lòng chọn ảnh.");
        }

        if (file.Length > MaxFileSize)
        {
            throw new ArgumentException(
                "Ảnh không được vượt quá 512 KB.");
        }

        var allowedExtensions = new[]
        {
            ".jpg", ".jpeg", ".png", ".webp"
        };

        var extension = Path.GetExtension(file.FileName);

        if (!allowedExtensions.Contains(
            extension,
            StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                "Chỉ hỗ trợ JPG, PNG và WEBP.");
        }

        try
        {
            using var stream = new MemoryStream();

            await file.CopyToAsync(stream);

            stream.Position = 0;

            var format = await Image.DetectFormatAsync(stream);

            var allowedFormats = new[]
            {
                "JPEG", "PNG", "WEBP"
            };

            if (!allowedFormats.Contains(
                format.Name,
                StringComparer.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    "Nội dung file ảnh không hợp lệ.");
            }

            stream.Position = 0;

            var info = await Image.IdentifyAsync(stream);

            if (info.Width > 2048 || info.Height > 2048)
            {
                throw new ArgumentException(
                    "Kích thước ảnh tối đa là 2048 x 2048.");
            }

            stream.Position = 0;

            using var image = await Image.LoadAsync(
                new SixLabors.ImageSharp.Formats.DecoderOptions
                {
                    MaxFrames = 1
                },
                stream);

            image.Mutate(x => x
                .AutoOrient()
                .Resize(new ResizeOptions
                {
                    Size = new Size(512, 512),
                    Mode = ResizeMode.Max
                })
                .BackgroundColor(Color.White));

            using var output = new MemoryStream();

            await image.SaveAsJpegAsync(
                output,
                new JpegEncoder
                {
                    Quality = 85,
                    SkipMetadata = true
                });

            if (output.Length > MaxFileSize)
            {
                throw new ArgumentException(
                    "Ảnh sau xử lý vượt quá dung lượng cho phép.");
            }

            return output.ToArray();
        }
        catch (UnknownImageFormatException)
        {
            throw new ArgumentException(
                "File không phải ảnh hợp lệ.");
        }
        catch (InvalidImageContentException)
        {
            throw new ArgumentException(
                "File ảnh bị lỗi hoặc không hợp lệ.");
        }
    }
}
