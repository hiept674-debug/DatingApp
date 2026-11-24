using Microsoft.AspNetCore.Mvc;
using DatingAppAPI.DTOs;
using DatingAppAPI.DTOs.Auth;
using DatingAppAPI.Services;
using Microsoft.AspNetCore.Authorization;
using DatingAppAPI.DTOs.Common;
namespace DatingAppAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
    [FromQuery] UserQueryParameters queryParams)
    {
        var users = await _userService.GetAllAsync(queryParams);

        return Ok(users);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserResponseDto>> GetById(int id)
    {
        var user = await _userService.GetByIdAsync(id);

        if (user == null)
        {
            return NotFound(new
            {
                message = $"Không tìm thấy người dùng có Id = {id}"
            });
        }

        return Ok(user);
    }
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<UserResponseDto>> Register(
        [FromBody] UserRegisterDto request)
    {
        var result = await _userService.RegisterAsync(request);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Message });
        }

        return StatusCode(201, new
        {
            message = "Đăng ký thành công. Vui lòng đăng nhập."
        });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UserRegisterDto request)
    {
        var result = await _userService.UpdateAsync(id, request);

        if (!result.IsSuccess)
        {
            return NotFound(new { message = result.Message });
        }

        return Ok(new { message = result.Message });
    }
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _userService.DeleteAsync(id);

        if (!result.IsSuccess)
        {
            return NotFound(new { message = result.Message });
        }

        return Ok(new { message = result.Message });
    }
}