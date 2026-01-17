# Supabase Migration - Quick Start Guide

**Branch:** `feature/supabase-postgres-migration`
**Full Plan:** See `supabase-migration-plan.md` for comprehensive details

---

## Overview

This guide provides a condensed implementation path for migrating from Azure MS SQL Server to Supabase PostgreSQL.

---

## Prerequisites

- [ ] .NET 8 SDK installed
- [ ] Docker Desktop installed
- [ ] Supabase account created
- [ ] Azure SQL Server backup completed
- [ ] Git branch created: `feature/supabase-postgres-migration`

---

## Phase 1: Database Layer Refactoring (Days 1-3)

### Step 1.1: Create Repository Interfaces

Create directory: `src/Application/Interfaces/Data/`

**Files to create:**
1. `IRepository.cs` - Generic CRUD interface
2. `IUnitOfWork.cs` - Transaction management interface
3. `IApplicationRepository.cs` - Application-specific queries
4. `ITimelineEventRepository.cs` - Timeline event queries
5. `IActivityEventRepository.cs` - Activity event queries
6. `IHuntingPartyRepository.cs` - Hunting party queries
7. `IHuntingPartyMembershipRepository.cs` - Membership queries

**Quick Implementation:** See Section 3 of full plan for complete code.

### Step 1.2: Implement Repositories

Create directory: `src/Infrastructure/Data/Repositories/`

**Files to create:**
1. `Repository.cs` - Generic base repository
2. `ApplicationRepository.cs` - Implements `IApplicationRepository`
3. `TimelineEventRepository.cs` - Implements `ITimelineEventRepository`
4. `ActivityEventRepository.cs` - Implements `IActivityEventRepository`
5. `HuntingPartyRepository.cs` - Implements `IHuntingPartyRepository`
6. `HuntingPartyMembershipRepository.cs` - Implements `IHuntingPartyMembershipRepository`

### Step 1.3: Implement Unit of Work

Create file: `src/Infrastructure/Data/UnitOfWork.cs`

This class coordinates all repositories and manages transactions.

### Step 1.4: Refactor Services

**Services to update:**
- `ApplicationService.cs`
- `TimelineEventService.cs`
- `ActivityEventService.cs`
- `HuntingPartyService.cs`
- `PointsService.cs`
- `StreakService.cs`
- `StatisticsService.cs`
- `AnalyticsService.cs`

**Pattern:**
```csharp
// OLD
private readonly ApplicationDbContext _context;

// NEW
private readonly IUnitOfWork _unitOfWork;
```

### Step 1.5: Register Services

Update `src/WebAPI/Program.cs`:

```csharp
// Add before AddControllers()
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
builder.Services.AddScoped<ITimelineEventRepository, TimelineEventRepository>();
builder.Services.AddScoped<IActivityEventRepository, ActivityEventRepository>();
builder.Services.AddScoped<IHuntingPartyRepository, HuntingPartyRepository>();
builder.Services.AddScoped<IHuntingPartyMembershipRepository, HuntingPartyMembershipRepository>();
```

---

## Phase 2: PostgreSQL Setup (Day 4)

### Step 2.1: Update Package References

Edit `src/Infrastructure/Infrastructure.csproj`:

**Remove:**
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.11" />
```

**Add:**
```xml
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.11" />
```

**Run:**
```bash
dotnet restore
```

### Step 2.2: Create Docker Compose File

Create `docker-compose.yml` in project root:

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

**Start PostgreSQL:**
```bash
docker-compose up -d postgres
```

### Step 2.3: Update Program.cs

Replace SQL Server configuration with PostgreSQL:

**Find (lines 21-38):**
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

**Replace with:**
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

### Step 2.4: Update Connection Strings

**Development (user secrets):**
```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=bigjobhunterpro_dev;Username=postgres;Password=localdev123" --project src/WebAPI
```

**Production (will use Supabase later):**
```bash
# For now, keep existing Azure connection string as fallback
```

### Step 2.5: Update SeedData.cs

Edit `src/Infrastructure/Data/SeedData.cs`:

**Find (lines 20-28):**
```csharp
if (context.Database.IsSqlite())
{
    await context.Database.EnsureCreatedAsync();
}
else
{
    await context.Database.MigrateAsync();
}
```

**Replace with:**
```csharp
if (context.Database.IsSqlite())
{
    await context.Database.EnsureCreatedAsync();
}
else
{
    // PostgreSQL and SQL Server both use migrations
    await context.Database.MigrateAsync();
}
```

### Step 2.6: Generate New Migrations

**Delete old migrations:**
```bash
# Backup first!
git add src/Infrastructure/Migrations/
git commit -m "backup: Save SQL Server migrations before deletion"

