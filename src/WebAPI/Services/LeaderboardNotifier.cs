using Application.DTOs.HuntingParty;
using Application.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebAPI.Hubs;

namespace WebAPI.Services;

public class LeaderboardNotifier : ILeaderboardNotifier
{
    private readonly IHubContext<LeaderboardHub> _hubContext;
    private readonly ApplicationDbContext _context;

    public LeaderboardNotifier(
        IHubContext<LeaderboardHub> hubContext,
        ApplicationDbContext context)
    {
        _hubContext = hubContext;
        _context = context;
    }

    public async Task NotifyLeaderboardUpdateAsync(Guid partyId)
    {
        // Get leaderboard data
        var members = await _context.HuntingPartyMemberships
            .Where(m => m.HuntingPartyId == partyId && m.IsActive)
            .Include(m => m.User)
            .ThenInclude(u => u.Applications)
            .Select(m => new
            {
                m.UserId,
                m.User.DisplayName,
                m.User.TotalPoints,
                ApplicationCount = m.User.Applications.Count
            })
            .OrderByDescending(m => m.TotalPoints)
            .ToListAsync();

        var leaderboard = members.Select((m, index) => new LeaderboardEntryDto
        {
            Rank = index + 1,
            UserId = m.UserId,
            DisplayName = m.DisplayName,
            TotalPoints = m.TotalPoints,
            ApplicationCount = m.ApplicationCount,
            IsCurrentUser = false // Will be set client-side
        }).ToList();

        // Broadcast to all clients in the party group
        await _hubContext.Clients.Group(partyId.ToString())
            .SendAsync("LeaderboardUpdated", leaderboard);

        // Send personalized rivalry updates to each member
        foreach (var member in members)
        {
            var currentRank = leaderboard.FindIndex(e => e.UserId == member.UserId) + 1;
            var userAhead = currentRank > 1 ? leaderboard[currentRank - 2] : null;
            var userBehind = currentRank < leaderboard.Count ? leaderboard[currentRank] : null;
            var currentEntry = leaderboard[currentRank - 1];

            var rivalry = new RivalryDto
            {
                CurrentRank = currentRank,
                TotalMembers = leaderboard.Count,
                UserAhead = userAhead != null ? new RivalInfoDto
                {
                    DisplayName = userAhead.DisplayName,
                    Points = userAhead.TotalPoints,
                    Gap = userAhead.TotalPoints - currentEntry.TotalPoints
                } : null,
                UserBehind = userBehind != null ? new RivalInfoDto
                {
                    DisplayName = userBehind.DisplayName,
                    Points = userBehind.TotalPoints,
                    Gap = currentEntry.TotalPoints - userBehind.TotalPoints
                } : null
            };

            // Send personalized rivalry to this specific user
            await _hubContext.Clients.Group(partyId.ToString())
                .SendAsync("RivalryUpdated", new { UserId = member.UserId, Rivalry = rivalry });
        }
    }
}
