using Application.Interfaces;
using Application.Interfaces.Data;
using Application.Scoring;
using Domain.Enums;

namespace Infrastructure.Services;

public class PointsService : IPointsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILeaderboardNotifier? _leaderboardNotifier;
    private readonly IStreakService _streakService;

    public PointsService(
        IUnitOfWork unitOfWork,
        IStreakService streakService,
        ILeaderboardNotifier? leaderboardNotifier = null)
    {
        _unitOfWork = unitOfWork;
        _streakService = streakService;
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
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null) throw new UnauthorizedAccessException("User not found");

        if (user.TotalPoints == 0 && user.Points > 0)
        {
            user.TotalPoints = user.Points;
        }

        user.Points = Math.Max(0, user.Points + pointsToAdd);
        user.TotalPoints = Math.Max(0, user.TotalPoints + pointsToAdd);

        // Update streak whenever points are earned
        if (pointsToAdd > 0)
        {
            await _streakService.UpdateStreakAsync(userId, DateTime.UtcNow);
        }

        // Notify leaderboard if user is in a party
        if (_leaderboardNotifier != null)
        {
            var membership = (await _unitOfWork.HuntingPartyMemberships
                .GetActiveByUserIdAsync(userId))
                .FirstOrDefault();

            if (membership != null)
            {
                await _leaderboardNotifier.NotifyLeaderboardUpdateAsync(membership.HuntingPartyId);
            }
        }

        return user.TotalPoints;
    }
}
