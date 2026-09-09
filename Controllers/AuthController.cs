using DatingAppAPI.Data;
using DatingAppAPI.DTOs;
using DatingAppAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingAppAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;

    public AuthController(AppDbContext context)
    {
        _context = context;
    }

    // POST: api/auth/register
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(
        RegisterDto dto)
    {
        // Kiểm tra email
        if (await _context.Users.AnyAsync(x => x.Email == dto.Email))
        {
            return BadRequest("Email đã tồn tại.");
        }

        // Kiểm tra phone
        if (await _context.Users.AnyAsync(x => x.Phone == dto.Phone))
        {
            return BadRequest("Số điện thoại đã tồn tại.");
        }

        // Hash password
        string passwordHash =
            BCrypt.Net.BCrypt.HashPassword(dto.Password);

        var user = new User
        {
            Email = dto.Email,
            Phone = dto.Phone,
            PasswordHash = passwordHash,
            FullName = dto.FullName,
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            City = dto.City,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        // Response
        var response = new AuthResponseDto
        {
            Id = user.Id,
            Email = user.Email,
            Phone = user.Phone,
            FullName = user.FullName,
            Token = "JWT_TOKEN_HERE"
        };

        return Ok(response);
    }


    // POST: api/auth/login
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(
        LoginDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x =>
                x.Email == dto.EmailOrPhone ||
                x.Phone == dto.EmailOrPhone);

        if (user == null)
        {
            return Unauthorized("Email hoặc số điện thoại không tồn tại.");
        }

        // Kiểm tra password
        bool passwordValid =
            BCrypt.Net.BCrypt.Verify(
                dto.Password,
                user.PasswordHash);

        if (!passwordValid)
        {
            return Unauthorized("Mật khẩu không đúng.");
        }

        var response = new AuthResponseDto
        {
            Id = user.Id,
            Email = user.Email,
            Phone = user.Phone,
            FullName = user.FullName,
            Token = "JWT_TOKEN_HERE"
        };

        return Ok(response);
    }
}