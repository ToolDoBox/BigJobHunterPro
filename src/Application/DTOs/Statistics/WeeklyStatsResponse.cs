namespace BigJobHunterPro.Application.DTOs.Statistics;

/// <summary>
/// Weekly statistics response comparing this week vs last week
/// </summary>
public class WeeklyStatsResponse
{
    /// <summary>
    /// Number of applications submitted in the current rolling 7-day window
    /// </summary>
    public int ApplicationsThisWeek { get; set; }

    /// <summary>
    /// Number of applications submitted in the previous rolling 7-day window
    /// </summary>
    public int ApplicationsLastWeek { get; set; }

    /// <summary>
    /// Percentage change from last week to this week (can be negative)
    /// </summary>
    public decimal PercentageChange { get; set; }

    /// <summary>
    /// Total points earned in the current rolling 7-day window
    /// </summary>
    public int PointsThisWeek { get; set; }

    /// <summary>
    /// Total points earned in the previous rolling 7-day window
    /// </summary>
    public int PointsLastWeek { get; set; }
}
