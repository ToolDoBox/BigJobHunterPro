namespace BigJobHunterPro.Application.DTOs.Statistics;

/// <summary>
/// Represents average time (in days) from application submission to a specific milestone
/// </summary>
public class AverageTimeToMilestone
{
    /// <summary>
    /// The milestone name (e.g., "Screening", "Interview", "Rejection", "Offer")
    /// </summary>
    public string Milestone { get; set; } = string.Empty;

    /// <summary>
    /// Average number of days to reach this milestone from application submission
    /// Null if no data available
    /// </summary>
    public decimal? AverageDays { get; set; }

    /// <summary>
    /// Number of applications that reached this milestone (sample size)
    /// </summary>
    public int SampleSize { get; set; }
}

/// <summary>
/// Response containing average time to reach various milestones
/// </summary>
public class AverageTimeResponse
{
    /// <summary>
    /// List of average times to different milestones
    /// </summary>
    public List<AverageTimeToMilestone> Milestones { get; set; } = new();
}
