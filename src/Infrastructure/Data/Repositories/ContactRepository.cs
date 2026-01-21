using Application.Interfaces.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for Contact entity.
/// </summary>
public class ContactRepository : Repository<Contact>, IContactRepository
{
    public ContactRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Contact>> GetByApplicationIdAsync(
        Guid applicationId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.ApplicationId == applicationId)
            .OrderBy(c => c.CreatedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountByApplicationIdAsync(
        Guid applicationId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .CountAsync(c => c.ApplicationId == applicationId, cancellationToken);
    }
}
