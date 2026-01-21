// src/Domain/Entities/Application.cs
using Domain.Enums;

namespace Domain.Entities;

public class Application
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string RoleTitle { get; set; } = string.Empty;
    public string SourceName { get; set; } = string.Empty;
    public string? SourceUrl { get; set; }
    public ApplicationStatus Status { get; set; }
    public WorkMode? WorkMode { get; set; }
    public string? Location { get; set; }
    public int? SalaryMin { get; set; }
    public int? SalaryMax { get; set; }
    public string? JobDescription { get; set; }
    public List<string> RequiredSkills { get; set; } = new();
    public List<string> NiceToHaveSkills { get; set; } = new();
    public bool ParsedByAI { get; set; }
    public AiParsingStatus AiParsingStatus { get; set; } = AiParsingStatus.Pending;
    public DateTime? LastAIParsedDate { get; set; }
    public string? RawPageContent { get; set; }
    public int Points { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public string? CoverLetterHtml { get; set; }
    public DateTime? CoverLetterGeneratedAt { get; set; }

    // Navigation properties
    public ApplicationUser User { get; set; } = null!;
    public List<TimelineEvent> TimelineEvents { get; set; } = new();
    public List<Contact> Contacts { get; set; } = new();

    // Computed properties for performance
    public ApplicationStatus ComputeCurrentStatus()
    {
        var latestEvent = TimelineEvents
            .Where(e => e.EventType != EventType.Prospecting)
            .OrderByDescending(e => e.Timestamp)
            .ThenByDescending(e => e.CreatedDate)
            .ThenByDescending(e => e.Id)
            .FirstOrDefault();

        if (latestEvent == null)
            return ApplicationStatus.Applied;

        return latestEvent.EventType switch
        {
            EventType.Applied => ApplicationStatus.Applied,
            EventType.Screening => ApplicationStatus.Screening,
            EventType.Interview => ApplicationStatus.Interview,
            EventType.Offer => ApplicationStatus.Offer,
            EventType.Rejected => ApplicationStatus.Rejected,
            EventType.Withdrawn => ApplicationStatus.Withdrawn,
            _ => ApplicationStatus.Applied
        };
    }

    public int ComputeTotalPoints()
    {
        return TimelineEvents.Sum(e => e.Points);
    }
}
