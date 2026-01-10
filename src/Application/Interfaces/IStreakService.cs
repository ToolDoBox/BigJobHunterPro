namespace Application.Interfaces;

public interface IStreakService
{
    Task<StreakUpdateResult> UpdateStreakAsync(string userId, DateTime activityTimestamp);
    bool IsStreakActive(DateTime? lastActivityDate, DateTime currentTimestamp);
}

public class StreakUpdateResult
{
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public bool StreakIncremented { get; set; }
    public bool StreakBroken { get; set; }
}
