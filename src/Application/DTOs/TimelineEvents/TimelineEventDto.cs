namespace Application.DTOs.TimelineEvents;

public class TimelineEventDto
{
    public Guid Id { get; set; }
    public Guid ApplicationId { get; set; }
    public string EventType { get; set; } = string.Empty;  // String for frontend
    public int? InterviewRound { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Notes { get; set; }
    public int Points { get; set; }
    public DateTime CreatedDate { get; set; }
}
