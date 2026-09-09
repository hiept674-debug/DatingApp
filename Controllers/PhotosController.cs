using DatingAppAPI.Data;
using DatingAppAPI.DTOs;
using DatingAppAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingAppAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PhotosController : ControllerBase
{
    private readonly AppDbContext _context;

    public PhotosController(AppDbContext context)
    {
        _context = context;
    }


    // ==========================
    // GET PHOTOS BY PROFILE
    // GET: api/photos/profile/1
    // ==========================
    [HttpGet("profile/{profileId}")]
    public async Task<ActionResult<IEnumerable<PhotoDto>>> GetPhotos(
        int profileId)
    {
        var photos = await _context.Photos
            .Where(x => x.ProfileId == profileId)
            .Select(photo => new PhotoDto
            {
                Id = photo.Id,
                ProfileId = photo.ProfileId,
                Url = photo.Url,
                IsMain = photo.IsMain,
                CreatedAt = photo.CreatedAt
            })
            .ToListAsync();

        return Ok(photos);
    }


    // ==========================
    // ADD PHOTO
    // POST: api/photos
    // ==========================
    [HttpPost]
    public async Task<ActionResult> AddPhoto(Photo photo)
    {
        var profileExists = await _context.Profiles
            .AnyAsync(x => x.Id == photo.ProfileId);

        if (!profileExists)
        {
            return BadRequest(new
            {
                message = "Profile không tồn tại."
            });
        }

        photo.Id = 0;
        photo.CreatedAt = DateTime.UtcNow;

        // Nếu đây là ảnh đầu tiên
        var hasPhoto = await _context.Photos
            .AnyAsync(x => x.ProfileId == photo.ProfileId);

        if (!hasPhoto)
        {
            photo.IsMain = true;
        }

        // Nếu chọn ảnh này làm main
        if (photo.IsMain)
        {
            var oldMainPhotos = await _context.Photos
                .Where(x =>
                    x.ProfileId == photo.ProfileId &&
                    x.IsMain)
                .ToListAsync();

            foreach (var oldPhoto in oldMainPhotos)
            {
                oldPhoto.IsMain = false;
            }
        }

        _context.Photos.Add(photo);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Thêm ảnh thành công.",
            photoId = photo.Id
        });
    }


    // ==========================
    // SET MAIN PHOTO
    // PUT: api/photos/1/main
    // ==========================
    [HttpPut("{id}/main")]
    public async Task<ActionResult> SetMainPhoto(int id)
    {
        var photo = await _context.Photos
            .FirstOrDefaultAsync(x => x.Id == id);

        if (photo == null)
        {
            return NotFound(new
            {
                message = "Không tìm thấy ảnh."
            });
        }

        var oldMainPhotos = await _context.Photos
            .Where(x =>
                x.ProfileId == photo.ProfileId &&
                x.IsMain)
            .ToListAsync();

        foreach (var oldPhoto in oldMainPhotos)
        {
            oldPhoto.IsMain = false;
        }

        photo.IsMain = true;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Đặt ảnh chính thành công."
        });
    }


    // ==========================
    // DELETE PHOTO
    // DELETE: api/photos/1
    // ==========================
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletePhoto(int id)
    {
        var photo = await _context.Photos
            .FirstOrDefaultAsync(x => x.Id == id);

        if (photo == null)
        {
            return NotFound(new
            {
                message = "Không tìm thấy ảnh."
            });
        }

        _context.Photos.Remove(photo);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Xóa ảnh thành công."
        });
    }
}