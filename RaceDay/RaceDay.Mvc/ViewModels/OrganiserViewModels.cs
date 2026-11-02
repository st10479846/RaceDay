using RaceDay.Contracts;

namespace RaceDay.Mvc.ViewModels;

public class OrganiserDashboardVm
{
    public List<OrganiserEventSummaryVm> Events { get; set; } = new();
}

public class OrganiserEventSummaryVm
{
    public int EventId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

// New ViewModels for management
public class ManageEventVm
{
    public EventDto Event { get; set; } = null!;
    public List<CategoryDto> Categories { get; set; } = new();
    public List<EnrolmentDto> Enrolments { get; set; } = new();
}

public class CaptureResultVm
{
    public int EnrolId { get; set; }
    public string FinishTime { get; set; } = string.Empty;
    public int Position { get; set; }
    public string? AgeCategory { get; set; }
}