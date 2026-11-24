
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using DatingAppAPI.DTOs.UploadFile;
using DatingAppAPI.DTOs;
using DatingAppAPI.DTOs.Auth;
using DatingAppAPI.DTOs.Common;
using DatingAppAPI.DTOs.Users;
using DatingAppAPI.Services;

namespace DatingAppAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAvatarService _avatarService;

    public UsersController(IUserService userService, IAvatarService avatarService)
    {
        _userService = userService;
        _avatarService = avatarService;
    }

    // GET: api/users/me
    [HttpGet("me")]
    public async Task<ActionResult<UserProfileDto>> GetMe()
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized(new
            {
                message = "Token không hợp lệ."
            });
        }

        var profile = await _userService
            .GetMyProfileAsync(userId.Value);

        if (profile == null)
        {
            return NotFound(new
            {
                message = "Không tìm thấy tài khoản đang hoạt động."
            });
        }

        return Ok(profile);
    }

    // PUT: api/users/me
    [HttpPut("me")]
    public async Task<ActionResult<UserProfileDto>> UpdateMe(
        [FromBody] UpdateProfileDto request)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized(new
            {
                message = "Token không hợp lệ."
            });
        }

        if (request.DateOfBirth == default ||
            request.DateOfBirth.Date > DateTime.UtcNow.Date)
        {
            return BadRequest(new
            {
                message = "Ngày sinh không hợp lệ."
            });
        }

        var profile = await _userService
            .UpdateMyProfileAsync(userId.Value, request);

        if (profile == null)
        {
            return NotFound(new
            {
                message = "Không tìm thấy tài khoản đang hoạt động."
            });
        }

        return Ok(profile);
    }

    // GET: api/users
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] UserQueryParameters queryParams)
    {
        var users = await _userService.GetAllAsync(queryParams);

        return Ok(users);
    }

    // GET: api/users/1
    [Authorize(Roles = "Admin")]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserResponseDto>> GetById(
        int id)
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

    // POST: api/users/register
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] UserRegisterDto request)
    {
        var result = await _userService.RegisterAsync(request);

        if (!result.IsSuccess)
        {
            return BadRequest(new
            {
                message = result.Message
            });
        }

        return StatusCode(201, new
        {
            message = "Đăng ký thành công. Vui lòng đăng nhập."
        });
    }

    // DELETE: api/users/1
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _userService.DeleteAsync(id);

        if (!result.IsSuccess)
        {
            return NotFound(new
            {
                message = result.Message
            });
        }

        return Ok(new
        {
            message = result.Message
        });
    }

    // Lấy UserId từ JWT
    private int? GetCurrentUserId()
    {
        var claim = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (int.TryParse(claim, out int userId) &&
            userId > 0)
        {
            return userId;
        }

        return null;
    }

    [HttpPost("me/avatar")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(1024 * 1024)]
    public async Task<IActionResult> UploadAvatar(
    [FromForm] FileUploadDbDto.UploadFileDto dto)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
            return Unauthorized();

        try
        {
            var avatarUrl = await _avatarService
                .UploadAvatarAsync(userId.Value, dto.File);

            if (avatarUrl == null)
                return NotFound();

            return Ok(new
            {
                message = "Cập nhật ảnh đại diện thành công.",
                avatarUrl
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpDelete("me/avatar")]
    public async Task<IActionResult> DeleteAvatar()
    {
        var userId = GetCurrentUserId();

        if (userId == null)
            return Unauthorized();

        var result = await _avatarService
            .DeleteAvatarAsync(userId.Value);

        if (!result)
            return NotFound();

        return Ok(new
        {
            message = "Đã xóa ảnh đại diện.",
            avatarUrl = (string?)null
        });
    }


}
