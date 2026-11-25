using DatingAppAPI.Models;

namespace DatingAppAPI.Repositories;

public interface IInterestRepository
{
    Task<List<Interest>> GetAllAsync(
        bool includeInactive = false);

    Task<Interest?> GetByIdAsync(int id);

    Task<bool> ExistsByNameAsync(
        string name,
        int? excludeId = null);

    Task<bool> IsActiveUserAsync(int userId);

    Task<List<Interest>> GetUserInterestsAsync(
        int userId);

    Task<bool> ReplaceUserInterestsAsync(
        int userId,
        List<int> interestIds);

    Task AddAsync(Interest interest);

    Task SaveChangesAsync();
}