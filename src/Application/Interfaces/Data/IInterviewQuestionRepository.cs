using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces.Data;

/// <summary>
/// Repository interface for InterviewQuestion entity.
/// </summary>
public interface IInterviewQuestionRepository : IRepository<InterviewQuestion>
{
    /// <summary>
    /// Gets all questions for a specific user with optional filtering.
    /// </summary>
    Task<(IEnumerable<InterviewQuestion> Questions, int TotalCount)> GetByUserIdAsync(
        string userId,
        QuestionCategory? category = null,
        string? searchTerm = null,
        Guid? applicationId = null,
        int page = 1,
        int pageSize = 20,
        string sortBy = "createdDate",
        bool sortDescending = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the most frequently asked questions for a user.
    /// </summary>
    Task<IEnumerable<InterviewQuestion>> GetFrequentQuestionsAsync(
        string userId,
        int limit = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets questions by category for a user.
    /// </summary>
    Task<IEnumerable<InterviewQuestion>> GetByCategoryAsync(
        string userId,
        QuestionCategory category,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts questions for a user.
    /// </summary>
    Task<int> CountByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a question by ID including related application data.
    /// </summary>
    Task<InterviewQuestion?> GetByIdWithApplicationAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
