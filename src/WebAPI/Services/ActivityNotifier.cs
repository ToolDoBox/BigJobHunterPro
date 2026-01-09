using Application.DTOs.ActivityEvents;
using Application.Interfaces;
using Microsoft.AspNetCore.SignalR;
using WebAPI.Hubs;

namespace WebAPI.Services;

public class ActivityNotifier : IActivityNotifier
{
    private readonly IHubContext<ActivityHub> _hubContext;

    public ActivityNotifier(IHubContext<ActivityHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyActivityAsync(Guid partyId, ActivityEventDto activityEvent)
    {
        await _hubContext.Clients.Group(partyId.ToString())
            .SendAsync("ActivityEventCreated", activityEvent);
    }
}
