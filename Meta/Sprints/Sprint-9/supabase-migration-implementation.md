# Supabase Migration - 3-Phase Implementation Plan

**Branch:** `feature/supabase-postgres-migration`
**Total Duration:** 5-7 days
**Approach:** Incremental migration with validation at each phase

---

## Table of Contents

- [Overview](#overview)
- [Pre-Implementation Checklist](#pre-implementation-checklist)
- [Phase 1: Database Layer Refactoring](#phase-1-database-layer-refactoring)
- [Phase 2: PostgreSQL Setup & Local Migration](#phase-2-postgresql-setup--local-migration)
- [Phase 3: Supabase Production Migration](#phase-3-supabase-production-migration)
- [Emergency Rollback Procedures](#emergency-rollback-procedures)

---

## Overview

### Three-Phase Strategy

```
Phase 1: Refactor Data Access Layer (2-3 days)
├─ Create repository interfaces
├─ Implement repositories & Unit of Work
├─ Refactor all services
└─ Test with EXISTING database (SQL Server/SQLite)
   ✅ Verify: Application works identically with new architecture

Phase 2: PostgreSQL Local Migration (1-2 days)
├─ Update to Npgsql package
├─ Set up Docker PostgreSQL locally
├─ Generate PostgreSQL migrations
└─ Test with LOCAL PostgreSQL
   ✅ Verify: Application works with PostgreSQL

Phase 3: Supabase Production Migration (1-2 days)
├─ Create Supabase project
├─ Migrate production data
├─ Deploy to production
└─ Monitor and optimize
   ✅ Verify: Production running on Supabase
```

### Why This Approach?

1. **Risk Isolation:** Architectural changes separated from database provider change
2. **Incremental Testing:** Each phase fully tested before proceeding
3. **Easy Rollback:** Can revert to previous phase if issues occur
4. **Confidence Building:** Validate architecture changes before data migration

---

## Pre-Implementation Checklist

### Before You Begin

- [ ] **Backup Azure SQL Database**
  ```bash
  # Via Azure Portal: Database → Export → Create .bacpac file
  # Download and store securely
  ```

- [ ] **Create feature branch** (already done)
  ```bash
  git checkout feature/supabase-postgres-migration
  git pull origin main  # Ensure up to date
  ```

- [ ] **Install required tools**
  - [ ] Docker Desktop
  - [ ] .NET 8 SDK
  - [ ] PostgreSQL client tools (optional, for debugging)
  - [ ] Git

- [ ] **Run existing tests to establish baseline**
  ```bash
  cd C:\Users\Inferno\Dev\BigJobHunterPro
  dotnet test
  # Note: Record results - this is your baseline
  ```

- [ ] **Document current database state**
  ```bash
  # Count records in production
  # Save this for validation later
  SELECT 'Users' as TableName, COUNT(*) as Count FROM AspNetUsers
  UNION ALL
  SELECT 'Applications', COUNT(*) FROM Applications
  UNION ALL
  SELECT 'TimelineEvents', COUNT(*) FROM TimelineEvents
  UNION ALL
  SELECT 'ActivityEvents', COUNT(*) FROM ActivityEvents
  UNION ALL
  SELECT 'HuntingParties', COUNT(*) FROM HuntingParties
  UNION ALL
  SELECT 'HuntingPartyMemberships', COUNT(*) FROM HuntingPartyMemberships
  ```

- [ ] **Notify team members** about ongoing migration work

---

## Phase 1: Database Layer Refactoring

**Duration:** 2-3 days
**Goal:** Implement repository pattern WITHOUT changing database provider
**Risk Level:** Low (no database changes)

### Why Phase 1 First?

By refactoring the data access layer while still using the existing database (SQL Server or SQLite), we can:
- Validate the new architecture works correctly
- Run all existing tests
- Fix any bugs in the repository implementation
- Ensure zero behavior changes before touching the database

---

### Step 1.1: Create Repository Interface Directory

```bash
# Create directory structure
mkdir -p src/Application/Interfaces/Data
```

**Verify:**
```bash
ls src/Application/Interfaces/Data
# Should exist (may be empty)
```

---

### Step 1.2: Create Generic Repository Interface

**Create file:** `src/Application/Interfaces/Data/IRepository.cs`

```csharp
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
```

**Test:** Build the project
```bash
cd src/Application
dotnet build
# Should compile successfully
```

---

### Step 1.3: Create Specialized Repository Interfaces

#### IApplicationRepository.cs

**Create file:** `src/Application/Interfaces/Data/IApplicationRepository.cs`

```csharp
using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces.Data;

/// <summary>
/// Repository interface for Application entity with specialized queries.
/// Extends the generic repository with application-specific operations.
/// </summary>
public interface IApplicationRepository : IRepository<Application>
{
    /// <summary>
    /// Gets all applications for a specific user.
    /// </summary>
    Task<IEnumerable<Application>> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets applications with their timeline events included.
    /// </summary>
    Task<IEnumerable<Application>> GetWithTimelineEventsAsync(
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a single application by ID with timeline events included.
    /// </summary>
    Task<Application?> GetByIdWithTimelineAsync(
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
    Task<IEnumerable<Application>> GetByStatusAsync(
        string userId,
        ApplicationStatus status,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets recent applications for a user (ordered by created date).
    /// </summary>
    Task<IEnumerable<Application>> GetRecentAsync(
        string userId,
        int count,
        CancellationToken cancellationToken = default);
}
```

#### ITimelineEventRepository.cs

**Create file:** `src/Application/Interfaces/Data/ITimelineEventRepository.cs`

```csharp
using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces.Data;

/// <summary>
/// Repository interface for TimelineEvent entity.
/// </summary>
public interface ITimelineEventRepository : IRepository<TimelineEvent>
{
    /// <summary>
    /// Gets all timeline events for a specific application.
    /// </summary>
    Task<IEnumerable<TimelineEvent>> GetByApplicationIdAsync(
        Guid applicationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets timeline events filtered by event type.
    /// </summary>
    Task<IEnumerable<TimelineEvent>> GetByEventTypeAsync(
        Guid applicationId,
        EventType eventType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the most recent timeline event for an application.
    /// </summary>
    Task<TimelineEvent?> GetMostRecentAsync(
        Guid applicationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts timeline events for an application.
    /// </summary>
    Task<int> CountByApplicationIdAsync(
        Guid applicationId,
        CancellationToken cancellationToken = default);
}
```

#### IActivityEventRepository.cs

**Create file:** `src/Application/Interfaces/Data/IActivityEventRepository.cs`

```csharp
using Domain.Entities;

namespace Application.Interfaces.Data;

/// <summary>
/// Repository interface for ActivityEvent entity.
/// </summary>
public interface IActivityEventRepository : IRepository<ActivityEvent>
{
    /// <summary>
    /// Gets recent activity events for a hunting party.
    /// </summary>
    Task<IEnumerable<ActivityEvent>> GetByPartyIdAsync(
        Guid partyId,
        int limit = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets activity events for a specific user.
    /// </summary>
    Task<IEnumerable<ActivityEvent>> GetByUserIdAsync(
        string userId,
        int limit = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets recent activity events across all parties a user belongs to.
    /// </summary>
    Task<IEnumerable<ActivityEvent>> GetRecentForUserPartiesAsync(
        string userId,
        int limit = 50,
        CancellationToken cancellationToken = default);
}
```

#### IHuntingPartyRepository.cs

**Create file:** `src/Application/Interfaces/Data/IHuntingPartyRepository.cs`

```csharp
using Domain.Entities;

namespace Application.Interfaces.Data;

/// <summary>
/// Repository interface for HuntingParty entity.
/// </summary>
public interface IHuntingPartyRepository : IRepository<HuntingParty>
{
    /// <summary>
    /// Gets a hunting party by its invite code.
    /// </summary>
    Task<HuntingParty?> GetByInviteCodeAsync(
        string inviteCode,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all hunting parties created by a user.
    /// </summary>
    Task<IEnumerable<HuntingParty>> GetByCreatorIdAsync(
        string creatorId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a hunting party with its memberships included.
    /// </summary>
    Task<HuntingParty?> GetWithMembershipsAsync(
        Guid partyId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an invite code already exists.
    /// </summary>
    Task<bool> InviteCodeExistsAsync(
        string inviteCode,
        CancellationToken cancellationToken = default);
}
```

#### IHuntingPartyMembershipRepository.cs

**Create file:** `src/Application/Interfaces/Data/IHuntingPartyMembershipRepository.cs`

```csharp
using Domain.Entities;

namespace Application.Interfaces.Data;

/// <summary>
/// Repository interface for HuntingPartyMembership entity.
/// </summary>
public interface IHuntingPartyMembershipRepository : IRepository<HuntingPartyMembership>
{
    /// <summary>
    /// Gets all memberships for a user.
    /// </summary>
    Task<IEnumerable<HuntingPartyMembership>> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active memberships for a user (with party details).
    /// </summary>
    Task<IEnumerable<HuntingPartyMembership>> GetActiveByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all memberships for a party.
    /// </summary>
    Task<IEnumerable<HuntingPartyMembership>> GetByPartyIdAsync(
        Guid partyId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific membership by party and user.
    /// </summary>
    Task<HuntingPartyMembership?> GetByPartyAndUserAsync(
        Guid partyId,
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user is already a member of a party.
    /// </summary>
    Task<bool> IsMemberAsync(
        Guid partyId,
        string userId,
        CancellationToken cancellationToken = default);
}
```

**Test:** Build the Application project
```bash
cd src/Application
dotnet build
# Should compile successfully with 0 errors
```

---

### Step 1.4: Create Unit of Work Interface

**Create file:** `src/Application/Interfaces/Data/IUnitOfWork.cs`

```csharp
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
```

**Test:** Build the Application project
```bash
cd src/Application
dotnet build
# Should compile successfully
```

**✅ Checkpoint 1.4 Complete:** All interfaces defined

---

### Step 1.5: Create Repository Implementations Directory

```bash
# Create directory structure
mkdir -p src/Infrastructure/Data/Repositories
```

---

### Step 1.6: Implement Generic Repository Base Class

**Create file:** `src/Infrastructure/Data/Repositories/Repository.cs`

```csharp
using Application.Interfaces.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

/// <summary>
/// Base repository implementation providing common CRUD operations.
/// All concrete repositories inherit from this class.
/// </summary>
/// <typeparam name="TEntity">The entity type this repository manages</typeparam>
public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = context.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    public virtual void Update(TEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        _dbSet.Update(entity);
    }

    public virtual void Delete(TEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        _dbSet.Remove(entity);
    }

    public virtual async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        return entity != null;
    }
}
```

**Test:** Build the Infrastructure project
```bash
cd src/Infrastructure
dotnet build
# Should compile successfully
```

---

### Step 1.7: Implement ApplicationRepository

**Create file:** `src/Infrastructure/Data/Repositories/ApplicationRepository.cs`

```csharp
using Application.Interfaces.Data;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for Application entity.
/// </summary>
public class ApplicationRepository : Repository<Application>, IApplicationRepository
{
    public ApplicationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Application>> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Application>> GetWithTimelineEventsAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.TimelineEvents.OrderByDescending(te => te.Timestamp))
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Application?> GetByIdWithTimelineAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.TimelineEvents.OrderByDescending(te => te.Timestamp))
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<int> CountByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .CountAsync(a => a.UserId == userId, cancellationToken);
    }

    public async Task<IEnumerable<Application>> GetByStatusAsync(
        string userId,
        ApplicationStatus status,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(a => a.UserId == userId && a.Status == status)
            .OrderByDescending(a => a.CreatedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Application>> GetRecentAsync(
        string userId,
        int count,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedDate)
            .Take(count)
            .ToListAsync(cancellationToken);
    }
}
```

---

### Step 1.8: Implement TimelineEventRepository

**Create file:** `src/Infrastructure/Data/Repositories/TimelineEventRepository.cs`

```csharp
using Application.Interfaces.Data;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for TimelineEvent entity.
/// </summary>
public class TimelineEventRepository : Repository<TimelineEvent>, ITimelineEventRepository
{
    public TimelineEventRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TimelineEvent>> GetByApplicationIdAsync(
        Guid applicationId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(te => te.ApplicationId == applicationId)
            .OrderByDescending(te => te.Timestamp)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TimelineEvent>> GetByEventTypeAsync(
        Guid applicationId,
        EventType eventType,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(te => te.ApplicationId == applicationId && te.EventType == eventType)
            .OrderByDescending(te => te.Timestamp)
            .ToListAsync(cancellationToken);
    }

    public async Task<TimelineEvent?> GetMostRecentAsync(
        Guid applicationId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(te => te.ApplicationId == applicationId)
            .OrderByDescending(te => te.Timestamp)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CountByApplicationIdAsync(
        Guid applicationId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .CountAsync(te => te.ApplicationId == applicationId, cancellationToken);
    }
}
```

---

### Step 1.9: Implement ActivityEventRepository

**Create file:** `src/Infrastructure/Data/Repositories/ActivityEventRepository.cs`

```csharp
using Application.Interfaces.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for ActivityEvent entity.
/// </summary>
public class ActivityEventRepository : Repository<ActivityEvent>, IActivityEventRepository
{
    public ActivityEventRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ActivityEvent>> GetByPartyIdAsync(
        Guid partyId,
        int limit = 50,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(ae => ae.User)
            .Where(ae => ae.PartyId == partyId)
            .OrderByDescending(ae => ae.CreatedDate)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ActivityEvent>> GetByUserIdAsync(
        string userId,
        int limit = 50,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(ae => ae.User)
            .Include(ae => ae.Party)
            .Where(ae => ae.UserId == userId)
            .OrderByDescending(ae => ae.CreatedDate)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ActivityEvent>> GetRecentForUserPartiesAsync(
        string userId,
        int limit = 50,
        CancellationToken cancellationToken = default)
    {
        // Get all party IDs the user belongs to
        var userPartyIds = await _context.HuntingPartyMemberships
            .Where(m => m.UserId == userId && m.IsActive)
            .Select(m => m.HuntingPartyId)
            .ToListAsync(cancellationToken);

        // Get recent activity events from those parties
        return await _dbSet
            .Include(ae => ae.User)
            .Include(ae => ae.Party)
            .Where(ae => userPartyIds.Contains(ae.PartyId))
            .OrderByDescending(ae => ae.CreatedDate)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }
}
```

---

### Step 1.10: Implement HuntingPartyRepository

**Create file:** `src/Infrastructure/Data/Repositories/HuntingPartyRepository.cs`

```csharp
using Application.Interfaces.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for HuntingParty entity.
/// </summary>
public class HuntingPartyRepository : Repository<HuntingParty>, IHuntingPartyRepository
{
    public HuntingPartyRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<HuntingParty?> GetByInviteCodeAsync(
        string inviteCode,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.InviteCode == inviteCode, cancellationToken);
    }

    public async Task<IEnumerable<HuntingParty>> GetByCreatorIdAsync(
        string creatorId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.CreatorId == creatorId)
            .OrderByDescending(p => p.CreatedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<HuntingParty?> GetWithMembershipsAsync(
        Guid partyId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Memberships)
                .ThenInclude(m => m.User)
            .Include(p => p.Creator)
            .FirstOrDefaultAsync(p => p.Id == partyId, cancellationToken);
    }

    public async Task<bool> InviteCodeExistsAsync(
        string inviteCode,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(p => p.InviteCode == inviteCode, cancellationToken);
    }
}
```

---

### Step 1.11: Implement HuntingPartyMembershipRepository

**Create file:** `src/Infrastructure/Data/Repositories/HuntingPartyMembershipRepository.cs`

```csharp
using Application.Interfaces.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for HuntingPartyMembership entity.
/// </summary>
public class HuntingPartyMembershipRepository : Repository<HuntingPartyMembership>, IHuntingPartyMembershipRepository
{
    public HuntingPartyMembershipRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<HuntingPartyMembership>> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(m => m.HuntingParty)
            .Where(m => m.UserId == userId)
            .OrderByDescending(m => m.JoinedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<HuntingPartyMembership>> GetActiveByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(m => m.HuntingParty)
            .Where(m => m.UserId == userId && m.IsActive)
            .OrderByDescending(m => m.JoinedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<HuntingPartyMembership>> GetByPartyIdAsync(
        Guid partyId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(m => m.User)
            .Where(m => m.HuntingPartyId == partyId)
            .OrderBy(m => m.JoinedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<HuntingPartyMembership?> GetByPartyAndUserAsync(
        Guid partyId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(m => m.HuntingParty)
            .Include(m => m.User)
            .FirstOrDefaultAsync(
                m => m.HuntingPartyId == partyId && m.UserId == userId,
                cancellationToken);
    }

    public async Task<bool> IsMemberAsync(
        Guid partyId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(
                m => m.HuntingPartyId == partyId && m.UserId == userId && m.IsActive,
                cancellationToken);
    }
}
```

**Test:** Build the Infrastructure project
```bash
cd src/Infrastructure
dotnet build
# Should compile successfully with 0 errors
```

**✅ Checkpoint 1.11 Complete:** All repository implementations created

---

### Step 1.12: Implement Unit of Work

**Create file:** `src/Infrastructure/Data/UnitOfWork.cs`

```csharp
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
```

**Test:** Build the Infrastructure project
```bash
cd src/Infrastructure
dotnet build
# Should compile successfully
```

**✅ Checkpoint 1.12 Complete:** Unit of Work implemented

---

### Step 1.13: Register Services in Dependency Injection

**Edit file:** `src/WebAPI/Program.cs`

**Find this section** (after `AddDbContext`, around line 38):

```csharp
// Add Identity with Password Policy
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // ... existing code
```

**Add BEFORE the Identity configuration:**

```csharp
// Register Unit of Work and Repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
builder.Services.AddScoped<ITimelineEventRepository, TimelineEventRepository>();
builder.Services.AddScoped<IActivityEventRepository, ActivityEventRepository>();
builder.Services.AddScoped<IHuntingPartyRepository, HuntingPartyRepository>();
builder.Services.AddScoped<IHuntingPartyMembershipRepository, HuntingPartyMembershipRepository>();
```

**Add using statements at the top of Program.cs:**

```csharp
using Application.Interfaces.Data;
using Infrastructure.Data;
using Infrastructure.Data.Repositories;
```

**Test:** Build the entire solution
```bash
cd C:\Users\Inferno\Dev\BigJobHunterPro
dotnet build
# Should compile successfully
```

**✅ Checkpoint 1.13 Complete:** Dependency injection configured

---

### Step 1.14: Refactor ApplicationService

**Edit file:** `src/Infrastructure/Services/ApplicationService.cs`

**Step 1.14.1:** Update the constructor and fields

**Find:**
```csharp
private readonly ApplicationDbContext _context;
private readonly ICurrentUserService _currentUser;
// ... other dependencies

public ApplicationService(
    ApplicationDbContext context,
    ICurrentUserService currentUser,
    // ... other parameters
```

**Replace with:**
```csharp
private readonly IUnitOfWork _unitOfWork;
private readonly ICurrentUserService _currentUser;
// ... other dependencies (keep all existing)

public ApplicationService(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    // ... other parameters (keep all existing)
```

**Update constructor body:**
```csharp
{
    _unitOfWork = unitOfWork;
    _currentUser = currentUser;
    // ... keep all other assignments
}
```

**Step 1.14.2:** Update all database operations

**Find patterns like:**
```csharp
_context.Applications.Add(application);
_context.TimelineEvents.Add(timelineEvent);
await _context.SaveChangesAsync();
```

**Replace with:**
```csharp
await _unitOfWork.Applications.AddAsync(application);
await _unitOfWork.TimelineEvents.AddAsync(timelineEvent);
await _unitOfWork.SaveChangesAsync();
```

**Find patterns like:**
```csharp
var application = await _context.Applications
    .Include(a => a.TimelineEvents)
    .FirstOrDefaultAsync(a => a.Id == id);
```

**Replace with:**
```csharp
var application = await _unitOfWork.Applications
    .GetByIdWithTimelineAsync(id);
```

**Find patterns like:**
```csharp
var applications = await _context.Applications
    .Where(a => a.UserId == userId)
    .OrderByDescending(a => a.CreatedDate)
    .ToListAsync();
```

**Replace with:**
```csharp
var applications = await _unitOfWork.Applications
    .GetByUserIdAsync(userId);
```

**Step 1.14.3:** Add using statement

**At the top of the file, add:**
```csharp
using Application.Interfaces.Data;
```

**Remove (if present):**
```csharp
using Infrastructure.Data; // No longer need direct DbContext reference
using Microsoft.EntityFrameworkCore; // Might not need this anymore
```

**Test after refactoring:**
```bash
cd src/Infrastructure
dotnet build
# Should compile successfully
```

---

### Step 1.15: Refactor Remaining Services

Apply the same refactoring pattern to all other services:

#### Services to Refactor:

1. **TimelineEventService.cs**
2. **ActivityEventService.cs**
3. **HuntingPartyService.cs**
4. **PointsService.cs**
5. **StreakService.cs**
6. **StatisticsService.cs**
7. **AnalyticsService.cs**

**Pattern for each service:**

1. Replace `ApplicationDbContext _context` with `IUnitOfWork _unitOfWork`
2. Update constructor parameter and assignment
3. Replace direct `_context.SomeEntity` calls with `_unitOfWork.SomeEntity`
4. Replace `_context.SaveChangesAsync()` with `_unitOfWork.SaveChangesAsync()`
5. Use specialized repository methods where available
6. Add `using Application.Interfaces.Data;`

**Example for TimelineEventService:**

**Before:**
```csharp
private readonly ApplicationDbContext _context;

public TimelineEventService(ApplicationDbContext context, ...)
{
    _context = context;
}

var events = await _context.TimelineEvents
    .Where(te => te.ApplicationId == applicationId)
    .ToListAsync();
```

**After:**
```csharp
private readonly IUnitOfWork _unitOfWork;

public TimelineEventService(IUnitOfWork unitOfWork, ...)
{
    _unitOfWork = unitOfWork;
}

var events = await _unitOfWork.TimelineEvents
    .GetByApplicationIdAsync(applicationId);
```

**Test after each service:**
```bash
cd src/Infrastructure
dotnet build
# Fix any compilation errors before moving to next service
```

**✅ Checkpoint 1.15 Complete:** All services refactored

---

### Step 1.16: Phase 1 Testing - Compile and Build

```bash
# Clean and rebuild entire solution
cd C:\Users\Inferno\Dev\BigJobHunterPro
dotnet clean
dotnet build

# Expected output: Build succeeded. 0 Error(s)
```

**If you have compilation errors:**
1. Read error messages carefully
2. Check that all using statements are correct
3. Verify constructor parameters match
4. Ensure all `_context` references are replaced with `_unitOfWork`

---

### Step 1.17: Phase 1 Testing - Run Unit Tests

```bash
cd C:\Users\Inferno\Dev\BigJobHunterPro
dotnet test

# Compare results with baseline from pre-implementation checklist
# All tests should pass
```

**If tests fail:**
1. Check if the failure is due to refactoring or pre-existing
2. Debug failing tests
3. Fix repository implementations if needed

---

### Step 1.18: Phase 1 Testing - Manual Application Testing

**Start the application:**
```bash
cd src/WebAPI
dotnet run
```

**Test these endpoints using Postman/curl/browser:**

#### Test 1: Register New User
```bash
POST http://localhost:5000/api/auth/register
Content-Type: application/json

{
  "email": "test-phase1@example.com",
  "password": "Test123!",
  "displayName": "Phase 1 Tester"
}

# Expected: 200 OK with JWT token
```

#### Test 2: Login
```bash
POST http://localhost:5000/api/auth/login
Content-Type: application/json

{
  "email": "test-phase1@example.com",
  "password": "Test123!"
}

# Expected: 200 OK with JWT token
# Save the token for next requests
```

#### Test 3: Create Application
```bash
POST http://localhost:5000/api/applications
Authorization: Bearer YOUR_TOKEN
Content-Type: application/json

{
  "sourceUrl": "https://example.com/job",
  "rawPageContent": "Software Engineer at Test Company\nRequirements: C#, .NET"
}

# Expected: 201 Created with application ID
# Save the application ID
```

#### Test 4: Get Applications
```bash
GET http://localhost:5000/api/applications
Authorization: Bearer YOUR_TOKEN

# Expected: 200 OK with list including the application you just created
```

#### Test 5: Get Single Application with Timeline
```bash
GET http://localhost:5000/api/applications/{APPLICATION_ID}
Authorization: Bearer YOUR_TOKEN

# Expected: 200 OK with application details and timeline events
```

#### Test 6: Create Hunting Party
```bash
POST http://localhost:5000/api/hunting-parties
Authorization: Bearer YOUR_TOKEN
Content-Type: application/json

{
  "name": "Phase 1 Test Party"
}

# Expected: 201 Created with party details and invite code
```

#### Test 7: Get User's Hunting Parties
```bash
GET http://localhost:5000/api/hunting-parties
Authorization: Bearer YOUR_TOKEN

# Expected: 200 OK with list of parties
```

#### Test 8: Activity Feed
```bash
GET http://localhost:5000/api/activity-feed
Authorization: Bearer YOUR_TOKEN

# Expected: 200 OK with recent activity events
```

**✅ All tests passing?**
- If YES: Phase 1 complete! Move to Phase 2.
- If NO: Debug issues before proceeding.

---

### Step 1.19: Commit Phase 1 Changes

```bash
cd C:\Users\Inferno\Dev\BigJobHunterPro

# Stage all changes
git add .

# Commit with descriptive message
git commit -m "feat: Implement repository pattern and Unit of Work

- Add generic IRepository<T> and specialized repository interfaces
- Implement concrete repositories for all entities
- Implement UnitOfWork for transaction management
- Refactor all services to use IUnitOfWork instead of DbContext
- Register repositories in dependency injection
- Maintain 100% backward compatibility (no database changes)

Phase 1 Complete: Database layer abstraction implemented
Tested: All unit tests passing, manual API testing successful"

# Push to remote
git push origin feature/supabase-postgres-migration
```

---

### ✅ Phase 1 Complete Checklist

- [ ] All repository interfaces created (7 files)
- [ ] All repository implementations created (6 files)
- [ ] Unit of Work implemented
- [ ] All services refactored (8 files)
- [ ] Dependency injection configured
- [ ] Solution builds without errors
- [ ] All unit tests pass
- [ ] Manual API testing successful:
  - [ ] User registration
  - [ ] User login
  - [ ] Create application
  - [ ] Get applications
  - [ ] Get single application with timeline
  - [ ] Create hunting party
  - [ ] Get hunting parties
  - [ ] Activity feed
- [ ] Changes committed and pushed

**✅ Phase 1 Sign-Off:**

Date: _______________
Tester: _______________
Status: ☐ Pass  ☐ Fail (if fail, document issues below)

Issues (if any):
_______________________________________________________
_______________________________________________________

---

## Phase 2: PostgreSQL Setup & Local Migration

**Duration:** 1-2 days
**Goal:** Migrate to PostgreSQL locally WITHOUT touching production
**Risk Level:** Medium (database provider change, but local only)

### Why Phase 2?

Phase 1 validated our architecture works with the existing database. Now we can safely change the database provider and test PostgreSQL locally before touching production data.

---

### Step 2.1: Create Docker Compose File

**Create file:** `C:\Users\Inferno\Dev\BigJobHunterPro\docker-compose.yml`

```yaml
version: '3.8'

services:
  postgres:
    image: postgres:16-alpine
    container_name: bigjobhunterpro-postgres
    environment:
      POSTGRES_DB: bigjobhunterpro_dev
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: localdev123
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres"]
      interval: 10s
      timeout: 5s
      retries: 5

volumes:
  postgres_data:
```

**Test:** Start PostgreSQL container
```bash
cd C:\Users\Inferno\Dev\BigJobHunterPro
docker-compose up -d postgres

# Check container is running
docker ps
# Should show: bigjobhunterpro-postgres with status "Up"

# Check PostgreSQL is ready
docker-compose logs postgres
# Should show: "database system is ready to accept connections"
```

**Verify connection:**
```bash
# Using Docker exec
docker exec -it bigjobhunterpro-postgres psql -U postgres -d bigjobhunterpro_dev -c "\dt"
# Should show: "Did not find any relations" (empty database, this is expected)
```

**✅ Checkpoint 2.1 Complete:** PostgreSQL running locally

---

### Step 2.2: Update Infrastructure Package References

**Edit file:** `src/Infrastructure/Infrastructure.csproj`

**Find:**
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.11" />
```

**Replace with:**
```xml
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.11" />
```

**Keep these packages:**
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="8.0.11" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.11">
```

**Restore packages:**
```bash
cd src/Infrastructure
dotnet restore

# Expected output: Restore succeeded
```

**Test:** Build the project
```bash
dotnet build
# Should compile successfully
```

**✅ Checkpoint 2.2 Complete:** Npgsql package installed

---

### Step 2.3: Update Program.cs for PostgreSQL

**Edit file:** `src/WebAPI/Program.cs`

**Find (around line 21-38):**
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (useSqlite)
    {
        var dbPath = Path.Combine(Environment.CurrentDirectory, "bigjobhunterpro.db");
        options.UseSqlite($"Data Source={dbPath}");
    }
    else
    {
        options.UseSqlServer(connectionString, sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        });
    }
});
```

**Replace with:**
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (useSqlite)
    {
        var dbPath = Path.Combine(Environment.CurrentDirectory, "bigjobhunterpro.db");
        options.UseSqlite($"Data Source={dbPath}");
    }
    else
    {
        options.UseNpgsql(connectionString, npgsqlOptions =>
        {
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryAttempts: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorCodesToAdd: null);
            npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory");
        });
    }
});
```

**Test:** Build the WebAPI project
```bash
cd src/WebAPI
dotnet build
# Should compile successfully
```

**✅ Checkpoint 2.3 Complete:** Program.cs updated for PostgreSQL

---

### Step 2.4: Set PostgreSQL Connection String

**Option A: User Secrets (Recommended for Development)**

```bash
cd src/WebAPI

dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=bigjobhunterpro_dev;Username=postgres;Password=localdev123"
```

**Option B: appsettings.Development.json (Alternative)**

**Edit:** `src/WebAPI/appsettings.Development.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=bigjobhunterpro_dev;Username=postgres;Password=localdev123"
  },
  "Logging": {
    ...
  }
}
```

**Verify connection string is set:**
```bash
cd src/WebAPI
dotnet user-secrets list
# Should show: ConnectionStrings:DefaultConnection = Host=localhost...
```

---

### Step 2.5: Backup and Remove Old Migrations

**IMPORTANT:** Backup before deleting!

```bash
cd C:\Users\Inferno\Dev\BigJobHunterPro

# Create backup commit
git add src/Infrastructure/Migrations/
git commit -m "backup: Save SQL Server migrations before deletion"

# Delete old migration files (keep the directory)
Remove-Item src\Infrastructure\Migrations\*.cs
# Or on bash: rm src/Infrastructure/Migrations/*.cs

# Verify migrations deleted
ls src/Infrastructure/Migrations/
# Should be empty (or only .gitkeep if present)
```

---

### Step 2.6: Generate PostgreSQL Migrations

```bash
cd C:\Users\Inferno\Dev\BigJobHunterPro

dotnet ef migrations add InitialPostgres --project src/Infrastructure --startup-project src/WebAPI

# Expected output:
# Build started...
# Build succeeded.
# Done. To undo this action, use 'ef migrations remove'
```

**Verify migration files created:**
```bash
ls src/Infrastructure/Migrations/
# Should show:
# - 20260116XXXXXX_InitialPostgres.cs
# - 20260116XXXXXX_InitialPostgres.Designer.cs
# - ApplicationDbContextModelSnapshot.cs
```

**Review migration file:**
```bash
# Open the migration file and verify it looks correct
# Check for PostgreSQL-specific syntax (no SQL Server T-SQL)
code src/Infrastructure/Migrations/*_InitialPostgres.cs
```

**Key things to verify in migration:**
- Uses `uuid` instead of `uniqueidentifier`
- Uses `text` instead of `nvarchar(max)`
- Uses `timestamp without time zone` instead of `datetime2`
- Uses `boolean` instead of `bit`
- No SQL Server-specific syntax (like `IF OBJECT_ID`)

**✅ Checkpoint 2.6 Complete:** PostgreSQL migrations generated

---

### Step 2.7: Apply Migrations to Local PostgreSQL

```bash
cd C:\Users\Inferno\Dev\BigJobHunterPro

dotnet ef database update --project src/Infrastructure --startup-project src/WebAPI

# Expected output:
# Build started...
# Build succeeded.
# Applying migration '20260116XXXXXX_InitialPostgres'.
# Done.
```

**Verify tables created:**
```bash
docker exec -it bigjobhunterpro-postgres psql -U postgres -d bigjobhunterpro_dev -c "\dt"

# Expected output: List of tables including:
# - AspNetUsers
# - Applications
# - TimelineEvents
# - ActivityEvents
# - HuntingParties
# - HuntingPartyMemberships
# - AspNetRoles
# - AspNetUserRoles
# - AspNetUserClaims
# - AspNetUserLogins
# - AspNetUserTokens
# - AspNetRoleClaims
# - __EFMigrationsHistory
```

**Check migration history:**
```bash
docker exec -it bigjobhunterpro-postgres psql -U postgres -d bigjobhunterpro_dev -c "SELECT * FROM \"__EFMigrationsHistory\";"

# Should show: InitialPostgres migration applied
```

**✅ Checkpoint 2.7 Complete:** Database schema created in PostgreSQL

---

### Step 2.8: Phase 2 Testing - Start Application

```bash
cd src/WebAPI
dotnet run

# Watch for startup messages
# Should see: "Now listening on: http://localhost:5000"
# Should NOT see any database connection errors
```

**If you see errors:**
- Check PostgreSQL container is running: `docker ps`
- Verify connection string: `dotnet user-secrets list`
- Check PostgreSQL logs: `docker-compose logs postgres`

**✅ Checkpoint 2.8 Complete:** Application starts successfully with PostgreSQL

---

### Step 2.9: Phase 2 Testing - API Endpoints

**Test 1: Register New User (PostgreSQL)**
```bash
POST http://localhost:5000/api/auth/register
Content-Type: application/json

{
  "email": "test-phase2@example.com",
  "password": "Test123!",
  "displayName": "Phase 2 Tester"
}

# Expected: 200 OK with JWT token
```

**Verify in database:**
```bash
docker exec -it bigjobhunterpro-postgres psql -U postgres -d bigjobhunterpro_dev -c "SELECT \"Id\", \"Email\", \"DisplayName\" FROM \"AspNetUsers\";"

# Should show: test-phase2@example.com user
```

**Test 2: Login**
```bash
POST http://localhost:5000/api/auth/login
Content-Type: application/json

{
  "email": "test-phase2@example.com",
  "password": "Test123!"
}

# Expected: 200 OK with JWT token
# Save token for next requests
```

**Test 3: Create Application**
```bash
POST http://localhost:5000/api/applications
Authorization: Bearer YOUR_TOKEN
Content-Type: application/json

{
  "sourceUrl": "https://example.com/postgres-job",
  "rawPageContent": "PostgreSQL DBA at Test Company\nRequirements: PostgreSQL, SQL"
}

# Expected: 201 Created with application ID
```

**Verify in database:**
```bash
docker exec -it bigjobhunterpro-postgres psql -U postgres -d bigjobhunterpro_dev -c "SELECT \"Id\", \"CompanyName\", \"RoleTitle\", \"Status\" FROM \"Applications\";"

# Should show: The application you just created (might have empty CompanyName/RoleTitle if AI parsing is pending)
```

**Test 4: Get Applications**
```bash
GET http://localhost:5000/api/applications
Authorization: Bearer YOUR_TOKEN

# Expected: 200 OK with list of applications
```

**Test 5: Create Timeline Event**
```bash
POST http://localhost:5000/api/applications/{APPLICATION_ID}/timeline-events
Authorization: Bearer YOUR_TOKEN
Content-Type: application/json

{
  "eventType": "Screening",
  "notes": "Phone screen scheduled for next week"
}

# Expected: 201 Created
```

**Verify in database:**
```bash
docker exec -it bigjobhunterpro-postgres psql -U postgres -d bigjobhunterpro_dev -c "SELECT \"Id\", \"EventType\", \"Notes\", \"Points\" FROM \"TimelineEvents\";"

# Should show: Both the initial "Applied" event and the new "Screening" event
```

**Test 6: Points Calculation**
```bash
GET http://localhost:5000/api/auth/me
Authorization: Bearer YOUR_TOKEN

# Expected: 200 OK
# Check "points" field - should be 3 (1 for Applied + 2 for Screening)
```

**Test 7: Create Hunting Party**
```bash
POST http://localhost:5000/api/hunting-parties
Authorization: Bearer YOUR_TOKEN
Content-Type: application/json

{
  "name": "PostgreSQL Test Party"
}

# Expected: 201 Created with party ID and invite code
```

**Verify in database:**
```bash
docker exec -it bigjobhunterpro-postgres psql -U postgres -d bigjobhunterpro_dev -c "SELECT \"Id\", \"Name\", \"InviteCode\" FROM \"HuntingParties\";"

# Should show: The party you just created
```

**Test 8: Activity Feed**
```bash
GET http://localhost:5000/api/activity-feed
Authorization: Bearer YOUR_TOKEN

# Expected: 200 OK with activity events
```

---

### Step 2.10: Phase 2 Testing - Data Type Verification

**Test UUID columns:**
```bash
docker exec -it bigjobhunterpro-postgres psql -U postgres -d bigjobhunterpro_dev -c "SELECT column_name, data_type FROM information_schema.columns WHERE table_name = 'Applications' AND column_name = 'Id';"

# Expected: data_type = uuid
```

**Test boolean columns:**
```bash
docker exec -it bigjobhunterpro-postgres psql -U postgres -d bigjobhunterpro_dev -c "SELECT column_name, data_type FROM information_schema.columns WHERE table_name = 'HuntingPartyMemberships' AND column_name = 'IsActive';"

# Expected: data_type = boolean
```

**Test timestamp columns:**
```bash
docker exec -it bigjobhunterpro-postgres psql -U postgres -d bigjobhunterpro_dev -c "SELECT column_name, data_type FROM information_schema.columns WHERE table_name = 'Applications' AND column_name = 'CreatedDate';"

# Expected: data_type = timestamp without time zone
```

**Test text columns:**
```bash
docker exec -it bigjobhunterpro-postgres psql -U postgres -d bigjobhunterpro_dev -c "SELECT column_name, data_type FROM information_schema.columns WHERE table_name = 'Applications' AND column_name = 'RawPageContent';"

# Expected: data_type = text
```

**✅ All data types correct?** If yes, proceed. If no, check migration files.

---

### Step 2.11: Phase 2 Testing - JSON Serialization

**Test JSON fields (RequiredSkills, NiceToHaveSkills):**

Create a test application with skills:
```bash
# First, manually update an application to add skills
docker exec -it bigjobhunterpro-postgres psql -U postgres -d bigjobhunterpro_dev

# In psql:
UPDATE "Applications"
SET "RequiredSkills" = '["C#", ".NET", "PostgreSQL"]',
    "NiceToHaveSkills" = '["Docker", "Kubernetes"]'
WHERE "Id" = 'YOUR_APPLICATION_ID';

# Exit psql
\q
```

**Fetch via API:**
```bash
GET http://localhost:5000/api/applications/{APPLICATION_ID}
Authorization: Bearer YOUR_TOKEN

# Expected: 200 OK
# Verify "requiredSkills" and "niceToHaveSkills" arrays are properly deserialized
```

**✅ Checkpoint 2.11 Complete:** JSON serialization working correctly

---

### Step 2.12: Phase 2 Testing - Run Unit Tests

```bash
cd C:\Users\Inferno\Dev\BigJobHunterPro
dotnet test

# All tests should pass
# Compare with Phase 1 baseline
```

**If tests fail:**
1. Check if tests are using in-memory database or real database
2. Update test configurations if needed
3. Some tests might need PostgreSQL-specific adjustments

---

### Step 2.13: Phase 2 Testing - Performance Baseline

**Test query performance:**

```bash
# Check execution time for common queries
docker exec -it bigjobhunterpro-postgres psql -U postgres -d bigjobhunterpro_dev

# In psql:
\timing on

-- Test 1: Get applications for user
SELECT * FROM "Applications" WHERE "UserId" = 'YOUR_USER_ID' ORDER BY "CreatedDate" DESC;

-- Test 2: Get application with timeline events
SELECT a.*, te.*
FROM "Applications" a
LEFT JOIN "TimelineEvents" te ON te."ApplicationId" = a."Id"
WHERE a."Id" = 'YOUR_APPLICATION_ID';

-- Test 3: Activity feed query
SELECT * FROM "ActivityEvents"
WHERE "PartyId" = 'YOUR_PARTY_ID'
ORDER BY "CreatedDate" DESC
LIMIT 50;

\timing off
\q
```

**Record baseline performance** (save for comparison after production migration).

---

### Step 2.14: Create .env.example File

**Create file:** `C:\Users\Inferno\Dev\BigJobHunterPro\.env.example`

```env
# Database Configuration
# For local development with Docker PostgreSQL:
ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=bigjobhunterpro_dev;Username=postgres;Password=localdev123

# For Supabase (Production):
# ConnectionStrings__DefaultConnection=Host=db.YOUR_PROJECT_REF.supabase.co;Port=6543;Database=postgres;Username=postgres;Password=YOUR_PASSWORD;Pooling Mode=Session

# JWT Configuration (use dotnet user-secrets in development)
JwtSettings__Secret=your-jwt-secret-key-here-min-32-chars
JwtSettings__Issuer=BigJobHunterPro
JwtSettings__Audience=BigJobHunterProUsers
JwtSettings__ExpirationHours=24

# AI Parsing (if using)
OpenAI__ApiKey=your-openai-api-key
OpenAI__Model=gpt-4

# Environment
ASPNETCORE_ENVIRONMENT=Development
```

**Add to .gitignore** (if not already present):
```bash
echo ".env" >> .gitignore
```

---

### Step 2.15: Update Documentation

**Update file:** `CLAUDE.md`

**Find the "Commands" section and add:**

```markdown
### Docker (Local PostgreSQL)
```bash
docker-compose up -d postgres         # Start PostgreSQL
docker-compose logs -f postgres       # View logs
docker-compose down                   # Stop PostgreSQL
docker-compose down -v                # Stop and remove data volume
```

### Database Migrations
```bash
# Generate new migration
dotnet ef migrations add MigrationName --project src/Infrastructure --startup-project src/WebAPI

# Apply migrations
dotnet ef database update --project src/Infrastructure --startup-project src/WebAPI

# Rollback to specific migration
dotnet ef database update PreviousMigrationName --project src/Infrastructure --startup-project src/WebAPI
```

### PostgreSQL Client (via Docker)
```bash
# Connect to local PostgreSQL
docker exec -it bigjobhunterpro-postgres psql -U postgres -d bigjobhunterpro_dev

# Useful psql commands:
# \dt                  - List tables
# \d "TableName"       - Describe table structure
# \q                   - Quit
```
```

---

### Step 2.16: Commit Phase 2 Changes

```bash
cd C:\Users\Inferno\Dev\BigJobHunterPro

# Stage all changes
git add .

# Commit
git commit -m "feat: Migrate to PostgreSQL with Npgsql provider

- Replace SqlServer package with Npgsql.EntityFrameworkCore.PostgreSQL
- Update Program.cs to use UseNpgsql()
- Generate new PostgreSQL migrations
- Add Docker Compose for local PostgreSQL
- Create .env.example for configuration reference
- Update CLAUDE.md with Docker commands

Phase 2 Complete: Application running on local PostgreSQL
Tested: All API endpoints functional, data types verified"

# Push to remote
git push origin feature/supabase-postgres-migration
```

---

### ✅ Phase 2 Complete Checklist

- [ ] Docker Compose file created
- [ ] PostgreSQL container running
- [ ] Npgsql package installed
- [ ] Program.cs updated for PostgreSQL
- [ ] Connection string configured
- [ ] Old migrations backed up and removed
- [ ] New PostgreSQL migrations generated
- [ ] Migrations applied to local database
- [ ] Application starts without errors
- [ ] All API endpoints tested:
  - [ ] User registration
  - [ ] User login
  - [ ] Create application
  - [ ] Get applications
  - [ ] Create timeline event
  - [ ] Points calculation
  - [ ] Create hunting party
  - [ ] Activity feed
- [ ] Data types verified (uuid, boolean, timestamp, text)
- [ ] JSON serialization working
- [ ] Unit tests passing
- [ ] Performance baseline recorded
- [ ] Documentation updated
- [ ] Changes committed and pushed

**✅ Phase 2 Sign-Off:**

Date: _______________
Tester: _______________
PostgreSQL Version: 16
Status: ☐ Pass  ☐ Fail (if fail, document issues below)

Issues (if any):
_______________________________________________________
_______________________________________________________

---

## Phase 3: Supabase Production Migration

**Duration:** 1-2 days
**Goal:** Migrate production data to Supabase and deploy
**Risk Level:** High (production data and deployment)

### Why Phase 3 Last?

Phases 1 and 2 validated:
- ✅ Repository architecture works correctly
- ✅ Application functions identically with PostgreSQL
- ✅ All data types and queries work

Now we can confidently migrate production data to Supabase.

---

### Step 3.1: Create Supabase Project

1. Go to [supabase.com](https://supabase.com)
2. Sign in or create account
3. Click "New Project"
4. Fill in details:
   - **Organization:** Your organization (or create new)
   - **Project Name:** `bigjobhunterpro-prod`
   - **Database Password:** Generate strong password (save in password manager!)
   - **Region:** Choose closest to your users (e.g., us-east-1, eu-west-1)
   - **Pricing Plan:** Start with Free tier
5. Click "Create new project"
6. Wait 2-3 minutes for provisioning

**Save these details:**
- Project Reference ID: `_________________`
- Database Password: `_________________`
- Project URL: `https://____________.supabase.co`

**✅ Checkpoint 3.1 Complete:** Supabase project created

---

### Step 3.2: Get Supabase Connection String

1. In Supabase Dashboard, go to **Settings** → **Database**
2. Scroll to **Connection String** section
3. Select **Connection Pooling** tab (Port 6543)
4. Copy the connection string (Transaction mode)
5. Replace `[YOUR-PASSWORD]` with your actual password

**Connection string format:**
```
Host=db.YOUR_PROJECT_REF.supabase.co;Port=6543;Database=postgres;Username=postgres;Password=YOUR_PASSWORD;Pooling Mode=Session
```

**Save this connection string** (you'll need it in Step 3.6)

---

### Step 3.3: Apply Migrations to Supabase

**Set Supabase connection string temporarily:**

```bash
cd src/WebAPI

# Save current connection string (backup)
$env:LOCAL_CONNECTION=$(dotnet user-secrets list | Select-String "ConnectionStrings:DefaultConnection")

# Set Supabase connection string
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=db.YOUR_PROJECT_REF.supabase.co;Port=6543;Database=postgres;Username=postgres;Password=YOUR_PASSWORD;Pooling Mode=Session"
```

**Apply migrations:**
```bash
cd C:\Users\Inferno\Dev\BigJobHunterPro

dotnet ef database update --project src/Infrastructure --startup-project src/WebAPI

# Expected output:
# Applying migration '20260116XXXXXX_InitialPostgres'.
# Done.
```

**Verify tables created in Supabase:**

1. Go to Supabase Dashboard → **Table Editor**
2. You should see tables:
   - AspNetUsers
   - Applications
   - TimelineEvents
   - ActivityEvents
   - HuntingParties
   - HuntingPartyMemberships
   - + AspNet Identity tables

**✅ Checkpoint 3.3 Complete:** Schema created in Supabase

---

### Step 3.4: Disable Row Level Security

Supabase enables RLS by default. Since we're using ASP.NET Core Identity for auth, disable RLS.

1. In Supabase Dashboard, go to **SQL Editor**
2. Click **New Query**
3. Paste and run:

```sql
-- Disable Row Level Security on all application tables
ALTER TABLE "AspNetUsers" DISABLE ROW LEVEL SECURITY;
ALTER TABLE "AspNetRoles" DISABLE ROW LEVEL SECURITY;
ALTER TABLE "AspNetUserRoles" DISABLE ROW LEVEL SECURITY;
ALTER TABLE "AspNetUserClaims" DISABLE ROW LEVEL SECURITY;
ALTER TABLE "AspNetUserLogins" DISABLE ROW LEVEL SECURITY;
ALTER TABLE "AspNetUserTokens" DISABLE ROW LEVEL SECURITY;
ALTER TABLE "AspNetRoleClaims" DISABLE ROW LEVEL SECURITY;
ALTER TABLE "Applications" DISABLE ROW LEVEL SECURITY;
ALTER TABLE "TimelineEvents" DISABLE ROW LEVEL SECURITY;
ALTER TABLE "ActivityEvents" DISABLE ROW LEVEL SECURITY;
ALTER TABLE "HuntingParties" DISABLE ROW LEVEL SECURITY;
ALTER TABLE "HuntingPartyMemberships" DISABLE ROW LEVEL SECURITY;
```

4. Click **Run** (or F5)
5. Expected: "Success. No rows returned"

**✅ Checkpoint 3.4 Complete:** RLS disabled

---

### Step 3.5: Export Data from Azure SQL Server

**Option A: Using Azure Data Studio (Recommended)**

1. Install [Azure Data Studio](https://docs.microsoft.com/en-us/sql/azure-data-studio/download-azure-data-studio)
2. Connect to Azure SQL Server
3. Right-click database → **Tasks** → **Export Data**
4. Choose format: CSV or JSON
5. Export tables in this order:
   - AspNetUsers (and other Identity tables)
   - HuntingParties
   - HuntingPartyMemberships
   - Applications
   - TimelineEvents
   - ActivityEvents

**Save exports to:** `C:\Users\Inferno\Dev\BigJobHunterPro\data-export\`

**Option B: Using SQL Query**

```sql
-- Connect to Azure SQL Server via SSMS or Azure Data Studio

-- Export AspNetUsers
SELECT * FROM AspNetUsers;
-- Save as CSV

-- Export Applications
SELECT * FROM Applications;
-- Save as CSV

-- Repeat for all tables...
```

**✅ Checkpoint 3.5 Complete:** Data exported from Azure SQL

---

### Step 3.6: Create Data Migration Tool

**Create directory:**
```bash
mkdir -p src/Tools/DataMigration
```

**Create file:** `src/Tools/DataMigration/DataMigration.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\Infrastructure\Infrastructure.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.11" />
    <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.11" />
    <PackageReference Include="Microsoft.Extensions.Configuration" Version="8.0.*" />
    <PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="8.0.*" />
    <PackageReference Include="Microsoft.Extensions.Configuration.EnvironmentVariables" Version="8.0.*" />
  </ItemGroup>
</Project>
```

**Create file:** `src/Tools/DataMigration/Program.cs`

```csharp
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("=== Big Job Hunter Pro - Data Migration Tool ===");
Console.WriteLine();

// Load configuration
var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

// Get connection strings
Console.Write("Enter Azure SQL Server connection string: ");
var sourceConnectionString = Console.ReadLine();

Console.Write("Enter Supabase PostgreSQL connection string: ");
var targetConnectionString = Console.ReadLine();

if (string.IsNullOrWhiteSpace(sourceConnectionString) || string.IsNullOrWhiteSpace(targetConnectionString))
{
    Console.WriteLine("ERROR: Both connection strings are required.");
    return;
}

Console.WriteLine();
Console.WriteLine("Setting up database contexts...");

// Create service provider for source (SQL Server)
var sourceServices = new ServiceCollection();
sourceServices.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(sourceConnectionString));
sourceServices.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

var sourceProvider = sourceServices.BuildServiceProvider();
var sourceContext = sourceProvider.GetRequiredService<ApplicationDbContext>();

// Create service provider for target (PostgreSQL)
var targetServices = new ServiceCollection();
targetServices.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(targetConnectionString));
targetServices.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

var targetProvider = targetServices.BuildServiceProvider();
var targetContext = targetProvider.GetRequiredService<ApplicationDbContext>();
var userManager = targetProvider.GetRequiredService<UserManager<ApplicationUser>>();

try
{
    Console.WriteLine("Testing connections...");
    await sourceContext.Database.CanConnectAsync();
    Console.WriteLine("✓ Source (Azure SQL) connected");

    await targetContext.Database.CanConnectAsync();
    Console.WriteLine("✓ Target (Supabase PostgreSQL) connected");

    Console.WriteLine();
    Console.WriteLine("=== Starting Data Migration ===");
    Console.WriteLine();

    // Step 1: Migrate Users
    Console.WriteLine("Step 1: Migrating users...");
    var sourceUsers = await sourceContext.Users.ToListAsync();
    Console.WriteLine($"Found {sourceUsers.Count} users in source database");

    foreach (var user in sourceUsers)
    {
        var existingUser = await userManager.FindByIdAsync(user.Id);
        if (existingUser == null)
        {
            // Create user with existing hash
            var result = await userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                Console.WriteLine($"  ✓ Migrated user: {user.Email}");
            }
            else
            {
                Console.WriteLine($"  ✗ Failed to migrate user: {user.Email}");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"    - {error.Description}");
                }
            }
        }
        else
        {
            Console.WriteLine($"  → User already exists: {user.Email}");
        }
    }

    // Step 2: Migrate Identity Roles, Claims, etc.
    Console.WriteLine();
    Console.WriteLine("Step 2: Migrating identity data...");

    var roles = await sourceContext.Roles.ToListAsync();
    Console.WriteLine($"Found {roles.Count} roles");
    foreach (var role in roles)
    {
        if (!await targetContext.Roles.AnyAsync(r => r.Id == role.Id))
        {
            targetContext.Roles.Add(role);
        }
    }

    var userRoles = await sourceContext.UserRoles.ToListAsync();
    Console.WriteLine($"Found {userRoles.Count} user roles");
    foreach (var userRole in userRoles)
    {
        if (!await targetContext.UserRoles.AnyAsync(ur => ur.UserId == userRole.UserId && ur.RoleId == userRole.RoleId))
        {
            targetContext.UserRoles.Add(userRole);
        }
    }

    await targetContext.SaveChangesAsync();
    Console.WriteLine("✓ Identity data migrated");

    // Step 3: Migrate Hunting Parties
    Console.WriteLine();
    Console.WriteLine("Step 3: Migrating hunting parties...");
    var parties = await sourceContext.HuntingParties.ToListAsync();
    Console.WriteLine($"Found {parties.Count} hunting parties");

    foreach (var party in parties)
    {
        if (!await targetContext.HuntingParties.AnyAsync(p => p.Id == party.Id))
        {
            targetContext.HuntingParties.Add(party);
            Console.WriteLine($"  ✓ Migrated party: {party.Name}");
        }
    }
    await targetContext.SaveChangesAsync();

    // Step 4: Migrate Party Memberships
    Console.WriteLine();
    Console.WriteLine("Step 4: Migrating party memberships...");
    var memberships = await sourceContext.HuntingPartyMemberships.ToListAsync();
    Console.WriteLine($"Found {memberships.Count} memberships");

    foreach (var membership in memberships)
    {
        if (!await targetContext.HuntingPartyMemberships.AnyAsync(m => m.Id == membership.Id))
        {
            targetContext.HuntingPartyMemberships.Add(membership);
        }
    }
    await targetContext.SaveChangesAsync();
    Console.WriteLine("✓ Memberships migrated");

    // Step 5: Migrate Applications
    Console.WriteLine();
    Console.WriteLine("Step 5: Migrating applications...");
    var applications = await sourceContext.Applications.ToListAsync();
    Console.WriteLine($"Found {applications.Count} applications");

    foreach (var app in applications)
    {
        if (!await targetContext.Applications.AnyAsync(a => a.Id == app.Id))
        {
            targetContext.Applications.Add(app);
        }
    }
    await targetContext.SaveChangesAsync();
    Console.WriteLine("✓ Applications migrated");

    // Step 6: Migrate Timeline Events
    Console.WriteLine();
    Console.WriteLine("Step 6: Migrating timeline events...");
    var events = await sourceContext.TimelineEvents.ToListAsync();
    Console.WriteLine($"Found {events.Count} timeline events");

    foreach (var evt in events)
    {
        if (!await targetContext.TimelineEvents.AnyAsync(e => e.Id == evt.Id))
        {
            targetContext.TimelineEvents.Add(evt);
        }
    }
    await targetContext.SaveChangesAsync();
    Console.WriteLine("✓ Timeline events migrated");

    // Step 7: Migrate Activity Events
    Console.WriteLine();
    Console.WriteLine("Step 7: Migrating activity events...");
    var activityEvents = await sourceContext.ActivityEvents.ToListAsync();
    Console.WriteLine($"Found {activityEvents.Count} activity events");

    foreach (var activityEvent in activityEvents)
    {
        if (!await targetContext.ActivityEvents.AnyAsync(ae => ae.Id == activityEvent.Id))
        {
            targetContext.ActivityEvents.Add(activityEvent);
        }
    }
    await targetContext.SaveChangesAsync();
    Console.WriteLine("✓ Activity events migrated");

    // Validation
    Console.WriteLine();
    Console.WriteLine("=== Validation ===");
    Console.WriteLine();

    var sourceUserCount = await sourceContext.Users.CountAsync();
    var targetUserCount = await targetContext.Users.CountAsync();
    Console.WriteLine($"Users: {sourceUserCount} (source) → {targetUserCount} (target) {(sourceUserCount == targetUserCount ? "✓" : "✗")}");

    var sourceAppCount = await sourceContext.Applications.CountAsync();
    var targetAppCount = await targetContext.Applications.CountAsync();
    Console.WriteLine($"Applications: {sourceAppCount} (source) → {targetAppCount} (target) {(sourceAppCount == targetAppCount ? "✓" : "✗")}");

    var sourceEventCount = await sourceContext.TimelineEvents.CountAsync();
    var targetEventCount = await targetContext.TimelineEvents.CountAsync();
    Console.WriteLine($"Timeline Events: {sourceEventCount} (source) → {targetEventCount} (target) {(sourceEventCount == targetEventCount ? "✓" : "✗")}");

    var sourceActivityCount = await sourceContext.ActivityEvents.CountAsync();
    var targetActivityCount = await targetContext.ActivityEvents.CountAsync();
    Console.WriteLine($"Activity Events: {sourceActivityCount} (source) → {targetActivityCount} (target) {(sourceActivityCount == targetActivityCount ? "✓" : "✗")}");

    var sourcePartyCount = await sourceContext.HuntingParties.CountAsync();
    var targetPartyCount = await targetContext.HuntingParties.CountAsync();
    Console.WriteLine($"Hunting Parties: {sourcePartyCount} (source) → {targetPartyCount} (target) {(sourcePartyCount == targetPartyCount ? "✓" : "✗")}");

    Console.WriteLine();
    Console.WriteLine("=== Migration Complete ===");
}
catch (Exception ex)
{
    Console.WriteLine();
    Console.WriteLine($"ERROR: {ex.Message}");
    Console.WriteLine(ex.StackTrace);
    return;
}
finally
{
    await sourceContext.DisposeAsync();
    await targetContext.DisposeAsync();
}
```

**Build the tool:**
```bash
cd src/Tools/DataMigration
dotnet build
# Should compile successfully
```

**✅ Checkpoint 3.6 Complete:** Migration tool created

---

### Step 3.7: Run Data Migration

**IMPORTANT:** Schedule this during low-traffic time (e.g., late night, weekend)

**Before running:**
1. Notify users of brief maintenance window
2. Make final backup of Azure SQL database
3. Double-check Supabase connection string

**Run migration:**
```bash
cd src/Tools/DataMigration
dotnet run

# Follow prompts:
# Enter Azure SQL Server connection string: [paste connection string]
# Enter Supabase PostgreSQL connection string: [paste connection string]

# Watch output for any errors
# Should see: "=== Migration Complete ===" with validation checkmarks
```

**If migration fails:**
1. Check error messages
2. Fix issues (connection, permissions, etc.)
3. Clear Supabase tables if needed:
   ```sql
   TRUNCATE TABLE "Applications" CASCADE;
   TRUNCATE TABLE "TimelineEvents" CASCADE;
   -- etc.
   ```
4. Run migration again

**✅ Checkpoint 3.7 Complete:** Data migrated to Supabase

---

### Step 3.8: Validate Migrated Data

**Validation Script:**

Run in Supabase SQL Editor:

```sql
-- Check record counts
SELECT 'Users' as Table_Name, COUNT(*) as Count FROM "AspNetUsers"
UNION ALL SELECT 'Applications', COUNT(*) FROM "Applications"
UNION ALL SELECT 'TimelineEvents', COUNT(*) FROM "TimelineEvents"
UNION ALL SELECT 'ActivityEvents', COUNT(*) FROM "ActivityEvents"
UNION ALL SELECT 'HuntingParties', COUNT(*) FROM "HuntingParties"
UNION ALL SELECT 'HuntingPartyMemberships', COUNT(*) FROM "HuntingPartyMemberships";
```

**Compare with baseline** from pre-implementation checklist.

**Sample data validation:**

```sql
-- Check user data
SELECT "Id", "Email", "DisplayName", "Points", "TotalPoints", "CurrentStreak"
FROM "AspNetUsers"
LIMIT 5;

-- Check application data
SELECT "Id", "UserId", "CompanyName", "RoleTitle", "Status", "Points"
FROM "Applications"
LIMIT 5;

-- Check timeline events
SELECT te."Id", te."ApplicationId", te."EventType", te."Points", te."Timestamp"
FROM "TimelineEvents" te
INNER JOIN "Applications" a ON a."Id" = te."ApplicationId"
LIMIT 10;

-- Check foreign key relationships
SELECT a."CompanyName", COUNT(te."Id") as EventCount
FROM "Applications" a
LEFT JOIN "TimelineEvents" te ON te."ApplicationId" = a."Id"
GROUP BY a."Id", a."CompanyName"
LIMIT 5;
```

**✅ All data looks correct?** If yes, proceed. If no, investigate discrepancies.

---

### Step 3.9: Test Application with Supabase

**Restore Supabase connection string:**
```bash
cd src/WebAPI

# Ensure Supabase connection string is set
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=db.YOUR_PROJECT_REF.supabase.co;Port=6543;Database=postgres;Username=postgres;Password=YOUR_PASSWORD;Pooling Mode=Session"
```

**Start application:**
```bash
cd src/WebAPI
dotnet run
```

**Test with existing user:**
```bash
POST http://localhost:5000/api/auth/login
Content-Type: application/json

{
  "email": "existing-user@example.com",
  "password": "TheirPassword123!"
}

# Expected: 200 OK with JWT token
```

**Verify data:**
```bash
GET http://localhost:5000/api/applications
Authorization: Bearer YOUR_TOKEN

# Expected: 200 OK with existing applications
```

**Test creating new data:**
```bash
POST http://localhost:5000/api/applications
Authorization: Bearer YOUR_TOKEN
Content-Type: application/json

{
  "sourceUrl": "https://example.com/final-test",
  "rawPageContent": "Final test job posting"
}

# Expected: 201 Created
```

**✅ Checkpoint 3.9 Complete:** Application works with Supabase + migrated data

---

### Step 3.10: Update Production Configuration

**For Azure App Service:**

1. Go to Azure Portal → Your App Service
2. Navigate to **Configuration** → **Application Settings**
3. Add/Update:
   - Name: `ConnectionStrings__DefaultConnection`
   - Value: `Host=db.YOUR_PROJECT_REF.supabase.co;Port=6543;Database=postgres;Username=postgres;Password=YOUR_PASSWORD;Pooling Mode=Session`
4. Click **Save**

**For other hosting (e.g., Docker, VM):**

Update environment variables:
```bash
export ConnectionStrings__DefaultConnection="Host=db.YOUR_PROJECT_REF.supabase.co;Port=6543;Database=postgres;Username=postgres;Password=YOUR_PASSWORD;Pooling Mode=Session"
```

---

### Step 3.11: Deploy to Production

**Option A: Azure App Service (Publish from VS Code/Visual Studio)**

1. Right-click WebAPI project → Publish
2. Follow wizard to publish to App Service
3. Monitor deployment logs
4. Verify deployment succeeded

**Option B: GitHub Actions / CI/CD**

1. Update connection string in GitHub Secrets
2. Trigger deployment pipeline
3. Monitor build and deployment
4. Verify deployment succeeded

**Option C: Manual Deployment**

```bash
# Build release
cd src/WebAPI
dotnet publish -c Release -o ./publish

# Deploy files to server (via FTP, SCP, etc.)
# Restart application
```

**✅ Checkpoint 3.11 Complete:** Application deployed to production

---

### Step 3.12: Production Smoke Tests

**Test 1: Health Check**
```bash
GET https://your-production-domain.com/api/health

# Expected: 200 OK (if health endpoint exists)
```

**Test 2: Login with Existing User**
```bash
POST https://your-production-domain.com/api/auth/login
Content-Type: application/json

{
  "email": "real-user@example.com",
  "password": "TheirActualPassword"
}

# Expected: 200 OK with JWT
```

**Test 3: Fetch User Data**
```bash
GET https://your-production-domain.com/api/applications
Authorization: Bearer PRODUCTION_TOKEN

# Expected: 200 OK with their applications
```

**Test 4: Create New Application**
```bash
POST https://your-production-domain.com/api/applications
Authorization: Bearer PRODUCTION_TOKEN
Content-Type: application/json

{
  "sourceUrl": "https://linkedin.com/jobs/12345",
  "rawPageContent": "Real job posting content"
}

# Expected: 201 Created
```

**Test 5: SignalR (if applicable)**
```bash
# Test real-time connections
# Open frontend and verify live updates work
```

**✅ All smoke tests passing?** If yes, migration successful! If no, check logs and troubleshoot.

---

### Step 3.13: Monitor Production

**For the next 24 hours, monitor:**

1. **Application Insights / Logs:**
   - Check for errors
   - Monitor exception rates
   - Verify request success rates

2. **Supabase Dashboard:**
   - Go to **Database** → **Monitoring**
   - Check connection count
   - Monitor query performance
   - Verify no unusual activity

3. **User Reports:**
   - Monitor support channels
   - Address any user-reported issues immediately

**Key metrics to watch:**
- Response times (should be similar or better than before)
- Error rates (should be near zero)
- Database connections (should be stable)
- Memory usage (should be normal)

---

### Step 3.14: Optimize and Tune

**After 24 hours of stable operation:**

#### Add Indexes for Slow Queries

Check slow queries in Supabase:
```sql
-- In Supabase SQL Editor
SELECT query, calls, mean_exec_time, max_exec_time
FROM pg_stat_statements
ORDER BY mean_exec_time DESC
LIMIT 10;
```

Add indexes if needed:
```sql
-- Example: If queries filtering by UserId + CreatedDate are slow
CREATE INDEX CONCURRENTLY idx_applications_userid_createddate
ON "Applications" ("UserId", "CreatedDate" DESC);

-- Example: If timeline event lookups are slow
CREATE INDEX CONCURRENTLY idx_timelineevents_applicationid_timestamp
ON "TimelineEvents" ("ApplicationId", "Timestamp" DESC);
```

#### Enable Connection Pooling (if not already)

Verify using connection pooler (Port 6543, not 5432).

#### Review Query Performance

Use Supabase's query analyzer to identify N+1 queries or missing indexes.

---

### Step 3.15: Decommission Azure SQL (Optional)

**WAIT AT LEAST 1 WEEK** before decommissioning Azure SQL Server.

**When ready:**

1. **Final Backup:**
   ```bash
   # Export Azure SQL to .bacpac file
   # Store securely for 30+ days
   ```

2. **Stop Azure SQL Database** (don't delete yet)
   - In Azure Portal → SQL Database → Overview → **Stop**

3. **Monitor for Issues:**
   - Wait another week
   - If no issues, proceed to deletion

4. **Delete Azure SQL Resources:**
   - Delete SQL Database
   - Delete SQL Server (if no other databases)
   - Remove connection strings from secrets/config

**✅ Checkpoint 3.15 Complete:** Azure SQL decommissioned (after 1+ week)

---

### Step 3.16: Final Documentation Update

**Update files:**

1. **README.md:**
   - Update getting started instructions for Supabase
   - Add Supabase setup section
   - Remove Azure SQL references

2. **CLAUDE.md:**
   - Update database connection instructions
   - Add Supabase connection examples
   - Update troubleshooting guide

3. **Meta/Docs/Project-Structure.md:**
   - Update database section to reflect Supabase
   - Document repository pattern
   - Update architecture diagrams

---

### Step 3.17: Commit Final Changes

```bash
cd C:\Users\Inferno\Dev\BigJobHunterPro

# Stage all changes
git add .

# Final commit
git commit -m "feat: Complete migration to Supabase PostgreSQL

- Create Supabase project and apply migrations
- Migrate production data from Azure SQL to Supabase
- Deploy application with Supabase connection
- Update production configuration
- Validate data integrity and application functionality

Phase 3 Complete: Production running on Supabase PostgreSQL
Status: Migration successful, all systems operational"

# Push to remote
git push origin feature/supabase-postgres-migration
```

**Create Pull Request:**

1. Go to GitHub repository
2. Create PR: `feature/supabase-postgres-migration` → `main`
3. Add description summarizing all 3 phases
4. Request review (if applicable)
5. Merge when approved

**Tag release:**
```bash
git checkout main
git pull origin main
git tag -a v2.0.0-supabase -m "Version 2.0: Migrated to Supabase PostgreSQL with repository pattern"
git push origin v2.0.0-supabase
```

---

### ✅ Phase 3 Complete Checklist

- [ ] Supabase project created
- [ ] Supabase connection string obtained
- [ ] Schema migrations applied to Supabase
- [ ] Row Level Security disabled
- [ ] Data exported from Azure SQL
- [ ] Data migration tool created
- [ ] Data migrated to Supabase
- [ ] Migrated data validated:
  - [ ] Record counts match
  - [ ] Sample data verified
  - [ ] Foreign keys intact
  - [ ] JSON fields correct
- [ ] Application tested with Supabase + migrated data
- [ ] Production configuration updated
- [ ] Application deployed to production
- [ ] Production smoke tests passed:
  - [ ] Health check
  - [ ] User login
  - [ ] Fetch data
  - [ ] Create new data
  - [ ] Real-time features
- [ ] Monitoring active (24 hours)
- [ ] Performance optimization complete
- [ ] Azure SQL decommission plan documented
- [ ] Documentation updated
- [ ] Changes committed and merged to main
- [ ] Release tagged

**✅ Phase 3 Sign-Off:**

Date: _______________
Production URL: _______________
Supabase Project: _______________
Status: ☐ Pass  ☐ Fail (if fail, rollback immediately)

Production Metrics (24 hours after deployment):
- Uptime: _______%
- Average Response Time: _______ms
- Error Rate: _______%
- Active Users: _______
- Database Connections: _______

Issues (if any):
_______________________________________________________
_______________________________________________________

---

## Emergency Rollback Procedures

### Rollback from Phase 3 (Production on Supabase)

**Scenario:** Critical issue discovered in production

**Immediate Steps (< 5 minutes):**

1. **Update connection string to Azure SQL:**
   ```bash
   # Azure App Service:
   # Settings → Configuration → Connection Strings
   # Update: ConnectionStrings__DefaultConnection
   # Value: [Azure SQL connection string]
   # Save → Restart
   ```

2. **Verify application is using Azure SQL:**
   ```bash
   # Test login endpoint
   # Check logs for SQL Server connections
   ```

3. **Notify team and users:**
   - Post status update
   - Estimate resolution time

**Post-Rollback (< 30 minutes):**

1. **Sync any new data** created on Supabase since migration:
   ```bash
   # Run data sync script to copy new records back to Azure SQL
   # Filter by timestamp > migration_time
   ```

2. **Investigate root cause:**
   - Review error logs
   - Check Supabase dashboard
   - Identify issue

3. **Fix and re-deploy:**
   - Fix issue
   - Test thoroughly
   - Re-attempt Phase 3

---

### Rollback from Phase 2 (Local PostgreSQL)

**Scenario:** Issues discovered during local testing

**Steps:**

1. **Restore SQL Server connection string:**
   ```bash
   cd src/WebAPI
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "[Azure SQL connection string]"
   ```

2. **Revert code changes:**
   ```bash
   git stash  # Save Phase 2 changes
   git checkout feature/supabase-postgres-migration~1  # Go to Phase 1 commit
   ```

3. **Rebuild and test:**
   ```bash
   dotnet build
   dotnet run
   # Test with SQL Server
   ```

4. **Fix issues, then resume Phase 2**

---

### Rollback from Phase 1 (Repository Pattern)

**Scenario:** Repository pattern causing unexpected issues

**Steps:**

1. **Revert to main branch:**
   ```bash
   git checkout main
   ```

2. **Rebuild:**
   ```bash
   dotnet build
   dotnet test
   dotnet run
   ```

3. **Investigate issues in Phase 1:**
   - Review repository implementations
   - Check for bugs in Unit of Work
   - Fix and re-attempt Phase 1

---

## Post-Migration Success Criteria

### Technical Criteria

- [ ] Application uptime > 99.9%
- [ ] Average response time < 500ms (or similar to pre-migration)
- [ ] Error rate < 0.1%
- [ ] All automated tests passing
- [ ] No data loss (100% record count match)
- [ ] Database connections stable (no connection pool exhaustion)

### Business Criteria

- [ ] All users can log in
- [ ] All features functional
- [ ] No user-reported critical bugs
- [ ] Performance acceptable to users
- [ ] Real-time features working (if applicable)

### Cost Criteria

- [ ] Hosting costs reduced (Supabase free tier vs Azure SQL)
- [ ] No unexpected charges
- [ ] Within budget projections

---

## Conclusion

This 3-phase implementation plan provides a methodical, low-risk approach to migrating from Azure MS SQL Server to Supabase PostgreSQL while refactoring the database layer using SOLID principles.

**Key Success Factors:**

1. **Incremental approach:** Each phase builds on the previous, with full testing
2. **Risk isolation:** Architectural changes separated from database provider change
3. **Comprehensive testing:** Multiple test checkpoints at each phase
4. **Rollback plans:** Clear procedures if issues arise
5. **Documentation:** Detailed instructions with verification steps

**Final Checklist:**

- [ ] Phase 1 Complete: Repository pattern implemented
- [ ] Phase 2 Complete: PostgreSQL running locally
- [ ] Phase 3 Complete: Production on Supabase
- [ ] All tests passing
- [ ] Production stable
- [ ] Team trained on new architecture
- [ ] Documentation updated

**Migration Complete!** 🎉

---

*Document Version: 1.0*
*Last Updated: 2026-01-16*
*Status: Ready for Implementation*