# Delete migration files (keep ApplicationDbContext.cs and SeedData.cs)
rm src/Infrastructure/Migrations/*.cs
```

**Generate new PostgreSQL migrations:**
```bash
dotnet ef migrations add InitialPostgres --project src/Infrastructure --startup-project src/WebAPI
```

**Apply migrations locally:**
```bash
dotnet ef database update --project src/Infrastructure --startup-project src/WebAPI
```

### Step 2.7: Test Locally

```bash
cd src/WebAPI
dotnet run
```

**Test endpoints:**
- POST `/api/auth/register` - Create test user
- POST `/api/auth/login` - Login
- GET `/api/applications` - Fetch applications
- POST `/api/applications` - Create application

---

## Phase 3: Supabase Setup (Day 5)

### Step 3.1: Create Supabase Project

1. Go to [supabase.com](https://supabase.com)
2. Click "New Project"
3. Set:
   - **Name:** `bigjobhunterpro-dev`
   - **Database Password:** (generate strong password, save it!)
   - **Region:** Choose closest to your users
4. Wait for provisioning (~2 minutes)

### Step 3.2: Get Connection String

1. Go to Project Settings → Database
2. Copy **Connection string** under "Connection Pooling" (Port 6543)
3. Replace `[YOUR-PASSWORD]` with the password you set

**Example:**
```
Host=db.abcdefghijk.supabase.co;Port=6543;Database=postgres;Username=postgres;Password=your-password;Pooling Mode=Session
```

### Step 3.3: Update User Secrets (Supabase)

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=db.abcdefghijk.supabase.co;Port=6543;Database=postgres;Username=postgres;Password=your-password;Pooling Mode=Session" --project src/WebAPI
```

### Step 3.4: Apply Migrations to Supabase

```bash
dotnet ef database update --project src/Infrastructure --startup-project src/WebAPI
```

**Verify in Supabase Dashboard:**
1. Go to Table Editor
2. You should see: `AspNetUsers`, `Applications`, `TimelineEvents`, etc.

### Step 3.5: Disable Row Level Security

Supabase enables RLS by default. Disable it for application tables:

**In Supabase SQL Editor:**
```sql
ALTER TABLE "AspNetUsers" DISABLE ROW LEVEL SECURITY;
ALTER TABLE "Applications" DISABLE ROW LEVEL SECURITY;
ALTER TABLE "TimelineEvents" DISABLE ROW LEVEL SECURITY;
ALTER TABLE "ActivityEvents" DISABLE ROW LEVEL SECURITY;
ALTER TABLE "HuntingParties" DISABLE ROW LEVEL SECURITY;
ALTER TABLE "HuntingPartyMemberships" DISABLE ROW LEVEL SECURITY;
ALTER TABLE "AspNetRoles" DISABLE ROW LEVEL SECURITY;
ALTER TABLE "AspNetUserRoles" DISABLE ROW LEVEL SECURITY;
ALTER TABLE "AspNetUserClaims" DISABLE ROW LEVEL SECURITY;
ALTER TABLE "AspNetUserLogins" DISABLE ROW LEVEL SECURITY;
ALTER TABLE "AspNetUserTokens" DISABLE ROW LEVEL SECURITY;
ALTER TABLE "AspNetRoleClaims" DISABLE ROW LEVEL SECURITY;
```

---

## Phase 4: Data Migration (Day 5-6)

### Option A: Manual Export/Import (Small Datasets)

If you have < 1000 records, use Azure Data Studio or SSMS to export to CSV, then import via Supabase SQL Editor.

### Option B: Automated Migration (Recommended)

Create migration tool: `src/Tools/DataMigration/`

**Files:**
1. `DataMigration.csproj`
2. `Program.cs` - Export from Azure SQL, Import to Supabase
3. `Transformer.cs` - Transform data types
4. `Validator.cs` - Validate migration

**Run:**
```bash
cd src/Tools/DataMigration
dotnet run
```

**See full plan (Section 6) for complete implementation.**

### Step 4.1: Validation Checklist

After migration, verify:

- [ ] User count matches (Azure SQL vs Supabase)
- [ ] Application count matches
- [ ] Timeline events count matches
- [ ] Sample application has correct timeline events
- [ ] Points and streaks calculated correctly
- [ ] JSON fields (RequiredSkills, NiceToHaveSkills) valid
- [ ] Timestamps in UTC

---

## Phase 5: Testing (Day 6)

### Step 5.1: Run Unit Tests

```bash
dotnet test
```

### Step 5.2: Manual Testing

- [ ] Register new user
- [ ] Login with JWT
- [ ] Create application
- [ ] Add timeline events
- [ ] Create hunting party
- [ ] Join hunting party
- [ ] View activity feed
- [ ] View leaderboard
- [ ] Test SignalR real-time updates

### Step 5.3: Performance Testing

**Query Performance:**
```bash
# Enable query logging (dev only)
dotnet run --environment Development
```

**Check for N+1 queries and optimize with Include().**

---

## Phase 6: Deployment (Day 7)

### Step 6.1: Update Production Configuration

**Set environment variables in Azure App Service:**

```
ConnectionStrings__DefaultConnection=Host=db.abcdefghijk.supabase.co;Port=6543;Database=postgres;Username=postgres;Password=PROD-PASSWORD;Pooling Mode=Session
```

### Step 6.2: Deploy

```bash
# Push branch
git add .
git commit -m "feat: Migrate to Supabase PostgreSQL with repository pattern"
git push origin feature/supabase-postgres-migration

# Create PR and merge after review
```

### Step 6.3: Monitor

- [ ] Check Application Insights for errors
- [ ] Monitor Supabase dashboard for query performance
- [ ] Verify connection pool usage
- [ ] Test all API endpoints in production

---

## Rollback Plan

**If critical issues occur:**

1. **Immediate rollback (< 5 minutes):**
   ```bash
   # Update connection string back to Azure SQL Server
   # Restart application
   ```

2. **Code rollback (< 30 minutes):**
   ```bash
   git revert feature/supabase-postgres-migration --no-commit
   git commit -m "Rollback to Azure SQL Server"
   git push origin main
   ```

3. **See full plan (Section 11) for detailed rollback procedure.**

---

## Useful Commands

**Entity Framework:**
```bash
# Add migration
dotnet ef migrations add MigrationName --project src/Infrastructure --startup-project src/WebAPI

# Update database
dotnet ef database update --project src/Infrastructure --startup-project src/WebAPI

# Generate SQL script
dotnet ef migrations script --project src/Infrastructure --startup-project src/WebAPI --output migration.sql
```

**Docker:**
```bash
# Start PostgreSQL
docker-compose up -d postgres

# View logs
docker-compose logs -f postgres

# Connect to psql
docker exec -it bigjobhunterpro-postgres psql -U postgres -d bigjobhunterpro_dev

# Stop and remove
docker-compose down -v
```

**Supabase:**
```bash
# Check table sizes
SELECT tablename, pg_size_pretty(pg_total_relation_size(tablename::text)) as size
FROM pg_tables WHERE schemaname = 'public';

# Active connections
SELECT count(*) FROM pg_stat_activity;
```

---

## Troubleshooting

### Issue: "Could not connect to PostgreSQL"
**Solution:** Check Docker container is running: `docker ps`

### Issue: "Migration already applied"
**Solution:** Delete migration: `dotnet ef migrations remove --project src/Infrastructure --startup-project src/WebAPI`

### Issue: "Npgsql package not found"
**Solution:** Run `dotnet restore` in Infrastructure project

### Issue: "Connection string format invalid"
**Solution:** Ensure format is: `Host=...;Port=...;Database=...;Username=...;Password=...`

### Issue: "JWT authentication fails"
**Solution:** Verify AspNetUsers table migrated correctly, regenerate JWT tokens

---

## Next Steps After Migration

1. **Optimize Performance:**
   - Add indexes for slow queries
   - Enable connection pooling
   - Use read replicas for analytics

2. **Leverage PostgreSQL Features:**
   - Convert JSON fields to `jsonb` type
   - Implement full-text search
   - Use PostgreSQL arrays for skill lists

3. **Explore Supabase Features:**
   - Real-time subscriptions (alternative to SignalR)
   - Supabase Auth (alternative to ASP.NET Identity)
   - Storage for file uploads

4. **Clean Up:**
   - Archive Azure SQL Server (optional)
   - Remove SQL Server packages
   - Update documentation

---

## Support

- **Full Plan:** `supabase-migration-plan.md`
- **PostgreSQL Docs:** [https://www.postgresql.org/docs/](https://www.postgresql.org/docs/)
- **Supabase Docs:** [https://supabase.com/docs](https://supabase.com/docs)
- **EF Core + PostgreSQL:** [https://www.npgsql.org/efcore/](https://www.npgsql.org/efcore/)

---

*Last Updated: 2026-01-16*
