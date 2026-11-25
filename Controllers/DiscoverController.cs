using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DatingAppAPI.Services;
using DatingAppAPI.DTOs.Discover;

namespace DatingAppAPI.Controllers;

[Authorize(Roles = "User,Admin")]
[ApiController]
[Route("api/[controller]")]
public class DiscoverController : ControllerBase
{
    private readonly IDiscoverService _discoverService;

    public DiscoverController(
        IDiscoverService discoverService)
    {
        _discoverService = discoverService;
    }

    [HttpGet]
    [HttpGet]
    public async Task<ActionResult<DiscoverResponseDto>>
    GetProfiles(
        [FromQuery] DiscoverQueryParameters parameters)
    {
        // Debug Advanced Filters
        Console.WriteLine("===== DISCOVER FILTERS =====");

        Console.WriteLine($"Search: {parameters.Search}");
        Console.WriteLine($"Category: {parameters.Category}");

        Console.WriteLine(
            $"InterestIds: {string.Join(",", parameters.InterestIds)}"
        );

        Console.WriteLine(
            $"MatchAllInterests: {parameters.MatchAllInterests}"
        );

        Console.WriteLine($"SortBy: {parameters.SortBy}");
        Console.WriteLine($"PageNumber: {parameters.PageNumber}");
        Console.WriteLine($"PageSize: {parameters.PageSize}");

        var result = await _discoverService
            .GetProfilesAsync(parameters);

        Console.WriteLine($"TotalCount: {result.TotalCount}");

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DiscoverProfileDto>>
        GetById(int id)
    {
        var user = await _discoverService.GetByIdAsync(id);

        if (user == null)
        {
            return NotFound(new
            {
                message = "Không tìm thấy người dùng."
            });
        }

        return Ok(user);
    }
}