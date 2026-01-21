using Domain.Entities;

namespace Application.Interfaces.Data;

/// <summary>
/// Repository interface for Contact entity.
/// </summary>
public interface IContactRepository : IRepository<Contact>
{
    /// <summary>
    /// Gets all contacts for a specific application.
    /// </summary>
    Task<IEnumerable<Contact>> GetByApplicationIdAsync(
        Guid applicationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts contacts for an application.
    /// </summary>
    Task<int> CountByApplicationIdAsync(
        Guid applicationId,
        CancellationToken cancellationToken = default);
}
