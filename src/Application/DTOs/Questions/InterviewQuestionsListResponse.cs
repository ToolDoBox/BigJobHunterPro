namespace Application.DTOs.Questions;

/// <summary>
/// Response DTO for listing interview questions with pagination support.
/// </summary>
public class InterviewQuestionsListResponse
{
    public List<InterviewQuestionDto> Questions { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
