
using DatingAppAPI.DTOs.UploadFile;
using Microsoft.AspNetCore.Http;

namespace DatingAppAPI.Services;

public interface IFileService
{
    Task<FileUploadDbDto.FileResponseDto> UploadFileAsync(
        IFormFile file);

    Task<FileUploadDbDto.FileDownloadDto?> GetFileForDownloadAsync(
        int id);
}
