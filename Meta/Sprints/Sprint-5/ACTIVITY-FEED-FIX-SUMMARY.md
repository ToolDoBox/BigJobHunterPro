# Activity Feed Bug Fix - Summary

**Date:** 2026-01-09
**Branch:** `fix/server-health`
**Status:** ✅ Fixed and Tested

## Problem

**Error:** `Invalid object name 'ActivityEvents'`
**Location:** Hunting Party Activity Feed endpoint (`GET /api/parties/{id}/activity`)
**Impact:** 500 Internal Server Error preventing the Activity Feed from loading

## Root Cause

The `ActivityEvents` table was missing from the development SQLite database because:

1. **Migration Incompatibility**: The EF Core migrations were generated for SQL Server (using `nvarchar(max)`, `nvarchar(450)`, etc.)
2. **SQLite Syntax Errors**: SQLite doesn't support SQL Server-specific data types, causing migration failures
3. **No Fallback**: The application wasn't creating the database schema when migrations failed

## Investigation Timeline

1. **✅ Identified Error**: Confirmed "Invalid object name 'ActivityEvents'" from provided error details
2. **✅ Located Migration**: Found `20260121090000_AddActivityEventsTable.cs` migration file
3. **✅ Attempted Auto-Migration**: Added `context.Database.MigrateAsync()` to Program.cs
4. **❌ Migration Failed**: SQL Server syntax incompatible with SQLite
5. **✅ Solution Found**: Use `Database.EnsureCreatedAsync()` for development SQLite

## Solution

Modified `src/WebAPI/Program.cs` (lines 277-282):

**Before:**
```csharp
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        // No database initialization code
        await Infrastructure.Data.SeedData.InitializeAsync(scope.ServiceProvider);
    }
}
```

**After:**
```csharp
if (app.Environment.IsDevelopment())
{
    // SeedData.InitializeAsync handles database creation internally
    await Infrastructure.Data.SeedData.InitializeAsync(app.Services);
}
```

**Key Change**: Let `SeedData.InitializeAsync()` handle database creation using `Database.EnsureCreatedAsync()` which creates the schema from the EF model without migrations (compatible with SQLite).

## Testing

### Test Steps:
1. Deleted existing SQLite database
2. Restarted backend server
3. Database created successfully with all tables including `ActivityEvents`
4. Created test user and hunting party
5. Tested activity feed endpoint

### Test Results:
```bash
GET http://localhost:5074/api/parties/{id}/activity?limit=50
Authorization: Bearer {token}

Response: 200 OK
{
  "partyId": "3caab763-cd5b-405c-bb0c-b1866b6c47f3",
  "events": [],
  "hasMore": false
}
```

✅ **Endpoint working successfully!**

## Files Modified

- `src/WebAPI/Program.cs` - Simplified database initialization for development

## Deployment Notes

### Development (SQLite)
- ✅ Fixed - Uses `Database.EnsureCreatedAsync()`
- Database auto-creates on server startup
- No migration issues

### Production (SQL Server)
- ⚠️ **Action Required**: The `AddActivityEventsTable` migration must be applied to production database
- Use Azure App Service deployment or run migration command:
  ```bash
  dotnet ef database update --project src/Infrastructure --startup-project src/WebAPI
  ```

## Migration Strategy

- **Development**: Use `Database.EnsureCreatedAsync()` (no migrations)
- **Production**: Use `Database.Migrate()` (proper migrations)
- This is handled in `SeedData.cs`:
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

## Next Steps for Production

To deploy this fix to production:

1. **Commit and Push**:
   ```bash
   git push origin fix/server-health
   ```

2. **Apply Migration to Production Database**:
   - Option A: Let the app auto-migrate on startup (if `MigrateAsync()` is in production startup code)
   - Option B: Manually run migration via Azure CLI or Portal
   - Option C: Run migration command during CI/CD deployment

3. **Verify in Production**:
   - Check that ActivityEvents table exists in production SQL Server
   - Test the activity feed endpoint
   - Monitor Application Insights for any errors

## Lessons Learned

1. **Dev/Prod Parity**: Development SQLite and production SQL Server require different approaches
2. **Migration Testing**: Always test migrations against the target database type
3. **Database Initialization**: Auto-creation strategies differ between development and production
4. **Error Visibility**: The temporary exception handler in `HuntingPartiesController.cs` (lines 214-228) could be removed once deployed

## Related Documentation

- Previous troubleshooting: `Meta/Sprints/Sprint-5/Health Fixes.md`
- Migration file: `src/Infrastructure/Migrations/20260121090000_AddActivityEventsTable.cs`
- Seed data logic: `src/Infrastructure/Data/SeedData.cs`

---

**Status**: ✅ Local Fix Complete - Ready for Production Deployment
