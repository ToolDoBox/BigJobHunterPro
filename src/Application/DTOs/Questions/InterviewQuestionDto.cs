namespace Application.DTOs.Questions;

/// <summary>
/// DTO for returning interview question data to clients.
/// </summary>
public class InterviewQuestionDto
{
    public Guid Id { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string? AnswerText { get; set; }
    public string? Notes { get; set; }
    public string Category { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public int TimesAsked { get; set; }
    public DateTime? LastAskedDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public Guid? ApplicationId { get; set; }
    public string? ApplicationCompany { get; set; }
    public string? ApplicationRole { get; set; }
}
