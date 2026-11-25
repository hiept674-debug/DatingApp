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
    public async Task<ActionResult<DiscoverResponseDto>>
        GetProfiles(
            [FromQuery] DiscoverQueryParameters parameters)
    {
        var result = await _discoverService
            .GetProfilesAsync(parameters);

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