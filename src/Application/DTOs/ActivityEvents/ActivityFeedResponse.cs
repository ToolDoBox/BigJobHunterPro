namespace Application.DTOs.ActivityEvents;

public class ActivityFeedResponse
{
    public Guid PartyId { get; set; }
    public List<ActivityEventDto> Events { get; set; } = new();
    public bool HasMore { get; set; }
}
