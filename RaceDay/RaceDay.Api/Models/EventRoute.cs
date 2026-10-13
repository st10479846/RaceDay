namespace RaceDay.Api.Models;

public class EventRoute
{
    public int RouteId { get; set; }

    public int EventId { get; set; }
    public Event? Event { get; set; }

    public string RouteName { get; set; } = string.Empty;

    public string StartPoint { get; set; } = string.Empty;

    public string EndPoint { get; set; } = string.Empty;

    public decimal DistanceKm { get; set; }

    public int? AltitudeGain { get; set; }
}