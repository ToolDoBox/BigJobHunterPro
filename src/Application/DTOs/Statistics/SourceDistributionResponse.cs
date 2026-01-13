namespace BigJobHunterPro.Application.DTOs.Statistics;

/// <summary>
/// Represents the count of applications from a specific source
/// </summary>
public class SourceDistribution
{
    /// <summary>
    /// The source name (e.g., "LinkedIn", "Indeed", "Company Website")
    /// </summary>
    public string SourceName { get; set; } = string.Empty;

    /// <summary>
    /// Number of applications from this source
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Percentage of total applications (0-100)
    /// </summary>
    public decimal Percentage { get; set; }
}

/// <summary>
/// Response containing the distribution of applications across different sources
/// </summary>
public class SourceDistributionResponse
{
    /// <summary>
    /// List of source distributions
    /// </summary>
    public List<SourceDistribution> Sources { get; set; } = new();

    /// <summary>
    /// Total number of applications
    /// </summary>
    public int TotalApplications { get; set; }
}
