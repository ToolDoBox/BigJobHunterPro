namespace BigJobHunterPro.Application.DTOs.Analytics;

/// <summary>
/// Represents a keyword and its frequency in successful applications
/// </summary>
public class KeywordFrequency
{
    /// <summary>
    /// The keyword extracted from successful applications
    /// </summary>
    public string Keyword { get; set; } = string.Empty;

    /// <summary>
    /// Number of times this keyword appeared in successful applications
    /// </summary>
    public int Frequency { get; set; }

    /// <summary>
    /// Percentage of successful applications containing this keyword
    /// </summary>
    public decimal Percentage { get; set; }
}
