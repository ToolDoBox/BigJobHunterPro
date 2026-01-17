using Application.Interfaces;
using Application.Interfaces.Data;
using Domain.Entities;

namespace Infrastructure.Services;

public class StreakService : IStreakService
{
    private readonly IUnitOfWork _unitOfWork;

    public StreakService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<StreakUpdateResult> UpdateStreakAsync(string userId, DateTime activityTimestamp)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException($"User not found: {userId}");
        }

        var result = new StreakUpdateResult
        {
            CurrentStreak = user.CurrentStreak,
            LongestStreak = user.LongestStreak,
            StreakIncremented = false,
            StreakBroken = false
        };

        // First activity ever
        if (user.LastActivityDate == null)
        {
            user.CurrentStreak = 1;
            user.LongestStreak = 1;
            user.LastActivityDate = activityTimestamp;
            user.StreakLastUpdated = activityTimestamp;

            result.CurrentStreak = 1;
            result.LongestStreak = 1;
            result.StreakIncremented = true;

            return result;
        }

        // Calculate time since last activity
        var timeSinceLastActivity = activityTimestamp - user.LastActivityDate.Value;

        if (timeSinceLastActivity.TotalHours < 24)
        {
            // Same day - no streak change, just update last activity time
            user.LastActivityDate = activityTimestamp;
            result.CurrentStreak = user.CurrentStreak;
            result.LongestStreak = user.LongestStreak;
        }
        else if (timeSinceLastActivity.TotalHours < 48)
        {
            // Next day - increment streak
            user.CurrentStreak++;
            user.LastActivityDate = activityTimestamp;
            user.StreakLastUpdated = activityTimestamp;

            // Update longest streak if needed
            if (user.CurrentStreak > user.LongestStreak)
            {
                user.LongestStreak = user.CurrentStreak;
            }

            result.CurrentStreak = user.CurrentStreak;
            result.LongestStreak = user.LongestStreak;
            result.StreakIncremented = true;
        }
        else
        {
            // Streak broken - reset to 1
            user.CurrentStreak = 1;
            user.LastActivityDate = activityTimestamp;
            user.StreakLastUpdated = activityTimestamp;

            result.CurrentStreak = 1;
            result.LongestStreak = user.LongestStreak; // Preserve longest streak
            result.StreakBroken = true;
        }

        return result;
    }

    public bool IsStreakActive(DateTime? lastActivityDate, DateTime currentTimestamp)
    {
        if (lastActivityDate == null)
        {
            return false;
        }

        var timeSinceLastActivity = currentTimestamp - lastActivityDate.Value;

        // Streak is active if last activity was within 48 hours (24-hour grace period)
        return timeSinceLastActivity.TotalHours < 48;
    }
}
