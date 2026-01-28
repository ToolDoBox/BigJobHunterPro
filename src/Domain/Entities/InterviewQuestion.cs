using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Represents an interview question that the user has encountered or wants to practice.
/// Part of the Question Range feature for building a personal Q&A knowledge base.
/// </summary>
public class InterviewQuestion
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    // Core fields
    public string QuestionText { get; set; } = string.Empty;
    public string? AnswerText { get; set; }
    public string? Notes { get; set; }

    // Categorization
    public QuestionCategory Category { get; set; }
    public List<string> Tags { get; set; } = new();

    // Tracking
    public int TimesAsked { get; set; } = 1;
    public DateTime? LastAskedDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }

    // Optional link to specific application (for context)
    public Guid? ApplicationId { get; set; }
    public Application? Application { get; set; }
}
