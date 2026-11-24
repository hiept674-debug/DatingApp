
using DatingAppAPI.Models;

namespace DatingAppAPI.Repositories;

public interface IFileRepository
{
    Task<StoredFile> AddAsync(StoredFile file);
    Task<StoredFile?> GetByIdAsync(int id);
}
