using Application.DTOs.ActivityEvents;

namespace Application.Interfaces;

public interface IActivityEventService
{
    Task<ActivityEventDto?> CreateEventAsync(CreateActivityEventRequest request);
    Task<ActivityFeedResponse?> GetPartyActivityAsync(Guid partyId, int limit);
}
