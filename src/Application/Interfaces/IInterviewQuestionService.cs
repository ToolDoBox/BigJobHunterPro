using Application.DTOs.Questions;
using Domain.Enums;

namespace Application.Interfaces;

/// <summary>
/// Service interface for interview question operations.
/// </summary>
public interface IInterviewQuestionService
{
    /// <summary>
    /// Creates a new interview question.
    /// </summary>
    Task<InterviewQuestionDto> CreateAsync(CreateInterviewQuestionRequest request);

    /// <summary>
    /// Gets all questions for the current user with optional filtering and pagination.
    /// </summary>
    Task<InterviewQuestionsListResponse> GetAllAsync(
        QuestionCategory? category = null,
        string? searchTerm = null,
        Guid? applicationId = null,
        int page = 1,
        int pageSize = 20,
        string sortBy = "createdDate",
        bool sortDescending = true);

    /// <summary>
    /// Gets a specific question by ID.
    /// </summary>
    Task<InterviewQuestionDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// Updates an existing question.
    /// </summary>
    Task<InterviewQuestionDto?> UpdateAsync(Guid id, UpdateInterviewQuestionRequest request);

    /// <summary>
    /// Deletes a question.
    /// </summary>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// Increments the TimesAsked counter for a question.
    /// </summary>
    Task<InterviewQuestionDto?> IncrementTimesAskedAsync(Guid id);

    /// <summary>
    /// Gets the most frequently asked questions for the current user.
    /// </summary>
    Task<IEnumerable<InterviewQuestionDto>> GetFrequentAsync(int limit = 10);
}
