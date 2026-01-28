using Application.Interfaces.Data;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for InterviewQuestion entity.
/// </summary>
public class InterviewQuestionRepository : Repository<InterviewQuestion>, IInterviewQuestionRepository
{
    public InterviewQuestionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<(IEnumerable<InterviewQuestion> Questions, int TotalCount)> GetByUserIdAsync(
        string userId,
        QuestionCategory? category = null,
        string? searchTerm = null,
        Guid? applicationId = null,
        int page = 1,
        int pageSize = 20,
        string sortBy = "createdDate",
        bool sortDescending = true,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(q => q.UserId == userId);

        // Apply filters
        if (category.HasValue)
        {
            query = query.Where(q => q.Category == category.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerSearchTerm = searchTerm.ToLower();
            query = query.Where(q =>
                q.QuestionText.ToLower().Contains(lowerSearchTerm) ||
                (q.AnswerText != null && q.AnswerText.ToLower().Contains(lowerSearchTerm)) ||
                (q.Notes != null && q.Notes.ToLower().Contains(lowerSearchTerm)));
        }

        if (applicationId.HasValue)
        {
            query = query.Where(q => q.ApplicationId == applicationId.Value);
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting
        query = sortBy.ToLower() switch
        {
            "timesasked" => sortDescending
                ? query.OrderByDescending(q => q.TimesAsked)
                : query.OrderBy(q => q.TimesAsked),
            "lastaskeddate" => sortDescending
                ? query.OrderByDescending(q => q.LastAskedDate)
                : query.OrderBy(q => q.LastAskedDate),
            "questiontext" => sortDescending
                ? query.OrderByDescending(q => q.QuestionText)
                : query.OrderBy(q => q.QuestionText),
            "category" => sortDescending
                ? query.OrderByDescending(q => q.Category)
                : query.OrderBy(q => q.Category),
            _ => sortDescending
                ? query.OrderByDescending(q => q.CreatedDate)
                : query.OrderBy(q => q.CreatedDate)
        };

        // Apply pagination
        var questions = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(q => q.Application)
            .ToListAsync(cancellationToken);

        return (questions, totalCount);
    }

    public async Task<IEnumerable<InterviewQuestion>> GetFrequentQuestionsAsync(
        string userId,
        int limit = 10,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(q => q.UserId == userId)
            .OrderByDescending(q => q.TimesAsked)
            .ThenByDescending(q => q.LastAskedDate)
            .Take(limit)
            .Include(q => q.Application)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<InterviewQuestion>> GetByCategoryAsync(
        string userId,
        QuestionCategory category,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(q => q.UserId == userId && q.Category == category)
            .OrderByDescending(q => q.CreatedDate)
            .Include(q => q.Application)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .CountAsync(q => q.UserId == userId, cancellationToken);
    }

    public async Task<InterviewQuestion?> GetByIdWithApplicationAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(q => q.Application)
            .FirstOrDefaultAsync(q => q.Id == id, cancellationToken);
    }
}
