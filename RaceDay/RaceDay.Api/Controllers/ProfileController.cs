using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Filters;
using RaceDay.Contracts;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/profile")]
public class ProfileController : ControllerBase
{
    private readonly RaceDayDbContext _db;

    public ProfileController(RaceDayDbContext db)
    {
        _db = db;
    }

    // ============================================================
    // GET /api/profile/me
    // ============================================================
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userId = HttpContext.Session.GetUserId();
        if (userId == null) return Unauthorized();

        var user = await _db.Users.FindAsync(userId.Value);
        if (user == null) return NotFound();

        return Ok(new ProfileDto(
            user.UserId,
            user.FullName,
            user.Email,
            user.Role,
            user.PhoneNumber));
    }

    // ============================================================
    // PUT /api/profile/me
    // ============================================================
    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe(UpdateProfileRequest req)
    {
        var userId = HttpContext.Session.GetUserId();
        if (userId == null) return Unauthorized();

        if (string.IsNullOrWhiteSpace(req.FullName))
        {
            return BadRequest(new { error = "Full name is required." });
        }

        var user = await _db.Users.FindAsync(userId.Value);
        if (user == null) return NotFound();

        user.FullName = req.FullName.Trim();
        user.PhoneNumber = req.PhoneNumber?.Trim();

        await _db.SaveChangesAsync();

        return Ok(new ProfileDto(
            user.UserId,
            user.FullName,
            user.Email,
            user.Role,
            user.PhoneNumber));
    }
}