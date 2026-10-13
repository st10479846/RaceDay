using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Filters;
using RaceDay.Api.Models;
using RaceDay.Contracts;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly RaceDayDbContext _db;

    public AuthController(RaceDayDbContext db)
    {
        _db = db;
    }

    // ============================================================
    // POST /api/auth/register
    // ============================================================
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest req)
    {
        // 1. Validate the request
        if (string.IsNullOrWhiteSpace(req.Email) ||
            string.IsNullOrWhiteSpace(req.Password) ||
            string.IsNullOrWhiteSpace(req.FirstName) ||
            string.IsNullOrWhiteSpace(req.LastName))
        {
            return BadRequest(new { error = "First name, last name, email, and password are required." });
        }

        if (req.Password.Length < 6)
        {
            return BadRequest(new { error = "Password must be at least 6 characters." });
        }

        // 2. Validate role
        if (req.Role != Roles.Organiser && req.Role != Roles.Participant)
        {
            return BadRequest(new { error = "Role must be 'Organiser' or 'Participant'." });
        }

        // 3. Check for duplicate email
        var normalisedEmail = req.Email.Trim().ToLowerInvariant();
        if (await _db.Users.AnyAsync(u => u.Email == normalisedEmail))
        {
            return Conflict(new { error = "Email is already registered." });
        }

        // 4. Hash the password and save the user
        var user = new AppUser
        {
            Email = normalisedEmail,
            FullName = $"{req.FirstName.Trim()} {req.LastName.Trim()}",
            PasswordHash = PasswordHasher.Hash(req.Password),
            Role = req.Role
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        // 5. Return 201 Created with a safe subset of user info
        return StatusCode(201, new AuthResponse(
            user.UserId,
            user.Email,
            user.FullName,
            user.Role));
    }

    // ============================================================
    // POST /api/auth/login
    // ============================================================
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Password))
        {
            return BadRequest(new { error = "Email and password are required." });
        }

        var normalisedEmail = req.Email.Trim().ToLowerInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == normalisedEmail);

        // Never reveal whether the email or password was wrong
        if (user == null || !PasswordHasher.Verify(req.Password, user.PasswordHash))
        {
            return Unauthorized(new { error = "Invalid email or password." });
        }

        if (!user.IsActive)
        {
            return Unauthorized(new { error = "Account is disabled." });
        }

        // Record last login
        user.LastLogin = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        // Create the session
        HttpContext.Session.SetUser(user.UserId, user.Role, user.FullName);

        return Ok(new AuthResponse(
            user.UserId,
            user.Email,
            user.FullName,
            user.Role));
    }

    // ============================================================
    // POST /api/auth/logout
    // ============================================================
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.ClearUser();
        return Ok(new { message = "Logged out successfully." });
    }
}