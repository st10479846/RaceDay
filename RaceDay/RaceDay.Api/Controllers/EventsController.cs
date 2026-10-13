using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Filters;
using RaceDay.Api.Models;
using RaceDay.Contracts;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/events")]
public class EventsController : ControllerBase
{
    private readonly RaceDayDbContext _db;

    public EventsController(RaceDayDbContext db)
    {
        _db = db;
    }

    // ============================================================
    // GET /api/events — anyone may view events
    // ============================================================
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var events = await _db.Events
            .AsNoTracking()
            .OrderBy(e => e.StartDate)
            .Select(e => ToDto(e))
            .ToListAsync();

        return Ok(events);
    }

    // ============================================================
    // GET /api/events/{id} — anyone may view a single event
    // ============================================================
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var e = await _db.Events.AsNoTracking().FirstOrDefaultAsync(x => x.EventId == id);
        if (e == null) return NotFound(new { error = "Event not found." });

        return Ok(ToDto(e));
    }

    // ============================================================
    // GET /api/events/mine — organiser only, own events
    // ============================================================
    [HttpGet("mine")]
    public async Task<IActionResult> GetMine()
    {
        if (HttpContext.Session.GetRole() != Roles.Organiser)
            return StatusCode(403, new { error = "Only organisers can view their events." });

        var userId = HttpContext.Session.GetUserId()!.Value;

        var events = await _db.Events
            .AsNoTracking()
            .Where(e => e.OrganizerId == userId)
            .OrderBy(e => e.StartDate)
            .Select(e => ToDto(e))
            .ToListAsync();

        return Ok(events);
    }

    // ============================================================
    // POST /api/events — organiser only, creates an event
    // ============================================================
    [HttpPost]
    public async Task<IActionResult> Create(CreateEventRequest req)
    {
        if (HttpContext.Session.GetRole() != Roles.Organiser)
            return StatusCode(403, new { error = "Only organisers can create events." });

        // Validate
        if (string.IsNullOrWhiteSpace(req.Title) ||
            string.IsNullOrWhiteSpace(req.Location) ||
            string.IsNullOrWhiteSpace(req.EventType))
        {
            return BadRequest(new { error = "Title, location, and event type are required." });
        }

        if (req.EndDate < req.StartDate)
            return BadRequest(new { error = "End date cannot be before start date." });

        if (req.DistanceKm <= 0)
            return BadRequest(new { error = "Distance must be greater than zero." });

        var userId = HttpContext.Session.GetUserId()!.Value;

        var e = new Event
        {
            OrganizerId = userId,
            Title = req.Title.Trim(),
            Description = req.Description?.Trim(),
            StartDate = req.StartDate,
            EndDate = req.EndDate,
            Location = req.Location.Trim(),
            DistanceKm = req.DistanceKm,
            EventType = req.EventType.Trim(),
            Status = "Draft"
        };

        _db.Events.Add(e);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = e.EventId }, ToDto(e));
    }

    // ============================================================
    // PUT /api/events/{id} — organiser must own the event
    // ============================================================
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateEventRequest req)
    {
        if (HttpContext.Session.GetRole() != Roles.Organiser)
            return StatusCode(403, new { error = "Only organisers can update events." });

        var userId = HttpContext.Session.GetUserId()!.Value;

        var e = await _db.Events.FindAsync(id);
        if (e == null) return NotFound(new { error = "Event not found." });

        if (e.OrganizerId != userId)
            return StatusCode(403, new { error = "You can only update your own events." });

        if (string.IsNullOrWhiteSpace(req.Title) ||
            string.IsNullOrWhiteSpace(req.Location) ||
            string.IsNullOrWhiteSpace(req.EventType))
        {
            return BadRequest(new { error = "Title, location, and event type are required." });
        }

        if (req.EndDate < req.StartDate)
            return BadRequest(new { error = "End date cannot be before start date." });

        if (req.DistanceKm <= 0)
            return BadRequest(new { error = "Distance must be greater than zero." });

        e.Title = req.Title.Trim();
        e.Description = req.Description?.Trim();
        e.StartDate = req.StartDate;
        e.EndDate = req.EndDate;
        e.Location = req.Location.Trim();
        e.DistanceKm = req.DistanceKm;
        e.EventType = req.EventType.Trim();
        e.Status = req.Status;

        await _db.SaveChangesAsync();

        return Ok(ToDto(e));
    }

    // ============================================================
    // DELETE /api/events/{id} — organiser must own the event
    // ============================================================
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (HttpContext.Session.GetRole() != Roles.Organiser)
            return StatusCode(403, new { error = "Only organisers can delete events." });

        var userId = HttpContext.Session.GetUserId()!.Value;

        var e = await _db.Events.FindAsync(id);
        if (e == null) return NotFound(new { error = "Event not found." });

        if (e.OrganizerId != userId)
            return StatusCode(403, new { error = "You can only delete your own events." });

        _db.Events.Remove(e);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    // ============================================================
    // Helper — maps Event entity → EventDto
    // ============================================================
    private static EventDto ToDto(Event e) => new(
        e.EventId,
        e.Title,
        e.Description ?? string.Empty,
        e.StartDate,
        e.EndDate,
        e.Location,
        e.DistanceKm,
        e.EventType,
        e.Status,
        e.OrganizerId);
}