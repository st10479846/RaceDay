namespace RaceDay.Api.Models;

public class Enrolment
{
    public int EnrolId { get; set; }

    public int UserId { get; set; }
    public AppUser? User { get; set; }

    public int EventId { get; set; }
    public Event? Event { get; set; }

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public string Status { get; set; } = "Pending";

    public DateTime EntryDate { get; set; } = DateTime.UtcNow;

    public string? BibNumber { get; set; }

    public string? EmergencyContact { get; set; }

    public string? EmergencyPhone { get; set; }

    // One-to-zero-or-one with Result (feedback fix)
    public Result? Result { get; set; }
}