using DatingAppAPI.DTOs.Discover;

namespace DatingAppAPI.Services;

public interface IDiscoverService
{
    Task<DiscoverResponseDto> GetProfilesAsync(
        DiscoverQueryParameters parameters);

    Task<DiscoverProfileDto?> GetByIdAsync(int id);
}