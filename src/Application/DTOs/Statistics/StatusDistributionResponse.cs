namespace BigJobHunterPro.Application.DTOs.Statistics;

/// <summary>
/// Represents the count of applications at a specific status/stage
/// </summary>
public class StatusDistribution
{
    /// <summary>
    /// The status/stage name (e.g., "Applied", "Screening", "Interview", "Rejected", "Offer")
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Number of applications at this status
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Percentage of total applications (0-100)
    /// </summary>
    public decimal Percentage { get; set; }
}

/// <summary>
/// Response containing the distribution of applications across different statuses
/// </summary>
public class StatusDistributionResponse
{
    /// <summary>
    /// List of status distributions
    /// </summary>
    public List<StatusDistribution> Statuses { get; set; } = new();

    /// <summary>
    /// Total number of applications
    /// </summary>
    public int TotalApplications { get; set; }
}
