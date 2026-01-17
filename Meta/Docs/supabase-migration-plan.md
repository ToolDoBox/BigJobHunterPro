# Supabase PostgreSQL Migration Plan

**Project:** Big Job Hunter Pro
**Migration Type:** Azure MS SQL Server → Supabase PostgreSQL
**Branch:** `feature/supabase-postgres-migration`
**Author:** System Architecture Team
**Date:** 2026-01-16
**Status:** Planning Phase

---

## Table of Contents

1. [Executive Summary](#executive-summary)
2. [Current State Analysis](#current-state-analysis)
3. [Database Layer Refactoring Strategy](#database-layer-refactoring-strategy)
4. [Migration Architecture](#migration-architecture)
5. [Supabase-Specific Implementation](#supabase-specific-implementation)
6. [Data Migration Process](#data-migration-process)
7. [File Change Inventory](#file-change-inventory)
8. [Implementation Timeline](#implementation-timeline)
9. [Testing Strategy](#testing-strategy)
10. [Risk Mitigation](#risk-mitigation)
11. [Rollback Plan](#rollback-plan)
12. [Post-Migration Tasks](#post-migration-tasks)

---

## Executive Summary

### Migration Goals

1. **Migrate from Azure MS SQL Server to Supabase PostgreSQL** while maintaining 100% feature parity
2. **Refactor database layer** using SOLID principles and modern design patterns for better maintainability
3. **Implement abstraction layers** to enable easy database provider switching in the future
4. **Preserve all existing data** with zero data loss during migration
5. **Maintain backward compatibility** with existing API contracts and frontend applications

### Key Benefits

- **Cost Reduction:** Free Supabase tier for development and testing
- **Developer Experience:** Improved local development with PostgreSQL Docker containers
- **Scalability:** Better JSON handling with PostgreSQL's native JSONB support
- **Real-time Features:** Native Supabase real-time capabilities (potential future enhancement)
- **Database Independence:** Abstracted data access layer enables future provider changes

### High-Level Approach

```
Phase 1: Refactor → Phase 2: Adapt EF Core → Phase 3: Migrate Data → Phase 4: Test → Phase 5: Deploy
```

**Estimated Effort:** 5-7 days (1 developer)
**Risk Level:** Medium (well-planned with rollback strategy)

---

## Current State Analysis

### Existing Architecture

**ORM:** Entity Framework Core 8.0.11
**Database Provider:** SQL Server (with SQLite fallback for development)
**Architecture Pattern:** Clean Architecture (Domain → Application → Infrastructure → API)
**Data Access Pattern:** Direct DbContext injection into services (no repository pattern)

### Database Schema

#### Core Tables

| Table | Purpose | Record Count (Est.) | Migration Complexity |
|-------|---------|---------------------|----------------------|
| `AspNetUsers` | User accounts + gamification | Low-Medium | **Medium** (Identity framework) |
| `Applications` | Job applications | Medium-High | **High** (JSON columns, indexes) |
| `TimelineEvents` | Application milestones | High | **Medium** (FK dependencies) |
| `ActivityEvents` | Social feed events | High | **Medium** (FK dependencies) |
| `HuntingParties` | User groups | Low | **Low** |
| `HuntingPartyMemberships` | Party membership | Low-Medium | **Low** |
| `AspNet*` (6 tables) | Identity framework | Low | **Low** (standard schema) |

#### Key Entity Relationships

```
ApplicationUser (1) ──< (N) Applications
    │
    └──< (N) HuntingPartyMemberships ──> (N) HuntingParties
    │
    └──< (N) ActivityEvents

Application (1) ──< (N) TimelineEvents
```

### Current Data Access Patterns

**File:** `src/Infrastructure/Services/ApplicationService.cs`

```csharp
// Current pattern - Direct DbContext usage
public class ApplicationService : IApplicationService
{
    private readonly ApplicationDbContext _context;

    public async Task<ApplicationDto> GetByIdAsync(Guid id)
    {
        return await _context.Applications
            .Include(a => a.TimelineEvents)
            .FirstOrDefaultAsync(a => a.Id == id);
    }
}
```

**Problem:** Tight coupling to EF Core and specific database provider.

### Critical SQL Server Dependencies

1. **Data Types:**
   - `uniqueidentifier` → PostgreSQL `uuid`
   - `nvarchar(n)` → PostgreSQL `varchar(n)` or `text`
   - `nvarchar(max)` → PostgreSQL `text`
   - `datetime2` → PostgreSQL `timestamp without time zone`
   - `bit` → PostgreSQL `boolean`

2. **T-SQL Syntax in Migrations:**
   - `IF OBJECT_ID(N'[TableName]', N'U') IS NULL` (20260205000000_AddHuntingPartyTables.cs)
   - `CONSTRAINT [DF_...]` default constraint naming

3. **EF Core Provider Methods:**
   - `UseSqlServer()` in Program.cs:30
   - `EnableRetryOnFailure()` SQL Server-specific configuration

4. **Connection String Format:**
   - SQL Server: `Server=...;Database=...;User Id=...;Password=...`
   - PostgreSQL: `Host=...;Port=...;Database=...;Username=...;Password=...`

---

## Database Layer Refactoring Strategy

### Design Principles (SOLID)

#### 1. Single Responsibility Principle (SRP)
- **Current Violation:** Services handle business logic + data access
- **Solution:** Separate data access into repository layer

#### 2. Open/Closed Principle (OCP)
- **Current Violation:** Services depend on concrete DbContext
- **Solution:** Depend on abstractions (interfaces)

#### 3. Liskov Substitution Principle (LSP)
- **Solution:** Repository implementations can be swapped without affecting consumers

#### 4. Interface Segregation Principle (ISP)
- **Solution:** Define granular repository interfaces per aggregate root

#### 5. Dependency Inversion Principle (DIP)
- **Current Violation:** Services depend on Infrastructure (ApplicationDbContext)
- **Solution:** Invert dependency - Application layer defines interfaces, Infrastructure implements

### Proposed Architecture: Unit of Work + Repository Pattern

```
┌─────────────────────────────────────────────────────────────┐
│                      API Layer (WebAPI)                      │
│                 Controllers + Hubs                           │
└────────────────────────────┬────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────┐
│              Application Layer (Business Logic)              │
│  Services (ApplicationService, PointsService, etc.)          │
│  Interfaces: IUnitOfWork, IRepository<T>                     │
└────────────────────────────┬────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────┐
│           Infrastructure Layer (Data Access)                 │
│  UnitOfWork → IUnitOfWork                                    │
│  Repositories: ApplicationRepository, TimelineEventRepository│
│  DbContext: ApplicationDbContext (EF Core)                   │
└─────────────────────────────────────────────────────────────┘
                             │
                             ▼
                    ┌────────┴────────┐
                    │   PostgreSQL     │
                    │    (Supabase)    │
                    └─────────────────┘
```

### New Abstractions (Application Layer)

#### `Application/Interfaces/Data/IRepository.cs`

```csharp
namespace Application.Interfaces.Data;

/// <summary>
/// Generic repository interface for basic CRUD operations
/// </summary>
public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    void Update(TEntity entity);
    void Delete(TEntity entity);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
```

#### `Application/Interfaces/Data/IApplicationRepository.cs`

```csharp
namespace Application.Interfaces.Data;

/// <summary>
/// Specialized repository for Application entity with custom queries
/// </summary>
public interface IApplicationRepository : IRepository<Domain.Entities.Application>
{
    Task<IEnumerable<Domain.Entities.Application>> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Domain.Entities.Application>> GetWithTimelineEventsAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<Domain.Entities.Application?> GetByIdWithTimelineAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<int> CountByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Domain.Entities.Application>> GetByStatusAsync(
        string userId,
        ApplicationStatus status,
        CancellationToken cancellationToken = default);
}
```

#### `Application/Interfaces/Data/IUnitOfWork.cs`

```csharp
namespace Application.Interfaces.Data;

/// <summary>
/// Unit of Work pattern to manage transactions across repositories
/// Ensures data consistency and provides single commit point
/// </summary>
public interface IUnitOfWork : IDisposable
{
    // Repository properties
    IApplicationRepository Applications { get; }
    ITimelineEventRepository TimelineEvents { get; }
    IActivityEventRepository ActivityEvents { get; }
    IHuntingPartyRepository HuntingParties { get; }
    IHuntingPartyMembershipRepository HuntingPartyMemberships { get; }

    // Transaction management
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
```

### Implementation (Infrastructure Layer)

#### `Infrastructure/Data/Repositories/Repository.cs`

```csharp
namespace Infrastructure.Data.Repositories;

/// <summary>
/// Base generic repository implementation using EF Core
/// </summary>
public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
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
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    public virtual void Update(TEntity entity)
    {
        _dbSet.Update(entity);
    }

    public virtual void Delete(TEntity entity)
    {
        _dbSet.Remove(entity);
    }

    public virtual async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken) != null;
    }
}
```

#### `Infrastructure/Data/Repositories/ApplicationRepository.cs`

```csharp
namespace Infrastructure.Data.Repositories;

public class ApplicationRepository : Repository<Domain.Entities.Application>, IApplicationRepository
{
    public ApplicationRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Domain.Entities.Application>> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Application>> GetWithTimelineEventsAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.TimelineEvents.OrderByDescending(te => te.Timestamp))
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Domain.Entities.Application?> GetByIdWithTimelineAsync(
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
        return await _dbSet.CountAsync(a => a.UserId == userId, cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Application>> GetByStatusAsync(
        string userId,
        ApplicationStatus status,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(a => a.UserId == userId && a.Status == status)
            .OrderByDescending(a => a.CreatedDate)
            .ToListAsync(cancellationToken);
    }
}
```

#### `Infrastructure/Data/UnitOfWork.cs`

```csharp
namespace Infrastructure.Data;

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
        _context = context;
    }

    public IApplicationRepository Applications =>
        _applications ??= new ApplicationRepository(_context);

    public ITimelineEventRepository TimelineEvents =>
        _timelineEvents ??= new TimelineEventRepository(_context);

    public IActivityEventRepository ActivityEvents =>
        _activityEvents ??= new ActivityEventRepository(_context);

    public IHuntingPartyRepository HuntingParties =>
        _huntingParties ??= new HuntingPartyRepository(_context);

    public IHuntingPartyMembershipRepository HuntingPartyMemberships =>
        _huntingPartyMemberships ??= new HuntingPartyMembershipRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
```

### Service Refactoring Example

**Before (ApplicationService.cs):**

```csharp
public class ApplicationService : IApplicationService
{
    private readonly ApplicationDbContext _context; // ❌ Direct dependency

    public async Task<ApplicationDto> GetByIdAsync(Guid id)
    {
        var app = await _context.Applications // ❌ Direct query
            .Include(a => a.TimelineEvents)
            .FirstOrDefaultAsync(a => a.Id == id);
        // ...
    }
}
```

**After (ApplicationService.cs):**

```csharp
public class ApplicationService : IApplicationService
{
    private readonly IUnitOfWork _unitOfWork; // ✅ Abstraction dependency

    public async Task<ApplicationDto> GetByIdAsync(Guid id)
    {
        var app = await _unitOfWork.Applications // ✅ Through repository
            .GetByIdWithTimelineAsync(id);
        // ...
    }
}
```

**Benefits:**
- ✅ Easier to test (mock IUnitOfWork)
- ✅ Database-agnostic service layer
- ✅ Clear separation of concerns
- ✅ Transaction management centralized
- ✅ Future-proof for database changes

---

## Migration Architecture

### PostgreSQL-Specific Considerations

#### 1. Case Sensitivity
- **SQL Server:** Case-insensitive by default
- **PostgreSQL:** Case-sensitive for unquoted identifiers
- **Solution:** Use lowercase table/column names or quote identifiers

#### 2. Boolean Type
- **SQL Server:** `bit` (0/1)
- **PostgreSQL:** `boolean` (true/false)
- **Impact:** `IsActive`, `ParsedByAI`, `EmailConfirmed` columns

#### 3. UUID Type
- **SQL Server:** `uniqueidentifier`
- **PostgreSQL:** `uuid` with `gen_random_uuid()` function
- **Impact:** All `Id` primary keys

#### 4. JSON Support
- **SQL Server:** `nvarchar(max)` with manual serialization
- **PostgreSQL:** Native `json` or `jsonb` type
- **Opportunity:** Convert `RequiredSkills`, `NiceToHaveSkills` to `jsonb`

#### 5. Sequences vs Identity
- **SQL Server:** `IDENTITY(1,1)`
- **PostgreSQL:** `SERIAL` or `GENERATED ALWAYS AS IDENTITY`
- **Impact:** ASP.NET Identity tables (auto-increment columns)

#### 6. Index Syntax
- **SQL Server:** `CREATE INDEX [IX_Name] ON [Table] ([Column])`
- **PostgreSQL:** `CREATE INDEX ix_name ON "Table" ("Column")`

### EF Core Provider Migration

#### Package Changes (Infrastructure.csproj)

**Remove:**
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.11" />
```

**Add:**
```xml
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.11" />
```

**Keep (for local development fallback):**
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="8.0.11" />
```

#### Program.cs Changes

**Before:**
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (useSqlite)
    {
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

**After:**
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (useSqlite)
    {
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

---

## Supabase-Specific Implementation

### 1. Supabase Project Setup

#### Create Project (via Supabase Dashboard)

1. Go to [supabase.com](https://supabase.com)
2. Create new project: `bigjobhunterpro-dev`
3. Note down:
   - **Database Password** (set during creation)
   - **Project URL**: `https://[project-ref].supabase.co`
   - **Connection String**: Available in Settings → Database

#### Connection String Format

```
Host=db.[project-ref].supabase.co;
Port=5432;
Database=postgres;
Username=postgres;
Password=[your-password];
SSL Mode=Require;
Trust Server Certificate=true;
```

### 2. Supabase-Specific Features

#### Enable Row Level Security (RLS) - OPTIONAL

Supabase has built-in RLS support. For now, we'll **disable it** on application tables since authentication is handled by ASP.NET Core Identity.

```sql
-- Disable RLS on application tables (execute in Supabase SQL Editor)
ALTER TABLE "AspNetUsers" DISABLE ROW LEVEL SECURITY;
ALTER TABLE "Applications" DISABLE ROW LEVEL SECURITY;
ALTER TABLE "TimelineEvents" DISABLE ROW LEVEL SECURITY;
ALTER TABLE "ActivityEvents" DISABLE ROW LEVEL SECURITY;
ALTER TABLE "HuntingParties" DISABLE ROW LEVEL SECURITY;
ALTER TABLE "HuntingPartyMemberships" DISABLE ROW LEVEL SECURITY;
```

#### Configure Connection Pooler

Supabase provides PgBouncer for connection pooling. Use **Session Mode** for compatibility with EF Core migrations.

**Connection String (Pooler):**
```
Host=db.[project-ref].supabase.co;
Port=6543;
Database=postgres;
Username=postgres;
Password=[your-password];
Pooling Mode=Session;
```

### 3. Environment Configuration

#### appsettings.Development.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=bigjobhunterpro_dev;Username=postgres;Password=localdev123"
  },
  "DatabaseProvider": "PostgreSQL"
}
```

#### appsettings.Production.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "** LOADED FROM ENVIRONMENT VARIABLES **"
  },
  "DatabaseProvider": "PostgreSQL",
  "Supabase": {
    "Url": "https://[project-ref].supabase.co",
    "AnonKey": "[anon-key-if-needed]"
  }
}
```

### 4. Docker Compose for Local Development

**docker-compose.yml** (root directory)

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

**Usage:**
```bash
docker-compose up -d postgres  # Start PostgreSQL locally
docker-compose down -v          # Stop and remove volumes
```

---

## Data Migration Process

### Phase 1: Export from Azure SQL Server

#### Option A: Using SQL Server Management Studio (SSMS)

1. Connect to Azure SQL Server
2. Right-click database → Tasks → Generate Scripts
3. Select all tables → Advanced → Schema + Data
4. Save as `.sql` file

#### Option B: Using Azure Data Studio

1. Install [Azure Data Studio](https://docs.microsoft.com/en-us/sql/azure-data-studio/download-azure-data-studio)
2. Connect to Azure SQL Database
3. Use **SQL Server Import Extension** to export to CSV
4. Export each table separately

#### Option C: Using .NET Script (Recommended)

Create a console app: `src/Tools/DataMigration/DataMigration.csproj`

```csharp
// DataMigration/Program.cs
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

var sourceConnectionString = "YOUR_AZURE_SQL_CONNECTION_STRING";
var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
optionsBuilder.UseSqlServer(sourceConnectionString);

using var sourceContext = new ApplicationDbContext(optionsBuilder.Options);

// Export to JSON files
var users = await sourceContext.Users.ToListAsync();
await File.WriteAllTextAsync("export/users.json", JsonSerializer.Serialize(users));

var applications = await sourceContext.Applications
    .Include(a => a.TimelineEvents)
    .ToListAsync();
await File.WriteAllTextAsync("export/applications.json", JsonSerializer.Serialize(applications));

// Continue for all tables...
Console.WriteLine("Export completed!");
```

**Run:**
```bash
cd src/Tools/DataMigration
dotnet run
```

### Phase 2: Transform Data (SQL Server → PostgreSQL)

#### Data Type Transformations

Create transformation script: `src/Tools/DataMigration/Transformer.cs`

```csharp
public class DataTransformer
{
    public void TransformTimestamps(List<Application> applications)
    {
        // SQL Server datetime2 → PostgreSQL timestamp
        foreach (var app in applications)
        {
            app.CreatedDate = DateTime.SpecifyKind(app.CreatedDate, DateTimeKind.Utc);
            app.UpdatedDate = DateTime.SpecifyKind(app.UpdatedDate, DateTimeKind.Utc);
        }
    }

    public void TransformJsonFields(List<Application> applications)
    {
        // Validate JSON serialization for PostgreSQL jsonb
        foreach (var app in applications)
        {
            if (app.RequiredSkills == null) app.RequiredSkills = new List<string>();
            if (app.NiceToHaveSkills == null) app.NiceToHaveSkills = new List<string>();
        }
    }
}
```

### Phase 3: Import to Supabase PostgreSQL

#### Option A: Direct Import via EF Core (Recommended)

```csharp
// DataMigration/Program.cs (continued)

var targetConnectionString = "YOUR_SUPABASE_CONNECTION_STRING";
var targetOptionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
targetOptionsBuilder.UseNpgsql(targetConnectionString);

using var targetContext = new ApplicationDbContext(targetOptionsBuilder.Options);

// Apply migrations first
await targetContext.Database.MigrateAsync();

// Import users (Identity framework)
var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
foreach (var user in users)
{
    await userManager.CreateAsync(user);
}

// Import applications
targetContext.Applications.AddRange(applications);
await targetContext.SaveChangesAsync();

// Import timeline events
targetContext.TimelineEvents.AddRange(timelineEvents);
await targetContext.SaveChangesAsync();

Console.WriteLine("Import completed!");
```

#### Option B: Bulk Import via pgAdmin

1. Install [pgAdmin](https://www.pgadmin.org/download/)
2. Connect to Supabase PostgreSQL
3. Use **Import/Export** tool per table
4. Map CSV columns to PostgreSQL columns

### Phase 4: Data Validation

#### Validation Checklist

```csharp
// DataMigration/Validator.cs
public class DataValidator
{
    public async Task ValidateMigration(
        ApplicationDbContext sourceContext,
        ApplicationDbContext targetContext)
    {
        // Count validation
        var sourceUserCount = await sourceContext.Users.CountAsync();
        var targetUserCount = await targetContext.Users.CountAsync();
        Assert.Equal(sourceUserCount, targetUserCount);

        var sourceAppCount = await sourceContext.Applications.CountAsync();
        var targetAppCount = await targetContext.Applications.CountAsync();
        Assert.Equal(sourceAppCount, targetAppCount);

        // Sample data validation
        var sourceApp = await sourceContext.Applications
            .Include(a => a.TimelineEvents)
            .FirstAsync();
        var targetApp = await targetContext.Applications
            .Include(a => a.TimelineEvents)
            .FirstAsync(a => a.Id == sourceApp.Id);

        Assert.Equal(sourceApp.CompanyName, targetApp.CompanyName);
        Assert.Equal(sourceApp.TimelineEvents.Count, targetApp.TimelineEvents.Count);

        Console.WriteLine("✅ Migration validation passed!");
    }
}
```

---

## File Change Inventory

### Critical Changes (Must Modify)

#### 1. Infrastructure Layer

| File | Change Type | Description |
|------|-------------|-------------|
| `src/Infrastructure/Infrastructure.csproj` | **Modify** | Replace SqlServer package with Npgsql |
| `src/Infrastructure/Data/ApplicationDbContext.cs` | **Modify** | Update for PostgreSQL-specific configurations |
| `src/Infrastructure/Data/SeedData.cs` | **Modify** | Update IsSqlite() check to IsNpgsql() |
| `src/Infrastructure/Migrations/*.cs` | **Replace** | Generate new migrations for PostgreSQL |
| `src/Infrastructure/Data/Repositories/Repository.cs` | **Create** | Generic repository base class |
| `src/Infrastructure/Data/Repositories/ApplicationRepository.cs` | **Create** | Application-specific repository |
| `src/Infrastructure/Data/Repositories/TimelineEventRepository.cs` | **Create** | TimelineEvent repository |
| `src/Infrastructure/Data/Repositories/ActivityEventRepository.cs` | **Create** | ActivityEvent repository |
| `src/Infrastructure/Data/Repositories/HuntingPartyRepository.cs` | **Create** | HuntingParty repository |
| `src/Infrastructure/Data/Repositories/HuntingPartyMembershipRepository.cs` | **Create** | Membership repository |
| `src/Infrastructure/Data/UnitOfWork.cs` | **Create** | Unit of Work implementation |

#### 2. Application Layer

| File | Change Type | Description |
|------|-------------|-------------|
| `src/Application/Interfaces/Data/IRepository.cs` | **Create** | Generic repository interface |
| `src/Application/Interfaces/Data/IApplicationRepository.cs` | **Create** | Application repository interface |
| `src/Application/Interfaces/Data/ITimelineEventRepository.cs` | **Create** | TimelineEvent repository interface |
| `src/Application/Interfaces/Data/IActivityEventRepository.cs` | **Create** | ActivityEvent repository interface |
| `src/Application/Interfaces/Data/IHuntingPartyRepository.cs` | **Create** | HuntingParty repository interface |
| `src/Application/Interfaces/Data/IHuntingPartyMembershipRepository.cs` | **Create** | Membership repository interface |
| `src/Application/Interfaces/Data/IUnitOfWork.cs` | **Create** | Unit of Work interface |
| `src/Infrastructure/Services/ApplicationService.cs` | **Refactor** | Use IUnitOfWork instead of DbContext |
| `src/Infrastructure/Services/TimelineEventService.cs` | **Refactor** | Use IUnitOfWork |
| `src/Infrastructure/Services/ActivityEventService.cs` | **Refactor** | Use IUnitOfWork |
| `src/Infrastructure/Services/HuntingPartyService.cs` | **Refactor** | Use IUnitOfWork |
| `src/Infrastructure/Services/PointsService.cs` | **Refactor** | Use IUnitOfWork |
| `src/Infrastructure/Services/StreakService.cs` | **Refactor** | Use IUnitOfWork |
| `src/Infrastructure/Services/StatisticsService.cs` | **Refactor** | Use IUnitOfWork |
| `src/Infrastructure/Services/AnalyticsService.cs` | **Refactor** | Use IUnitOfWork |

#### 3. WebAPI Layer

| File | Change Type | Description |
|------|-------------|-------------|
| `src/WebAPI/Program.cs` | **Modify** | Replace UseSqlServer() with UseNpgsql() |
| `src/WebAPI/appsettings.Development.json` | **Modify** | Update connection string format |
| `src/WebAPI/appsettings.Production.json` | **Modify** | Update connection string format |

#### 4. Configuration Files

| File | Change Type | Description |
|------|-------------|-------------|
| `docker-compose.yml` | **Create** | PostgreSQL container for local dev |
| `.env.example` | **Create** | Example environment variables |

#### 5. Documentation

| File | Change Type | Description |
|------|-------------|-------------|
| `CLAUDE.md` | **Update** | Update database setup instructions |
| `README.md` | **Update** | Update getting started guide |
| `Meta/Docs/Project-Structure.md` | **Update** | Document new repository pattern |

### New Files to Create

```
src/
├── Application/
│   └── Interfaces/
│       └── Data/
│           ├── IRepository.cs                              [NEW]
│           ├── IUnitOfWork.cs                             [NEW]
│           ├── IApplicationRepository.cs                  [NEW]
│           ├── ITimelineEventRepository.cs                [NEW]
│           ├── IActivityEventRepository.cs                [NEW]
│           ├── IHuntingPartyRepository.cs                 [NEW]
│           └── IHuntingPartyMembershipRepository.cs       [NEW]
├── Infrastructure/
│   └── Data/
│       ├── Repositories/
│       │   ├── Repository.cs                              [NEW]
│       │   ├── ApplicationRepository.cs                   [NEW]
│       │   ├── TimelineEventRepository.cs                 [NEW]
│       │   ├── ActivityEventRepository.cs                 [NEW]
│       │   ├── HuntingPartyRepository.cs                  [NEW]
│       │   └── HuntingPartyMembershipRepository.cs        [NEW]
│       └── UnitOfWork.cs                                  [NEW]
├── Tools/
│   └── DataMigration/
│       ├── DataMigration.csproj                           [NEW]
│       ├── Program.cs                                     [NEW]
│       ├── Transformer.cs                                 [NEW]
│       └── Validator.cs                                   [NEW]
└── docker-compose.yml                                     [NEW]
```

---

## Implementation Timeline

### Sprint Breakdown (5-7 Days)

#### Day 1: Architecture Setup
- [ ] Create repository interfaces in Application layer
- [ ] Create IUnitOfWork interface
- [ ] Set up Supabase project
- [ ] Configure local PostgreSQL Docker container
- [ ] Update Infrastructure.csproj packages

#### Day 2: Repository Implementation
- [ ] Implement generic Repository<T> base class
- [ ] Implement ApplicationRepository
- [ ] Implement TimelineEventRepository
- [ ] Implement ActivityEventRepository
- [ ] Implement HuntingPartyRepository
- [ ] Implement HuntingPartyMembershipRepository
- [ ] Implement UnitOfWork

#### Day 3: Service Layer Refactoring
- [ ] Refactor ApplicationService to use IUnitOfWork
- [ ] Refactor TimelineEventService
- [ ] Refactor ActivityEventService
- [ ] Refactor HuntingPartyService
- [ ] Refactor PointsService
- [ ] Refactor StreakService
- [ ] Refactor StatisticsService
- [ ] Refactor AnalyticsService

#### Day 4: EF Core Migration + Testing
- [ ] Update Program.cs to UseNpgsql()
- [ ] Delete old SQL Server migrations
- [ ] Generate new PostgreSQL migrations (`dotnet ef migrations add InitialPostgres`)
- [ ] Update ApplicationDbContext for PostgreSQL specifics
- [ ] Test locally with Docker PostgreSQL
- [ ] Run all unit tests
- [ ] Run integration tests

#### Day 5: Data Migration
- [ ] Create DataMigration console app
- [ ] Export data from Azure SQL Server
- [ ] Transform data for PostgreSQL
- [ ] Import data to Supabase
- [ ] Validate data integrity
- [ ] Compare row counts and samples

#### Day 6: End-to-End Testing
- [ ] Test user registration and login
- [ ] Test application CRUD operations
- [ ] Test timeline event creation
- [ ] Test hunting party features
- [ ] Test activity feed
- [ ] Test points and streak calculations
- [ ] Performance testing

#### Day 7: Deployment + Documentation
- [ ] Update production configuration
- [ ] Deploy to Supabase production instance
- [ ] Update environment variables
- [ ] Monitor application logs
- [ ] Update CLAUDE.md
- [ ] Update README.md
- [ ] Document rollback procedure
- [ ] Create post-migration runbook

---

## Testing Strategy

### Unit Testing

**Test Repository Pattern:**

```csharp
// ApplicationRepository.Tests.cs
public class ApplicationRepositoryTests
{
    [Fact]
    public async Task GetByUserIdAsync_ReturnsUserApplications()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        using var context = new ApplicationDbContext(options);
        var repository = new ApplicationRepository(context);

        // Act
        var results = await repository.GetByUserIdAsync("user-123");

        // Assert
        Assert.NotEmpty(results);
    }
}
```

### Integration Testing

**Test with TestContainers:**

```csharp
// ApplicationService.IntegrationTests.cs
public class ApplicationServiceIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .Build();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
    }

    [Fact]
    public async Task CreateApplication_InsertsToDatabase()
    {
        // Arrange
        var connectionString = _postgres.GetConnectionString();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        using var context = new ApplicationDbContext(options);
        await context.Database.MigrateAsync();

        var unitOfWork = new UnitOfWork(context);
        var service = new ApplicationService(unitOfWork, ...);

        // Act
        var result = await service.CreateApplicationAsync(...);

        // Assert
        var saved = await unitOfWork.Applications.GetByIdAsync(result.Id);
        Assert.NotNull(saved);
    }

    public async Task DisposeAsync()
    {
        await _postgres.DisposeAsync();
    }
}
```

### Manual Testing Checklist

- [ ] Create new user account
- [ ] Log in with JWT token
- [ ] Create job application
- [ ] Add timeline event (screening, interview, offer, rejection)
- [ ] Verify points calculation
- [ ] Verify streak updates
- [ ] Create hunting party
- [ ] Join hunting party with invite code
- [ ] View activity feed
- [ ] View leaderboard
- [ ] Test SignalR real-time updates
- [ ] Import Job-Hunt-Context JSON file
- [ ] Export user data
- [ ] Test pagination and filtering

---

## Risk Mitigation

### Identified Risks

| Risk | Severity | Mitigation Strategy |
|------|----------|---------------------|
| **Data Loss During Migration** | Critical | Multi-stage validation, backup Azure DB before export |
| **Performance Degradation** | High | Load testing, index optimization, connection pooling |
| **Breaking API Changes** | High | Maintain API contracts, version endpoints if needed |
| **Authentication Issues** | High | Test Identity framework thoroughly with PostgreSQL |
| **Migration Downtime** | Medium | Schedule during low-traffic window, blue-green deployment |
| **Timezone Handling** | Medium | Use UTC consistently, test timestamp conversions |
| **JSON Serialization** | Medium | Validate JSON fields, test with edge cases |
| **Connection String Leaks** | High | Use environment variables, never commit secrets |

### Contingency Plans

#### If Data Migration Fails
1. Rollback to Azure SQL Server (see Rollback Plan)
2. Investigate data transformation issues
3. Re-run validation scripts
4. Attempt incremental import (table by table)

#### If Performance Issues Occur
1. Enable query logging: `options.EnableSensitiveDataLogging()` (dev only)
2. Analyze slow queries in Supabase dashboard
3. Add missing indexes
4. Enable connection pooling (PgBouncer)
5. Consider read replicas for analytics queries

#### If Authentication Breaks
1. Verify ASP.NET Identity tables migrated correctly
2. Test password hashing compatibility
3. Regenerate JWT tokens
4. Clear client-side token cache

---

## Rollback Plan

### Scenario: Critical Issue Discovered Post-Deployment

#### Step 1: Immediate Rollback (< 5 minutes)

**Revert Connection String:**

```bash
# Update environment variable to point back to Azure SQL Server
export ConnectionStrings__DefaultConnection="Server=tcp:bigjobhunterpro.database.windows.net..."

# Restart application
dotnet run --project src/WebAPI
```

**Or via Azure App Service Configuration:**

1. Go to Azure Portal → App Service → Configuration
2. Update `ConnectionStrings:DefaultConnection` to Azure SQL Server
3. Save → Restart

#### Step 2: Code Rollback (< 30 minutes)

```bash
# Revert to previous commit (before migration)
git revert feature/supabase-postgres-migration --no-commit
git commit -m "Rollback to Azure SQL Server"
git push origin main

# Redeploy previous version
# (Deployment pipeline will automatically use Azure SQL Server connection string)
```

#### Step 3: Data Reconciliation

If any data was written to Supabase PostgreSQL during the migration window:

1. Export new data from Supabase
2. Import into Azure SQL Server
3. Merge using timestamps (keep latest changes)

**Merge Script:**

```csharp
// Merge new Supabase data back into Azure SQL
var supabaseApps = await supabaseContext.Applications
    .Where(a => a.CreatedDate > migrationStartTime)
    .ToListAsync();

azureContext.Applications.AddRange(supabaseApps);
await azureContext.SaveChangesAsync();
```

### Rollback Testing

**Before Migration:**
- [ ] Test rollback procedure in staging environment
- [ ] Document exact rollback steps
- [ ] Prepare rollback scripts
- [ ] Assign rollback responsibility (who can execute)

---

## Post-Migration Tasks

### Immediate (Day 1)

- [ ] Monitor application logs for errors
- [ ] Check error rates in Application Insights
- [ ] Monitor database connection pool usage
- [ ] Validate all API endpoints return 200 OK
- [ ] Test user login/registration
- [ ] Verify SignalR hubs connect successfully

### Short-Term (Week 1)

- [ ] Optimize slow queries identified in monitoring
- [ ] Add missing indexes based on query patterns
- [ ] Update developer documentation
- [ ] Train team on new repository pattern
- [ ] Archive old SQL Server migrations
- [ ] Clean up Azure SQL Server resources (optional)

### Long-Term (Month 1)

- [ ] Evaluate cost savings (Azure SQL vs Supabase)
- [ ] Consider Supabase real-time features (subscriptions)
- [ ] Migrate to PostgreSQL JSONB for skill columns
- [ ] Implement full-text search (PostgreSQL native)
- [ ] Set up automated backups (Supabase PITR)
- [ ] Performance benchmarking report

---

## Appendix A: Connection String Examples

### Development (Local Docker PostgreSQL)

```
Host=localhost;Port=5432;Database=bigjobhunterpro_dev;Username=postgres;Password=localdev123
```

### Supabase (Production)

```
Host=db.abcdefghijk.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=your-secure-password;SSL Mode=Require;Trust Server Certificate=true
```

### Supabase (Connection Pooler - Recommended for EF Core)

```
Host=db.abcdefghijk.supabase.co;Port=6543;Database=postgres;Username=postgres;Password=your-secure-password;Pooling Mode=Session
```

---

## Appendix B: SQL Server to PostgreSQL Data Type Mapping

| SQL Server | PostgreSQL | EF Core Type | Notes |
|------------|------------|--------------|-------|
| `uniqueidentifier` | `uuid` | `Guid` | Use `gen_random_uuid()` for defaults |
| `nvarchar(n)` | `varchar(n)` | `string` | PostgreSQL uses UTF-8 by default |
| `nvarchar(max)` | `text` | `string` | No length limit |
| `datetime2` | `timestamp without time zone` | `DateTime` | Always use UTC |
| `datetimeoffset` | `timestamp with time zone` | `DateTimeOffset` | Includes timezone |
| `bit` | `boolean` | `bool` | true/false vs 0/1 |
| `int` | `integer` | `int` | 32-bit signed |
| `bigint` | `bigint` | `long` | 64-bit signed |
| `decimal(p,s)` | `numeric(p,s)` | `decimal` | Arbitrary precision |
| `float` | `double precision` | `double` | 64-bit floating point |
| `varbinary(max)` | `bytea` | `byte[]` | Binary data |

---

## Appendix C: Useful Commands

### Entity Framework Core

```bash
# Generate new migration for PostgreSQL
dotnet ef migrations add InitialPostgres --project src/Infrastructure --startup-project src/WebAPI

# Apply migrations
dotnet ef database update --project src/Infrastructure --startup-project src/WebAPI

# Remove last migration (if not applied)
dotnet ef migrations remove --project src/Infrastructure --startup-project src/WebAPI

# Generate SQL script from migrations
dotnet ef migrations script --project src/Infrastructure --startup-project src/WebAPI --output migration.sql
```

### Docker

```bash
# Start PostgreSQL container
docker-compose up -d postgres

# View logs
docker-compose logs -f postgres

# Connect to PostgreSQL CLI
docker exec -it bigjobhunterpro-postgres psql -U postgres -d bigjobhunterpro_dev

# Stop and remove container + volumes
docker-compose down -v
```

### Supabase CLI (Optional)

```bash
# Install Supabase CLI
npm install -g supabase

# Link to project
supabase link --project-ref [your-project-ref]

# Pull database schema
supabase db pull

# Generate TypeScript types (for frontend)
supabase gen types typescript --linked > types/supabase.ts
```

---

## Appendix D: Monitoring Queries

### Check Table Sizes (PostgreSQL)

```sql
SELECT
    schemaname,
    tablename,
    pg_size_pretty(pg_total_relation_size(schemaname||'.'||tablename)) AS size
FROM pg_tables
WHERE schemaname NOT IN ('pg_catalog', 'information_schema')
ORDER BY pg_total_relation_size(schemaname||'.'||tablename) DESC;
```

### Active Connections

```sql
SELECT
    datname,
    count(*) as connections
FROM pg_stat_activity
GROUP BY datname;
```

### Slow Queries

```sql
SELECT
    query,
    calls,
    mean_exec_time,
    max_exec_time
FROM pg_stat_statements
ORDER BY mean_exec_time DESC
LIMIT 10;
```

---

## Sign-Off

**Plan Reviewed By:**

- [ ] Backend Lead: ________________ Date: ________
- [ ] DevOps Engineer: _____________ Date: ________
- [ ] QA Lead: _____________________ Date: ________
- [ ] Product Owner: _______________ Date: ________

**Approved for Implementation:** [ ] Yes  [ ] No

**Scheduled Migration Date:** _________________

**Rollback Decision Maker:** _________________

---

*End of Migration Plan*
