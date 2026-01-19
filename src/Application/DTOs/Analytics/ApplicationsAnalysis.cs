namespace BigJobHunterPro.Application.DTOs.Analytics;

/// <summary>
/// Represents comprehensive analysis of all user applications
/// Used for job description insights including role keywords and skill frequencies
/// </summary>
public class ApplicationsAnalysis
{
    /// <summary>
    /// Top keywords extracted from role titles across all applications
    /// </summary>
    public List<KeywordFrequency> RoleKeywords { get; set; } = new();

    /// <summary>
    /// Most frequently appearing skills across all applications
    /// </summary>
    public List<SkillFrequency> TopSkills { get; set; } = new();

    /// <summary>
    /// Total number of applications analyzed
    /// </summary>
    public int TotalApplicationsAnalyzed { get; set; }
}

/// <summary>
/// Represents a skill and its frequency across applications
/// </summary>
public class SkillFrequency
{
    /// <summary>
    /// The skill name
    /// </summary>
    public string Skill { get; set; } = string.Empty;

    /// <summary>
    /// Number of applications containing this skill
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Percentage of applications containing this skill
    /// </summary>
    public decimal Percentage { get; set; }
}
