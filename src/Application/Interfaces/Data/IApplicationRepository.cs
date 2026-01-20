using ApplicationEntity = Domain.Entities.Application;
using Domain.Enums;

namespace Application.Interfaces.Data;

/// <summary>
/// Repository interface for Application entity with specialized queries.
/// Extends the generic repository with application-specific operations.
/// </summary>
public interface IApplicationRepository : IRepository<ApplicationEntity>
{
    /// <summary>
    /// Gets all applications for a specific user.
    /// </summary>
    Task<IEnumerable<ApplicationEntity>> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets applications with their timeline events included.
    /// </summary>
    Task<IEnumerable<ApplicationEntity>> GetWithTimelineEventsAsync(
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a single application by ID with timeline events included.
    /// </summary>
    Task<ApplicationEntity?> GetByIdWithTimelineAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts the total number of applications for a user.
    /// </summary>
    Task<int> CountByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets applications filtered by status.
    /// </summary>
    Task<IEnumerable<ApplicationEntity>> GetByStatusAsync(
        string userId,
        ApplicationStatus status,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets recent applications for a user (ordered by created date).
    /// </summary>
    Task<IEnumerable<ApplicationEntity>> GetRecentAsync(
        string userId,
        int count,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a page of applications for a user (ordered by created date).
    /// </summary>
    Task<IReadOnlyList<ApplicationEntity>> GetPageByUserIdAsync(
        string userId,
        int skip,
        int take,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a page of applications for a user with optional search and status filters.
    /// </summary>
    Task<IReadOnlyList<ApplicationEntity>> GetPageByUserIdWithFiltersAsync(
        string userId,
        int skip,
        int take,
        string? search = null,
        ApplicationStatus? status = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts applications for a user with optional search and status filters.
    /// </summary>
    Task<int> CountByUserIdWithFiltersAsync(
        string userId,
        string? search = null,
        ApplicationStatus? status = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts applications for a user within a date range.
    /// </summary>
    Task<int> CountByUserIdInRangeAsync(
        string userId,
        DateTime startInclusive,
        DateTime endExclusive,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets application counts grouped by user ID.
    /// </summary>
    Task<Dictionary<string, int>> GetCountsByUserIdsAsync(
        IEnumerable<string> userIds,
        CancellationToken cancellationToken = default);
}
