using Domain.Enums;

namespace Domain.Entities;

public class ActivityEvent
{
    public Guid Id { get; set; }
    public Guid PartyId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ActivityEventType EventType { get; set; }
    public int PointsDelta { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? CompanyName { get; set; }
    public string? RoleTitle { get; set; }
    public string? MilestoneLabel { get; set; }

    // Navigation
    public HuntingParty Party { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
}
