namespace RaceDay.Api.Models;

public class Event
{
    public int EventId { get; set; }

    public int OrganizerId { get; set; }
    public AppUser? Organizer { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string Location { get; set; } = string.Empty;

    // REQUIRED (feedback fix)
    public decimal DistanceKm { get; set; }

    // REQUIRED (feedback fix): Road / Trail / Cycling etc.
    public string EventType { get; set; } = "Road";

    public string Status { get; set; } = "Draft";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<Category> Categories { get; set; } = new List<Category>();

    public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();

    public ICollection<EventRoute> Routes { get; set; } = new List<EventRoute>();
}