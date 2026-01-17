using Application.Interfaces.Data;
using Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Data;

/// <summary>
/// Unit of Work implementation coordinating all repositories.
/// Manages database transactions and provides a single commit point.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    // Lazy-loaded repositories
    private IApplicationRepository? _applications;
    private ITimelineEventRepository? _timelineEvents;
    private IActivityEventRepository? _activityEvents;
    private IHuntingPartyRepository? _huntingParties;
    private IHuntingPartyMembershipRepository? _huntingPartyMemberships;
    private IUserRepository? _users;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Gets the Application repository (lazy-loaded).
    /// </summary>
    public IApplicationRepository Applications =>
        _applications ??= new ApplicationRepository(_context);

    /// <summary>
    /// Gets the TimelineEvent repository (lazy-loaded).
    /// </summary>
    public ITimelineEventRepository TimelineEvents =>
        _timelineEvents ??= new TimelineEventRepository(_context);

    /// <summary>
    /// Gets the ActivityEvent repository (lazy-loaded).
    /// </summary>
    public IActivityEventRepository ActivityEvents =>
        _activityEvents ??= new ActivityEventRepository(_context);

    /// <summary>
    /// Gets the HuntingParty repository (lazy-loaded).
    /// </summary>
    public IHuntingPartyRepository HuntingParties =>
        _huntingParties ??= new HuntingPartyRepository(_context);

    /// <summary>
    /// Gets the HuntingPartyMembership repository (lazy-loaded).
    /// </summary>
    public IHuntingPartyMembershipRepository HuntingPartyMemberships =>
        _huntingPartyMemberships ??= new HuntingPartyMembershipRepository(_context);

    /// <summary>
    /// Gets the ApplicationUser repository (lazy-loaded).
    /// </summary>
    public IUserRepository Users =>
        _users ??= new UserRepository(_context);

    /// <summary>
    /// Saves all changes made in this unit of work.
    /// This is the single commit point for all repositories.
    /// </summary>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Begins an explicit database transaction.
    /// </summary>
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            throw new InvalidOperationException("A transaction is already in progress.");
        }

        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    /// <summary>
    /// Commits the current transaction.
    /// </summary>
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No transaction is in progress.");
        }

        try
        {
            await _transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await _transaction.RollbackAsync(cancellationToken);
            throw;
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    /// <summary>
    /// Rolls back the current transaction.
    /// </summary>
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No transaction is in progress.");
        }

        await _transaction.RollbackAsync(cancellationToken);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    /// <summary>
    /// Disposes the Unit of Work and the underlying DbContext.
    /// </summary>
    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
