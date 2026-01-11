# Production Incident - January 11, 2026

## Post-Mortem & Prevention Measures

### Executive Summary

On January 11, 2026, the production deployment failed with two critical issues:
1. **TypeScript Build Failure** - Duplicate type exports prevented frontend compilation
2. **Corrupted EF Core Migration** - Migration contained SQLite types instead of SQL Server, nearly causing catastrophic database corruption

Both issues were resolved, and comprehensive prevention measures have been implemented.

---

## Issue #1: Frontend Build Failure (TypeScript)

### What Happened
- **File:** `bigjobhunterpro-web/src/services/analytics.ts:18`
- **Error:** Duplicate export of types `KeywordFrequency` and `ConversionBySource`
- **Impact:** TypeScript compilation failed, causing Oryx to deploy source files instead of built bundles
- **User Impact:** Site loaded but showed MIME type errors, breaking JavaScript execution

### Root Cause
```typescript
// Lines 4-15: Already exported
export interface KeywordFrequency { ... }
export interface ConversionBySource { ... }

// Line 18: Redundant re-export
export type { KeywordFrequency, ConversionBySource }; // ❌ DUPLICATE
```

### Fix Applied
Removed line 18 (redundant export statement)

### Prevention Measures Implemented

#### 1. Pre-Build Validation (GitHub Actions)
Added explicit build step to frontend workflow that:
- Runs `npm ci` to install exact dependency versions
- Runs `npm run build` and fails if compilation errors occur
- Verifies `dist/index.html` exists
- Verifies `dist/staticwebapp.config.json` has MIME type mappings

**Workflow:** `.github/workflows/azure-static-web-apps-orange-cliff-0e76fcd10.yml`

#### 2. ESLint Configuration
Add to `.eslintrc.json`:
```json
{
  "rules": {
    "no-duplicate-imports": "error",
    "import/no-duplicates": "error"
  }
}
```

#### 3. Pre-Commit Hooks (Recommended)
Install Husky:
```bash
npm install --save-dev husky
npx husky init
```

Add `.husky/pre-commit`:
```bash
#!/bin/sh
cd bigjobhunterpro-web
npm run build || exit 1
```

---

## Issue #2: Corrupted Database Migration (CRITICAL)

### What Happened
- **File:** `src/Infrastructure/Migrations/20260110160814_AddStreakTracking.cs`
- **Error:** Migration generated with SQLite types instead of SQL Server types
- **Impact:** Migration would have converted entire database schema, **destroying all data**
- **Actual Damage:** Migration was blocked; manual SQL applied instead

### Root Cause

#### The Migration Was Generated Against SQLite
Developer created migration while local environment was configured for SQLite:

**Program.cs:18-37:**
```csharp
var useSqlite = builder.Environment.IsDevelopment() &&
    (string.IsNullOrEmpty(connectionString) ||
     connectionString.Contains("PLACEHOLDER") ||
     connectionString.Contains("USER SECRETS"));

if (useSqlite)
{
    options.UseSqlite($"Data Source={dbPath}");
}
else
{
    options.UseSqlServer(connectionString);
}
```

When no SQL Server connection string was configured locally, it fell back to SQLite. EF Core then generated a migration to "fix" all schema differences between SQLite and SQL Server models.

### What Would Have Happened (If Not Caught)

The migration would have executed:
```csharp
// Convert ALL GUIDs to TEXT (data loss)
migrationBuilder.AlterColumn<Guid>(
    name: "Id",
    table: "Applications",
    type: "TEXT",  // ❌ SQLite type
    oldType: "uniqueidentifier");  // Was SQL Server GUID

// Convert ALL datetimes to TEXT (query failures)
migrationBuilder.AlterColumn<DateTime>(
    name: "CreatedDate",
    table: "Applications",
    type: "TEXT",  // ❌ SQLite type
    oldType: "datetime2");  // Was SQL Server datetime

// ... 800+ more destructive changes
```

**Result:** Complete database corruption, all data lost, irreversible damage.

### Fix Applied
1. Manually executed SQL to add only the 4 intended columns:
   - `CurrentStreak` (int)
   - `LastActivityDate` (datetime2)
   - `LongestStreak` (int)
   - `StreakLastUpdated` (datetime2)

2. Inserted migration history record manually

3. Kept corrupt migration file as historical record (DO NOT apply it!)

### Prevention Measures Implemented

#### 1. Migration Validation (GitHub Actions)
Added validation step to backend workflow that scans all migrations for:
- SQLite type conversions (`TEXT`, `INTEGER` replacing `nvarchar`, `datetime2`, `uniqueidentifier`, `int`, `bit`)
- Excessive `AlterColumn` operations (>50 indicates full schema conversion)

**Workflow:** `.github/workflows/main_bjhp-api-prod.yml` (lines 32-67)

If dangerous patterns detected, **deployment is blocked**.

#### 2. Safe Migration Creation Script
**File:** `create-migration.ps1`

