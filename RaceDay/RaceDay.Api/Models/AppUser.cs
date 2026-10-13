namespace RaceDay.Api.Models;

public class AppUser
{
    public int UserId { get; set; }

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Role { get; set; } = Roles.Participant;

    public string? PhoneNumber { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastLogin { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<Event> OrganizedEvents { get; set; } = new List<Event>();

    public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
}