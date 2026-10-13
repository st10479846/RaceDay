namespace RaceDay.Api.Models;

public class Category
{
    public int CategoryId { get; set; }

    public int EventId { get; set; }
    public Event? Event { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal DistanceKm { get; set; }

    public decimal EntryFee { get; set; }

    public TimeOnly StartTime { get; set; }

    public int? MaxParticipants { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
}