using Microsoft.AspNetCore.Http;
using DatingAppAPI.DTOs.UploadFile;
using DatingAppAPI.Repositories;
using DatingAppAPI.Models;

namespace DatingAppAPI.Services
{
    public class FileService : IFileService
    {
        private readonly IFileRepository _fileRepository;

        public FileService(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        public async Task<FileUploadDbDto.FileResponseDto> UploadFileAsync(
    IFormFile file)
        {
            ValidateImageFile(file);
            if (file == null || file.Length == 0)
                throw new ArgumentException("File không hợp lệ.");

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var storedFile = new StoredFile
            {
                FileName = Path.GetFileName(file.FileName),
                ContentType = file.ContentType,
                Content = memoryStream.ToArray(),
                Length = file.Length
            };

            var createdFile = await _fileRepository.AddAsync(storedFile);

            return new FileUploadDbDto.FileResponseDto
            {
                Id = createdFile.Id,
                FileName = createdFile.FileName,
                Size = createdFile.Length,
                UploadedAt = createdFile.UploadedAt
            };
        }

        public async Task<FileUploadDbDto.FileDownloadDto?> GetFileForDownloadAsync(
    int id)
        {
            var file = await _fileRepository.GetByIdAsync(id);
            if (file == null) return null;

            return new FileUploadDbDto.FileDownloadDto
            {
                Content = file.Content,
                ContentType = file.ContentType,
                FileName = file.FileName
            };
        }

        private static void ValidateImageFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Vui lòng chọn file ảnh.");
            }

            // Tối đa 512 KB = 0,5 MiB
            const long maxFileSize = 512 * 1024;

            if (file.Length > maxFileSize)
            {
                throw new ArgumentException(
                    "Dung lượng ảnh không được vượt quá 512 KB (0,5 MB).");
            }

            var allowedExtensions = new[]
            {
        ".jpg", ".jpeg", ".png", ".gif", ".webp"
    };

            var extension = Path.GetExtension(file.FileName);

            if (string.IsNullOrWhiteSpace(extension) ||
                !allowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    "Chỉ chấp nhận file ảnh JPG, JPEG, PNG, GIF hoặc WEBP.");
            }

            var allowedContentTypes = new[]
            {
        "image/jpeg",
        "image/png",
        "image/gif",
        "image/webp"
    };

            if (!allowedContentTypes.Contains(
                file.ContentType,
                StringComparer.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    "Định dạng nội dung file không hợp lệ.");
            }
        }

    }
}