using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Filters;
using RaceDay.Api.Models;
using RaceDay.Contracts;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/enrolments")]
public class EnrolmentsController : ControllerBase
{
    private static readonly string[] AllowedStatuses =
        { "Pending", "Confirmed", "Cancelled", "Completed" };

    private readonly RaceDayDbContext _db;

    public EnrolmentsController(RaceDayDbContext db)
    {
        _db = db;
    }

    // ============================================================
    // POST /api/enrolments — participant only
    // ============================================================
    [HttpPost]
    public async Task<IActionResult> Create(CreateEnrolmentRequest req)
    {
        if (HttpContext.Session.GetRole() != Roles.Participant)
            return StatusCode(403, new { error = "Only participants can enrol in events." });

        var userId = HttpContext.Session.GetUserId()!.Value;

        // 1. Event must exist
        var ev = await _db.Events.FindAsync(req.EventId);
        if (ev == null) return NotFound(new { error = "Event not found." });

        // 2. Event must accept enrolments
        if (ev.Status != "Open")
            return BadRequest(new { error = "This event is not currently open for enrolments." });

        // 3. Category must exist AND belong to the event
        var category = await _db.Categories.FindAsync(req.CategoryId);
        if (category == null)
            return BadRequest(new { error = "Category not found." });

        if (category.EventId != req.EventId)
            return BadRequest(new { error = "Category does not belong to this event." });

        // 4. Prevent duplicate enrolment (also enforced by a unique index)
        var already = await _db.Enrolments
            .AnyAsync(e => e.UserId == userId && e.EventId == req.EventId);

        if (already)
            return Conflict(new { error = "You are already enrolled in this event." });

        // 5. Create the enrolment
        var enrolment = new Enrolment
        {
            UserId = userId,
            EventId = req.EventId,
            CategoryId = req.CategoryId,
            Status = "Pending",
            EntryDate = DateTime.UtcNow
        };

        _db.Enrolments.Add(enrolment);
        await _db.SaveChangesAsync();

        return StatusCode(201, ToDto(enrolment));
    }

    // ============================================================
    // GET /api/enrolments/mine — any logged-in user
    // ============================================================
    [HttpGet("mine")]
    public async Task<IActionResult> GetMine()
    {
        var userId = HttpContext.Session.GetUserId()!.Value;

        var enrolments = await _db.Enrolments
            .AsNoTracking()
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.EntryDate)
            .Select(e => ToDto(e))
            .ToListAsync();

        return Ok(enrolments);
    }

    // ============================================================
    // GET /api/enrolments/event/{eventId} — organiser owner only
    // ============================================================
    [HttpGet("event/{eventId:int}")]
    public async Task<IActionResult> GetForEvent(int eventId)
    {
        if (HttpContext.Session.GetRole() != Roles.Organiser)
            return StatusCode(403, new { error = "Only organisers can view event enrolments." });

        var userId = HttpContext.Session.GetUserId()!.Value;

        var ev = await _db.Events.FindAsync(eventId);
        if (ev == null) return NotFound(new { error = "Event not found." });

        if (ev.OrganizerId != userId)
            return StatusCode(403, new { error = "You can only view enrolments for your own events." });

        var enrolments = await _db.Enrolments
            .AsNoTracking()
            .Where(e => e.EventId == eventId)
            .OrderBy(e => e.EntryDate)
            .Select(e => ToDto(e))
            .ToListAsync();

        return Ok(enrolments);
    }

    // ============================================================
    // PUT /api/enrolments/{id}/status — organiser owner only
    // ============================================================
    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, UpdateEnrolmentStatusRequest req)
    {
        if (HttpContext.Session.GetRole() != Roles.Organiser)
            return StatusCode(403, new { error = "Only organisers can update enrolment status." });

        var userId = HttpContext.Session.GetUserId()!.Value;

        // Validate status
        if (string.IsNullOrWhiteSpace(req.Status) || !AllowedStatuses.Contains(req.Status))
        {
            return BadRequest(new
            {
                error = $"Status must be one of: {string.Join(", ", AllowedStatuses)}."
            });
        }

        var enrolment = await _db.Enrolments
            .Include(e => e.Event)
            .FirstOrDefaultAsync(e => e.EnrolId == id);

        if (enrolment == null) return NotFound(new { error = "Enrolment not found." });

        if (enrolment.Event!.OrganizerId != userId)
            return StatusCode(403, new { error = "You can only update enrolments for your own events." });

        enrolment.Status = req.Status;
        if (!string.IsNullOrWhiteSpace(req.BibNumber))
            enrolment.BibNumber = req.BibNumber.Trim();

        await _db.SaveChangesAsync();

        return Ok(ToDto(enrolment));
    }

    // ============================================================
    // Helper
    // ============================================================
    private static EnrolmentDto ToDto(Enrolment e) => new(
        e.EnrolId,
        e.UserId,
        e.EventId,
        e.CategoryId,
        e.Status,
        e.BibNumber,
        e.EntryDate);
}