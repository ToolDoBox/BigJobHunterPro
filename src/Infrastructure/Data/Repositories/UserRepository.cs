using Application.Interfaces.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for ApplicationUser entity.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<ApplicationUser> _dbSet;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = context.Set<ApplicationUser>();
    }

    public async Task<ApplicationUser?> GetByIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { userId }, cancellationToken);
    }

    public async Task<string?> GetDisplayNameAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(user => user.Id == userId)
            .Select(user => user.DisplayName)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
