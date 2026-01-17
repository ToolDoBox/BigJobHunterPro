namespace Application.Interfaces.Data;

/// <summary>
/// Generic repository interface for basic CRUD operations.
/// All entity repositories should inherit from this interface.
/// </summary>
/// <typeparam name="TEntity">The entity type this repository manages</typeparam>
public interface IRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Gets an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier (Guid)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The entity if found, null otherwise</returns>
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities of this type.
    /// WARNING: Use with caution on large tables. Consider pagination.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>All entities</returns>
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new entity to the database.
    /// Note: Must call SaveChangesAsync on UnitOfWork to persist.
    /// </summary>
    /// <param name="entity">The entity to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The added entity</returns>
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing entity.
    /// Note: Must call SaveChangesAsync on UnitOfWork to persist.
    /// </summary>
    /// <param name="entity">The entity to update</param>
    void Update(TEntity entity);

    /// <summary>
    /// Deletes an entity from the database.
    /// Note: Must call SaveChangesAsync on UnitOfWork to persist.
    /// </summary>
    /// <param name="entity">The entity to delete</param>
    void Delete(TEntity entity);

    /// <summary>
    /// Checks if an entity with the given ID exists.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if exists, false otherwise</returns>
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
