namespace RaceDay.Api.Models;

public class Result
{
    public int ResultId { get; set; }

    // FK to Enrolment (feedback fix)
    public int EnrolId { get; set; }
    public Enrolment? Enrolment { get; set; }

    public string FinishTime { get; set; } = string.Empty;

    public int Position { get; set; }

    public string? AgeCategory { get; set; }

    public bool IsVerified { get; set; }

    public DateTime CapturedAt { get; set; } = DateTime.UtcNow;
}