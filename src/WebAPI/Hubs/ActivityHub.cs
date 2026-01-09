using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace WebAPI.Hubs;

[Authorize]
public class ActivityHub : Hub
{
    private readonly IHuntingPartyService _huntingPartyService;
    private readonly ICurrentUserService _currentUserService;

    public ActivityHub(
        IHuntingPartyService huntingPartyService,
        ICurrentUserService currentUserService)
    {
        _huntingPartyService = huntingPartyService;
        _currentUserService = currentUserService;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = _currentUserService.GetUserId();
        if (userId != null)
        {
            var partyId = await _huntingPartyService.GetUserPartyIdAsync(userId);
            if (partyId.HasValue)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, partyId.Value.ToString());
            }
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = _currentUserService.GetUserId();
        if (userId != null)
        {
            var partyId = await _huntingPartyService.GetUserPartyIdAsync(userId);
            if (partyId.HasValue)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, partyId.Value.ToString());
            }
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinPartyGroup(Guid partyId)
    {
        var userId = _currentUserService.GetUserId();
        if (userId == null) return;

        var userPartyId = await _huntingPartyService.GetUserPartyIdAsync(userId);
        if (userPartyId == partyId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, partyId.ToString());
        }
    }

    public async Task LeavePartyGroup(Guid partyId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, partyId.ToString());
    }
}
