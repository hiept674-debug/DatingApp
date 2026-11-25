using System.Security.Claims;
using DatingAppAPI.DTOs.Interests;
using DatingAppAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingAppAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class InterestsController : ControllerBase
{
    private readonly IInterestService _interestService;

    public InterestsController(
        IInterestService interestService)
    {
        _interestService = interestService;
    }

    // GET: api/interests
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _interestService
            .GetAllAsync();

        return Ok(result);
    }

    // GET: api/interests/me
    [HttpGet("me")]
    public async Task<IActionResult> GetMyInterests()
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _interestService
            .GetMyInterestsAsync(userId.Value);

        if (result == null)
        {
            return NotFound(new
            {
                message = "Không tìm thấy tài khoản."
            });
        }

        return Ok(result);
    }

    // PUT: api/interests/me
    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyInterests(
        [FromBody] UpdateUserInterestsDto request)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _interestService
            .UpdateMyInterestsAsync(
                userId.Value,
                request);

        if (!result.IsSuccess)
        {
            return BadRequest(new
            {
                message = result.Message
            });
        }

        return Ok(new
        {
            message = result.Message
        });
    }

    // GET: api/interests/admin
    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    public async Task<IActionResult> GetAllForAdmin()
    {
        var result = await _interestService
            .GetAllAsync(includeInactive: true);

        return Ok(result);
    }

    // POST: api/interests
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateInterestDto request)
    {
        var result = await _interestService
            .CreateAsync(request);

        if (!result.IsSuccess)
        {
            return BadRequest(new
            {
                message = result.Message
            });
        }

        return StatusCode(
            StatusCodes.Status201Created,
            new
            {
                message = result.Message,
                data = result.Data
            });
    }

    // PUT: api/interests/5
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateInterestDto request)
    {
        var result = await _interestService
            .UpdateAsync(id, request);

        if (!result.IsSuccess)
        {
            return BadRequest(new
            {
                message = result.Message
            });
        }

        return Ok(new
        {
            message = result.Message
        });
    }

    // DELETE: api/interests/5
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _interestService
            .DeleteAsync(id);

        if (!result.IsSuccess)
        {
            return BadRequest(new
            {
                message = result.Message
            });
        }

        return Ok(new
        {
            message = result.Message
        });
    }

    private int? GetCurrentUserId()
    {
        var claim = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (int.TryParse(claim, out int userId)
            && userId > 0)
        {
            return userId;
        }

        return null;
    }
}