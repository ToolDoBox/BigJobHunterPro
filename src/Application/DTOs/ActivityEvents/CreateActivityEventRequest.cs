using Domain.Enums;

namespace Application.DTOs.ActivityEvents;

public class CreateActivityEventRequest
{
    public Guid PartyId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ActivityEventType EventType { get; set; }
    public int PointsDelta { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? CompanyName { get; set; }
    public string? RoleTitle { get; set; }
    public string? MilestoneLabel { get; set; }
}
