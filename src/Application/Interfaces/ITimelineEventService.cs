using Application.DTOs.TimelineEvents;

namespace Application.Interfaces;

public interface ITimelineEventService
{
    Task<TimelineEventDto> CreateTimelineEventAsync(Guid applicationId, CreateTimelineEventRequest request);
    Task<TimelineEventsListResponse> GetTimelineEventsAsync(Guid applicationId);
    Task<TimelineEventDto?> UpdateTimelineEventAsync(Guid applicationId, Guid eventId, UpdateTimelineEventRequest request);
    Task<bool> DeleteTimelineEventAsync(Guid applicationId, Guid eventId);
}
