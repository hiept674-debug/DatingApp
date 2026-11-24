
using DatingAppAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DatingAppAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly AppDbContext _context;

    public FilesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetFile(int id)
    {
        var claim = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (!int.TryParse(claim, out int userId))
            return Unauthorized();

        var file = await _context.StoredFiles
            .AsNoTracking()
            .FirstOrDefaultAsync(f =>
                f.Id == id &&
                f.OwnerId == userId &&
                f.Purpose == "Avatar" &&
                f.Owner != null &&
                f.Owner.IsActive);

        if (file == null)
            return NotFound();

        Response.Headers["X-Content-Type-Options"] =
            "nosniff";

        Response.Headers["Cache-Control"] =
            "private, no-store";

        return File(
            file.Content,
            file.ContentType);
    }
}
