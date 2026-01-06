using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Application.Interfaces;

namespace WebAPI.Hubs;

[Authorize]
public class LeaderboardHub : Hub
{
    private readonly IHuntingPartyService _huntingPartyService;
    private readonly ICurrentUserService _currentUserService;

    public LeaderboardHub(
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
                // Add user to their party's SignalR group
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

    /// <summary>
    /// Allow client to manually join a party group (used when first joining a party)
    /// </summary>
    public async Task JoinPartyGroup(Guid partyId)
    {
        var userId = _currentUserService.GetUserId();
        if (userId == null) return;

        // Verify user is a member of this party
        var userPartyId = await _huntingPartyService.GetUserPartyIdAsync(userId);
        if (userPartyId == partyId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, partyId.ToString());
        }
    }

    /// <summary>
    /// Allow client to leave a party group (used when leaving a party)
    /// </summary>
    public async Task LeavePartyGroup(Guid partyId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, partyId.ToString());
    }
}
