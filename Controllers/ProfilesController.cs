using DatingAppAPI.Data;
using DatingAppAPI.DTOs;
using DatingAppAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingAppAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfilesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProfilesController(AppDbContext context)
    {
        _context = context;
    }


    // ==========================
    // GET ALL PROFILES
    // GET: api/profiles
    // ==========================
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProfileDto>>> GetProfiles()
    {
        var profiles = await _context.Profiles
            .Include(x => x.Photos)
            .Select(profile => new ProfileDto
            {
                Id = profile.Id,
                UserId = profile.UserId,
                FullName = profile.FullName,
                DateOfBirth = profile.DateOfBirth,
                Gender = profile.Gender,
                Bio = profile.Bio,
                City = profile.City,
                Job = profile.Job,
                Education = profile.Education,
                Interests = profile.Interests,
                CreatedAt = profile.CreatedAt,
                UpdatedAt = profile.UpdatedAt,

                Photos = profile.Photos
                    .Select(photo => new PhotoDto
                    {
                        Id = photo.Id,
                        ProfileId = photo.ProfileId,
                        Url = photo.Url,
                        IsMain = photo.IsMain,
                        CreatedAt = photo.CreatedAt
                    })
                    .ToList()
            })
            .ToListAsync();

        return Ok(profiles);
    }


    // ==========================
    // GET PROFILE BY ID
    // GET: api/profiles/1
    // ==========================
    [HttpGet("{id}")]
    public async Task<ActionResult<ProfileDto>> GetProfile(int id)
    {
        var profile = await _context.Profiles
            .Include(x => x.Photos)
            .Where(x => x.Id == id)
            .Select(profile => new ProfileDto
            {
                Id = profile.Id,
                UserId = profile.UserId,
                FullName = profile.FullName,
                DateOfBirth = profile.DateOfBirth,
                Gender = profile.Gender,
                Bio = profile.Bio,
                City = profile.City,
                Job = profile.Job,
                Education = profile.Education,
                Interests = profile.Interests,
                CreatedAt = profile.CreatedAt,
                UpdatedAt = profile.UpdatedAt,

                Photos = profile.Photos
                    .Select(photo => new PhotoDto
                    {
                        Id = photo.Id,
                        ProfileId = photo.ProfileId,
                        Url = photo.Url,
                        IsMain = photo.IsMain,
                        CreatedAt = photo.CreatedAt
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (profile == null)
        {
            return NotFound(new
            {
                message = "Không tìm thấy profile."
            });
        }

        return Ok(profile);
    }


    // ==========================
    // CREATE PROFILE
    // POST: api/profiles
    // ==========================
    [HttpPost]
    public async Task<ActionResult> CreateProfile(Profile profile)
    {
        var userExists = await _context.Users
            .AnyAsync(x => x.Id == profile.UserId);

        if (!userExists)
        {
            return BadRequest(new
            {
                message = "User không tồn tại."
            });
        }

        var profileExists = await _context.Profiles
            .AnyAsync(x => x.UserId == profile.UserId);

        if (profileExists)
        {
            return BadRequest(new
            {
                message = "User đã có profile."
            });
        }

        profile.Id = 0;
        profile.CreatedAt = DateTime.UtcNow;

        _context.Profiles.Add(profile);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Tạo profile thành công.",
            profileId = profile.Id
        });
    }


    // ==========================
    // UPDATE PROFILE
    // PUT: api/profiles/1
    // ==========================
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateProfile(
        int id,
        Profile profile)
    {
        var existingProfile = await _context.Profiles
            .FirstOrDefaultAsync(x => x.Id == id);

        if (existingProfile == null)
        {
            return NotFound(new
            {
                message = "Không tìm thấy profile."
            });
        }

        existingProfile.FullName = profile.FullName;
        existingProfile.DateOfBirth = profile.DateOfBirth;
        existingProfile.Gender = profile.Gender;
        existingProfile.Bio = profile.Bio;
        existingProfile.City = profile.City;
        existingProfile.Job = profile.Job;
        existingProfile.Education = profile.Education;
        existingProfile.Interests = profile.Interests;
        existingProfile.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Cập nhật profile thành công."
        });
    }


    // ==========================
    // DELETE PROFILE
    // DELETE: api/profiles/1
    // ==========================
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProfile(int id)
    {
        var profile = await _context.Profiles
            .FirstOrDefaultAsync(x => x.Id == id);

        if (profile == null)
        {
            return NotFound(new
            {
                message = "Không tìm thấy profile."
            });
        }

        _context.Profiles.Remove(profile);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Xóa profile thành công."
        });
    }
}