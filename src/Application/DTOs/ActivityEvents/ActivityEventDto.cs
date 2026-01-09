namespace Application.DTOs.ActivityEvents;

public class ActivityEventDto
{
    public Guid Id { get; set; }
    public Guid PartyId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserDisplayName { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public int PointsDelta { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? CompanyName { get; set; }
    public string? RoleTitle { get; set; }
    public string? MilestoneLabel { get; set; }
}
