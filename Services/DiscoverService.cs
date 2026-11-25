using DatingAppAPI.Models;
using DatingAppAPI.DTOs.Discover;
using DatingAppAPI.Repositories;

namespace DatingAppAPI.Services;

public class DiscoverService : IDiscoverService
{
    private readonly IDiscoverRepository _repository;

    public DiscoverService(IDiscoverRepository repository)
    {
        _repository = repository;
    }

    public async Task<DiscoverResponseDto> GetProfilesAsync(
        DiscoverQueryParameters parameters)
    {
        var (items, totalCount) =
            await _repository.GetProfilesAsync(parameters);

        return new DiscoverResponseDto
        {
            Items = items
                .Select(MapToDto)
                .ToList(),

            TotalCount = totalCount,

            PageNumber = Math.Clamp(
                parameters.PageNumber, 1, 100000),

            PageSize = Math.Clamp(
                parameters.PageSize, 1, 24)
        };
    }

    public async Task<DiscoverProfileDto?> GetByIdAsync(
        int id)
    {
        var user = await _repository.GetByIdAsync(id);

        if (user == null)
        {
            return null;
        }

        return MapToDto(user);
    }

    private static DiscoverProfileDto MapToDto(User user)
    {
        return new DiscoverProfileDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Bio = user.Bio ?? string.Empty,
            AvatarUrl = user.AvatarUrl,
            CreatedAt = user.CreatedAt
        };
    }
}