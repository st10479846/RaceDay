using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Filters;
using RaceDay.Api.Models;
using RaceDay.Contracts;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api")]
public class CategoriesController : ControllerBase
{
    private readonly RaceDayDbContext _db;

    public CategoriesController(RaceDayDbContext db)
    {
        _db = db;
    }

    // ============================================================
    // GET /api/events/{eventId}/categories — public
    // ============================================================
    [HttpGet("events/{eventId:int}/categories")]
    [AllowAnonymous]
    public async Task<IActionResult> GetForEvent(int eventId)
    {
        var ev = await _db.Events.FindAsync(eventId);
        if (ev == null) return NotFound(new { error = "Event not found." });

        var categories = await _db.Categories
            .AsNoTracking()
            .Where(c => c.EventId == eventId)
            .OrderBy(c => c.DistanceKm)
            .Select(c => ToDto(c))
            .ToListAsync();

        return Ok(categories);
    }

    // ============================================================
    // POST /api/events/{eventId}/categories — organiser owner only
    // ============================================================
    [HttpPost("events/{eventId:int}/categories")]
    public async Task<IActionResult> Create(int eventId, CreateCategoryRequest req)
    {
        if (HttpContext.Session.GetRole() != Roles.Organiser)
            return StatusCode(403, new { error = "Only organisers can create categories." });

        var userId = HttpContext.Session.GetUserId()!.Value;

        var ev = await _db.Events.FindAsync(eventId);
        if (ev == null) return NotFound(new { error = "Event not found." });

        if (ev.OrganizerId != userId)
            return StatusCode(403, new { error = "You can only add categories to your own events." });

        // Validate
        if (string.IsNullOrWhiteSpace(req.Name))
            return BadRequest(new { error = "Category name is required." });

        if (req.DistanceKm <= 0)
            return BadRequest(new { error = "Distance must be greater than zero." });

        if (req.EntryFee < 0)
            return BadRequest(new { error = "Entry fee cannot be negative." });

        var category = new Category
        {
            EventId = eventId,
            Name = req.Name.Trim(),
            Description = req.Description?.Trim(),
            DistanceKm = req.DistanceKm,
            EntryFee = req.EntryFee,
            StartTime = req.StartTime,
            MaxParticipants = req.MaxParticipants
        };

        _db.Categories.Add(category);
        await _db.SaveChangesAsync();

        return StatusCode(201, ToDto(category));
    }

    // ============================================================
    // PUT /api/categories/{id} — organiser owner only
    // ============================================================
    [HttpPut("categories/{id:int}")]
    public async Task<IActionResult> Update(int id, CreateCategoryRequest req)
    {
        if (HttpContext.Session.GetRole() != Roles.Organiser)
            return StatusCode(403, new { error = "Only organisers can update categories." });

        var userId = HttpContext.Session.GetUserId()!.Value;

        var category = await _db.Categories
            .Include(c => c.Event)
            .FirstOrDefaultAsync(c => c.CategoryId == id);

        if (category == null) return NotFound(new { error = "Category not found." });

        if (category.Event!.OrganizerId != userId)
            return StatusCode(403, new { error = "You can only update categories for your own events." });

        if (string.IsNullOrWhiteSpace(req.Name))
            return BadRequest(new { error = "Category name is required." });

        if (req.DistanceKm <= 0)
            return BadRequest(new { error = "Distance must be greater than zero." });

        if (req.EntryFee < 0)
            return BadRequest(new { error = "Entry fee cannot be negative." });

        category.Name = req.Name.Trim();
        category.Description = req.Description?.Trim();
        category.DistanceKm = req.DistanceKm;
        category.EntryFee = req.EntryFee;
        category.StartTime = req.StartTime;
        category.MaxParticipants = req.MaxParticipants;

        await _db.SaveChangesAsync();

        return Ok(ToDto(category));
    }

    // ============================================================
    // DELETE /api/categories/{id} — organiser owner only
    // ============================================================
    [HttpDelete("categories/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (HttpContext.Session.GetRole() != Roles.Organiser)
            return StatusCode(403, new { error = "Only organisers can delete categories." });

        var userId = HttpContext.Session.GetUserId()!.Value;

        var category = await _db.Categories
            .Include(c => c.Event)
            .FirstOrDefaultAsync(c => c.CategoryId == id);

        if (category == null) return NotFound(new { error = "Category not found." });

        if (category.Event!.OrganizerId != userId)
            return StatusCode(403, new { error = "You can only delete categories for your own events." });

        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    // ============================================================
    // Helper
    // ============================================================
    private static CategoryDto ToDto(Category c) => new(
        c.CategoryId,
        c.EventId,
        c.Name,
        c.Description,
        c.DistanceKm,
        c.EntryFee,
        c.StartTime,
        c.MaxParticipants);
}