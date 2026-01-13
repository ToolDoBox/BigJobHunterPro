using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.DTOs.TimelineEvents;

public class UpdateTimelineEventRequest
{
    [Required]
    public EventType EventType { get; set; }

    public int? InterviewRound { get; set; }

    [Required]
    public DateTime Timestamp { get; set; }

    [MaxLength(5000)]
    public string? Notes { get; set; }
}
