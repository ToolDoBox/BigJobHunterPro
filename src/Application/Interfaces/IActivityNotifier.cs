using Application.DTOs.ActivityEvents;

namespace Application.Interfaces;

public interface IActivityNotifier
{
    Task NotifyActivityAsync(Guid partyId, ActivityEventDto activityEvent);
}
