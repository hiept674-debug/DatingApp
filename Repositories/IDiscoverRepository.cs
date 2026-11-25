using DatingAppAPI.Models;
using DatingAppAPI.DTOs.Discover;

namespace DatingAppAPI.Repositories;

public interface IDiscoverRepository
{
    Task<(List<User> Items, int TotalCount)>
        GetProfilesAsync(DiscoverQueryParameters parameters);

    Task<User?> GetByIdAsync(int id);
}