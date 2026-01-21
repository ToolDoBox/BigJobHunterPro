namespace Application.Interfaces.Data;

/// <summary>
/// Unit of Work pattern interface for coordinating multiple repositories.
/// Provides a single transaction boundary for all database operations.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Repository for Application entities.
    /// </summary>
    IApplicationRepository Applications { get; }

    /// <summary>
    /// Repository for TimelineEvent entities.
    /// </summary>
    ITimelineEventRepository TimelineEvents { get; }

    /// <summary>
    /// Repository for Contact entities.
    /// </summary>
    IContactRepository Contacts { get; }

    /// <summary>
    /// Repository for ActivityEvent entities.
    /// </summary>
    IActivityEventRepository ActivityEvents { get; }

    /// <summary>
    /// Repository for HuntingParty entities.
    /// </summary>
    IHuntingPartyRepository HuntingParties { get; }

    /// <summary>
    /// Repository for HuntingPartyMembership entities.
    /// </summary>
    IHuntingPartyMembershipRepository HuntingPartyMemberships { get; }

    /// <summary>
    /// Repository for ApplicationUser entities.
    /// </summary>
    IUserRepository Users { get; }

    /// <summary>
    /// Saves all changes made in this unit of work to the database.
    /// This is the single commit point for all repositories.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The number of state entries written to the database</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins an explicit database transaction.
    /// Use this when you need to manually control transaction boundaries.
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits the current transaction.
    /// </summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the current transaction.
    /// </summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
