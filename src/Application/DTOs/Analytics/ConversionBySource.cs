namespace BigJobHunterPro.Application.DTOs.Analytics;

/// <summary>
/// Represents conversion rate statistics for an application source/platform
/// </summary>
public class ConversionBySource
{
    /// <summary>
    /// The source/platform name (e.g., "LinkedIn", "Indeed", "Company Website")
    /// </summary>
    public string SourceName { get; set; } = string.Empty;

    /// <summary>
    /// Total number of applications from this source
    /// </summary>
    public int TotalApplications { get; set; }

    /// <summary>
    /// Number of applications that reached interview stage or beyond
    /// </summary>
    public int InterviewCount { get; set; }

    /// <summary>
    /// Conversion rate (InterviewCount / TotalApplications * 100)
    /// </summary>
    public decimal ConversionRate { get; set; }
}
