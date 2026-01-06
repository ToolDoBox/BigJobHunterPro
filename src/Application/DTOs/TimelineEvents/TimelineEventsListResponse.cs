namespace Application.DTOs.TimelineEvents;

public class TimelineEventsListResponse
{
    public Guid ApplicationId { get; set; }
    public List<TimelineEventDto> Events { get; set; } = new();
    public int TotalPoints { get; set; }
    public string CurrentStatus { get; set; } = string.Empty;
}
