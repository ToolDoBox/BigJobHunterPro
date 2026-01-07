using Application.Interfaces;
using Application.Scoring;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class PointsService : IPointsService
{
    private readonly ApplicationDbContext _context;
    private readonly ILeaderboardNotifier? _leaderboardNotifier;

    public PointsService(ApplicationDbContext context, ILeaderboardNotifier? leaderboardNotifier = null)
    {
        _context = context;
        _leaderboardNotifier = leaderboardNotifier;
    }

    public int CalculatePoints(ApplicationStatus status)
    {
        return PointsRules.GetPoints(status);
    }

    public int CalculatePoints(EventType eventType, int? interviewRound = null)
    {
        return PointsRules.GetPoints(eventType, interviewRound);
    }

    public async Task<int> UpdateUserTotalPointsAsync(string userId, int pointsToAdd)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) throw new UnauthorizedAccessException("User not found");

        if (user.TotalPoints == 0 && user.Points > 0)
        {
            user.TotalPoints = user.Points;
        }

        user.Points = Math.Max(0, user.Points + pointsToAdd);
        user.TotalPoints = Math.Max(0, user.TotalPoints + pointsToAdd);

        // Notify leaderboard if user is in a party
        if (_leaderboardNotifier != null)
        {
            var membership = await _context.HuntingPartyMemberships
                .FirstOrDefaultAsync(m => m.UserId == userId && m.IsActive);

            if (membership != null)
            {
                await _leaderboardNotifier.NotifyLeaderboardUpdateAsync(membership.HuntingPartyId);
            }
        }

        return user.TotalPoints;
    }
}
