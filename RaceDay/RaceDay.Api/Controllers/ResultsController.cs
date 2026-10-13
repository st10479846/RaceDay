using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Filters;
using RaceDay.Api.Models;
using RaceDay.Contracts;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/results")]
public class ResultsController : ControllerBase
{
    private readonly RaceDayDbContext _db;

    public ResultsController(RaceDayDbContext db)
    {
        _db = db;
    }

    // ============================================================
    // POST /api/results — organiser owner only
    // ============================================================
    [HttpPost]
    public async Task<IActionResult> Create(CreateResultRequest req)
    {
        if (HttpContext.Session.GetRole() != Roles.Organiser)
            return StatusCode(403, new { error = "Only organisers can capture results." });

        var userId = HttpContext.Session.GetUserId()!.Value;

        // 1. Load enrolment + related event so we can verify ownership
        var enrolment = await _db.Enrolments
            .Include(e => e.Event)
            .FirstOrDefaultAsync(e => e.EnrolId == req.EnrolId);

        if (enrolment == null)
            return NotFound(new { error = "Enrolment not found." });

        // 2. The organiser must own the event this enrolment belongs to
        if (enrolment.Event!.OrganizerId != userId)
            return StatusCode(403, new
            {
                error = "You can only capture results for your own events."
            });

        // 3. Only confirmed/completed enrolments can receive results
        if (enrolment.Status != "Confirmed" && enrolment.Status != "Completed")
            return BadRequest(new
            {
                error = "Results can only be captured for Confirmed or Completed enrolments."
            });

        // 4. Validate inputs
        if (string.IsNullOrWhiteSpace(req.FinishTime))
            return BadRequest(new { error = "Finish time is required." });

        if (req.Position <= 0)
            return BadRequest(new { error = "Position must be greater than zero." });

        // 5. One result per enrolment
        if (await _db.Results.AnyAsync(r => r.EnrolId == req.EnrolId))
            return Conflict(new { error = "A result has already been captured for this enrolment." });

        var result = new Result
        {
            EnrolId = req.EnrolId,
            FinishTime = req.FinishTime.Trim(),
            Position = req.Position,
            AgeCategory = req.AgeCategory?.Trim(),
            IsVerified = true,
            CapturedAt = DateTime.UtcNow
        };

        _db.Results.Add(result);

        // Mark enrolment as Completed (nice touch — reflects the state machine)
        enrolment.Status = "Completed";

        await _db.SaveChangesAsync();

        return StatusCode(201, ToDto(result));
    }

    // ============================================================
    // GET /api/results/mine — participant sees own results
    // ============================================================
    [HttpGet("mine")]
    public async Task<IActionResult> GetMine()
    {
        var userId = HttpContext.Session.GetUserId()!.Value;

        var results = await _db.Results
            .AsNoTracking()
            .Include(r => r.Enrolment)
            .Where(r => r.Enrolment!.UserId == userId)
            .OrderByDescending(r => r.CapturedAt)
            .Select(r => ToDto(r))
            .ToListAsync();

        return Ok(results);
    }

    // ============================================================
    // GET /api/results/event/{eventId} — organiser owner only
    // ============================================================
    [HttpGet("event/{eventId:int}")]
    public async Task<IActionResult> GetForEvent(int eventId)
    {
        if (HttpContext.Session.GetRole() != Roles.Organiser)
            return StatusCode(403, new { error = "Only organisers can view event results." });

        var userId = HttpContext.Session.GetUserId()!.Value;

        var ev = await _db.Events.FindAsync(eventId);
        if (ev == null) return NotFound(new { error = "Event not found." });

        if (ev.OrganizerId != userId)
            return StatusCode(403, new
            {
                error = "You can only view results for your own events."
            });

        var results = await _db.Results
            .AsNoTracking()
            .Include(r => r.Enrolment)
            .Where(r => r.Enrolment!.EventId == eventId)
            .OrderBy(r => r.Position)
            .Select(r => ToDto(r))
            .ToListAsync();

        return Ok(results);
    }

    // ============================================================
    // Helper
    // ============================================================
    private static ResultDto ToDto(Result r) => new(
        r.ResultId,
        r.EnrolId,
        r.FinishTime,
        r.Position,
        r.AgeCategory,
        r.IsVerified,
        r.CapturedAt);
}