Usage:
```powershell
.\create-migration.ps1 -MigrationName "AddUserPreferences"
```

This script:
- Forces `ASPNETCORE_ENVIRONMENT=Production` to use SQL Server provider
- Checks that connection string is configured and not SQLite
- Creates migration with correct SQL Server types
- Reminds developer to review migration before applying

#### 3. Migration Review Checklist
Before committing any migration:

✅ **Review the migration file:**
- [ ] Does it only contain the changes you intended?
- [ ] Are all types SQL Server types (`int`, `nvarchar`, `datetime2`, `uniqueidentifier`, `bit`)?
- [ ] Are there NO SQLite types (`TEXT`, `INTEGER`, `REAL`)?
- [ ] Is the number of operations reasonable (<20 for typical changes)?

✅ **Test locally against SQL Server:**
```bash
dotnet ef database update --project src/Infrastructure --startup-project src/WebAPI
```

✅ **Run validation script:**
```powershell
.\validate-migration.ps1 -MigrationFile "src/Infrastructure/Migrations/YourMigration.cs"
```

#### 4. Environment Configuration Requirements

**For Local Development with SQL Server:**

Set connection string in `src/WebAPI/appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BigJobHunterProDev;Trusted_Connection=True;"
  }
}
```

Or use User Secrets:
```bash
cd src/WebAPI
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "YOUR_SQL_SERVER_CONNECTION_STRING"
```

**Never create migrations against SQLite if you're deploying to SQL Server!**

#### 5. Production Migration Process

**Manual Review Required:**
1. Developer creates migration using `create-migration.ps1`
2. Developer reviews migration file for correctness
3. Developer runs `validate-migration.ps1`
4. Code review includes migration inspection
5. CI/CD validates migration automatically
6. If validation passes, deployment proceeds
7. Migration runs automatically on app startup (future: change to manual apply)

**Emergency Manual Migration:**
If automated migration fails in production:
1. Generate SQL script: `dotnet ef migrations script --idempotent --output migration.sql`
2. Review SQL script manually
3. Apply via Azure Portal Query Editor or `exec-sql.js` script
4. Insert migration history record manually
5. Restart app service

---

## Lessons Learned

### What Went Wrong
1. **No build verification** - Frontend deployed source files when build failed
2. **Environment mismatch** - Migration created against wrong database provider
3. **No migration validation** - Dangerous migration nearly deployed
4. **No manual review** - Changes committed without inspection

### What Went Right
1. **Health checks** - Backend correctly reported degraded state (503)
2. **Detailed logging** - Health check endpoint revealed exact issue
3. **Safe deployment** - Migration didn't auto-apply on startup
4. **Quick recovery** - Manual fixes applied within minutes

### Process Improvements
1. ✅ **Automated validation** - CI/CD now blocks dangerous changes
2. ✅ **Developer tools** - Scripts to create and validate migrations safely
3. ✅ **Documentation** - This post-mortem and prevention guide
4. ✅ **Fail-fast** - Builds must succeed before deployment proceeds

---

## Action Items for Developers

### Before Creating a Migration
- [ ] Ensure SQL Server connection string is configured locally
- [ ] Verify `Program.cs` is using SQL Server (not SQLite)
- [ ] Use `create-migration.ps1` script instead of direct `dotnet ef` commands

### After Creating a Migration
- [ ] Review the migration file for correctness
- [ ] Run `validate-migration.ps1` to check for dangerous patterns
- [ ] Test migration against local SQL Server
- [ ] Commit migration only if validation passes

### Before Deploying
- [ ] Verify CI/CD build passes all validation steps
- [ ] Review migration in pull request
- [ ] Ensure health check passes after deployment

---

## Quick Reference Commands

### Frontend
```bash
# Build and validate
cd bigjobhunterpro-web
npm ci
npm run build
npm run lint

# Verify build output
test -f dist/index.html && echo "✓ Build succeeded" || echo "✗ Build failed"
```

### Backend
```bash
# Create migration (use script)
.\create-migration.ps1 -MigrationName "YourMigrationName"

# Validate migration
.\validate-migration.ps1 -MigrationFile "src/Infrastructure/Migrations/YourMigration.cs"

# Apply migration locally
dotnet ef database update --project src/Infrastructure --startup-project src/WebAPI

# Generate SQL script for manual review
dotnet ef migrations script --idempotent --output migration.sql --project src/Infrastructure --startup-project src/WebAPI
```

---

## Related Files
- Frontend Workflow: `.github/workflows/azure-static-web-apps-orange-cliff-0e76fcd10.yml`
- Backend Workflow: `.github/workflows/main_bjhp-api-prod.yml`
- Migration Creation: `create-migration.ps1`
- Migration Validation: `validate-migration.ps1`
- Emergency SQL Execution: `exec-sql.js`

---

**Date:** January 11, 2026
**Status:** Resolved
**Severity:** Critical (data loss risk)
**Impact:** 0 users affected (caught before user impact)
**Resolution Time:** ~2 hours
