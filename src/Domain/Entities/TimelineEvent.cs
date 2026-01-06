using Domain.Enums;

namespace Domain.Entities;

public class TimelineEvent
{
    public Guid Id { get; set; }
    public Guid ApplicationId { get; set; }
    public EventType EventType { get; set; }
    public int? InterviewRound { get; set; }  // Only for Interview events
    public DateTime Timestamp { get; set; }
    public string? Notes { get; set; }
    public int Points { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }

    // Navigation property
    public Application Application { get; set; } = null!;
}
