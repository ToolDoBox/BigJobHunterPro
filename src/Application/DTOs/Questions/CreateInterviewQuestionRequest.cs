using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.DTOs.Questions;

/// <summary>
/// Request DTO for creating a new interview question.
/// </summary>
public class CreateInterviewQuestionRequest
{
    [Required]
    [MaxLength(2000)]
    public string QuestionText { get; set; } = string.Empty;

    [MaxLength(5000)]
    public string? AnswerText { get; set; }

    [MaxLength(2000)]
    public string? Notes { get; set; }

    [Required]
    public QuestionCategory Category { get; set; }

    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Optional: Link this question to a specific job application for context.
    /// </summary>
    public Guid? ApplicationId { get; set; }
}
