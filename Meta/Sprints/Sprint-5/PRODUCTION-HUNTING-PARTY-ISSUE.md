# Production Hunting Party Activity Feed Issue - Comprehensive Report

**Date:** 2026-01-09
**Priority:** 🔴 HIGH - Production Outage
**Status:** ⚠️ Pending Migration in Production
**Branch:** `main` (fix deployed, migration pending)

---

## Executive Summary

The Hunting Party Activity Feed feature is **non-functional in production** due to a missing database table (`ActivityEvents`). While the code fix has been deployed to production, the required database migration has not been applied, causing the feature to fail with a 500 Internal Server Error.

**Impact:**
- ❌ All users attempting to access Hunting Party Activity Feed receive 500 errors
- ❌ SignalR real-time updates for activity cannot function
- ❌ Feature is completely broken in production since deployment

**Resolution Time:** ~5-10 minutes once Azure CLI access is established

---

## Problem Description

### User-Facing Symptoms
1. **500 Internal Server Error** when accessing Hunting Party page
2. **Empty Activity Feed** - no events display
3. **CORS errors** in browser console (secondary symptom)
4. **SignalR disconnections** due to failed API calls

### Technical Error
```
SQLite Error 1: 'no such table: ActivityEvents'
```
(Note: Error message references SQLite, but production uses SQL Server - the underlying issue is the same: missing table)

### Affected Endpoint
```
GET /api/parties/{partyId}/activity?limit=50
Status: 500 Internal Server Error
```

### Current Production Status
```json
{
  "status": "Degraded",
  "components": {
    "Database": "Healthy ✅",
    "JWT": "Healthy ✅",
    "Migrations": "Degraded ⚠️",
    "Configuration": "Healthy ✅"
  },
  "messages": [
    "Warning: 1 pending database migration(s)"
  ],
  "pendingMigrations": [
    "20260121090000_AddActivityEventsTable"
  ]
}
```

**API Health Endpoint:** https://bjhp-api-prod.azurewebsites.net/api/health/ready

---

## Root Cause Analysis

### Timeline of Events

1. **Migration Created** (2026-01-21 timestamp)
   - File: `src/Infrastructure/Migrations/20260121090000_AddActivityEventsTable.cs`
   - Creates `ActivityEvents` table with proper schema
   - Generated for SQL Server

2. **Code Deployed to Production**
   - Activity Feed feature code deployed
   - Migration file included in deployment
   - **Migration NOT auto-applied** (by design - manual application required)

3. **Runtime Failure**
   - Application tries to query `ActivityEvents` table
   - Table doesn't exist in production database
   - EF Core throws exception
   - 500 error returned to client

### Why This Happened

**Development vs Production Database Strategy:**
- **Development:** SQLite with `Database.EnsureCreatedAsync()` - auto-creates all tables
- **Production:** SQL Server with manual migrations - requires explicit migration application
- **Gap:** Production migrations must be applied separately after deployment

---

## What We've Fixed

### ✅ Local Development (COMPLETE)

**Changes Made:**
1. Modified `src/WebAPI/Program.cs` to use `Database.EnsureCreatedAsync()` for development
2. Added `.gitignore` entries for SQLite database files
3. Verified all 37 tests pass in Release configuration
4. Tested Activity Feed locally - confirmed working

**Commits:**
- `66da63c` - fix: Use EnsureCreatedAsync for SQLite development database
- `b7ed766` - docs: Add Activity Feed bug fix summary
- `236ebd5` - chore: Add SQLite database files to gitignore

**Local Test Results:**
```bash
✅ Build: SUCCESS
✅ Tests: 37/37 PASSED
✅ Activity Feed Endpoint: 200 OK
✅ Response: {"partyId":"...","events":[],"hasMore":false}
```

### ✅ Code Deployment (COMPLETE)

**Pushed to Main:** 2026-01-09
**GitHub Actions:** Auto-deployed to Azure App Service
**App Service Status:** Running with updated code

### ⚠️ Database Migration (PENDING)

**What's Missing:**
The `AddActivityEventsTable` migration needs to be applied to the production SQL Server database.

---

## Migration Details

### Migration File
**Location:** `src/Infrastructure/Migrations/20260121090000_AddActivityEventsTable.cs`

### What the Migration Creates

**Table: ActivityEvents**
```sql
CREATE TABLE "ActivityEvents" (
    "Id" uniqueidentifier NOT NULL PRIMARY KEY,
    "PartyId" uniqueidentifier NOT NULL,
    "UserId" nvarchar(450) NOT NULL,
    "EventType" int NOT NULL,
    "PointsDelta" int NOT NULL,
    "CreatedDate" datetime2 NOT NULL,
    "CompanyName" nvarchar(200) NULL,
    "RoleTitle" nvarchar(200) NULL,
    "MilestoneLabel" nvarchar(200) NULL,

    CONSTRAINT "FK_ActivityEvents_AspNetUsers_UserId"
        FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id")
        ON DELETE CASCADE,

    CONSTRAINT "FK_ActivityEvents_HuntingParties_PartyId"
        FOREIGN KEY ("PartyId") REFERENCES "HuntingParties" ("Id")
        ON DELETE CASCADE
);

CREATE INDEX "IX_ActivityEvents_CreatedDate"
    ON "ActivityEvents" ("CreatedDate");

CREATE INDEX "IX_ActivityEvents_PartyId"
    ON "ActivityEvents" ("PartyId");

CREATE INDEX "IX_ActivityEvents_UserId"
    ON "ActivityEvents" ("UserId");
```

### Migration Purpose
The `ActivityEvents` table stores all hunting party activity:
- Application logged events
- Milestone achievements (10, 25, 50, 100 apps)
- Interview completed events
- Offer received events
- Rejection logged events

---

## Solution - How to Fix Production

### Prerequisites
- ✅ Code deployed to production (DONE)
- ⚠️ Azure CLI installed (DONE)
- ⚠️ Azure CLI authenticated (PENDING - login failed)
- ⚠️ Access to Azure subscription (PENDING)

### Option 1: Apply Migration via Azure Portal (RECOMMENDED)

**Steps:**

1. **Navigate to Azure Portal**
   - URL: https://portal.azure.com
   - Resource: `bjhp-api-prod` (App Service)

2. **Open SSH/Kudu Console**
   - Go to: Development Tools → SSH or Advanced Tools (Kudu)
   - Open command prompt

3. **Navigate to Application Directory**
   ```bash
   cd /home/site/wwwroot
   ```

4. **Apply Migration**
   ```bash
   dotnet ef database update --no-build
   ```

   Or if EF tools not available:
   ```bash
   # The app should auto-apply on restart if SeedData.cs is configured
   # Just restart the app service
   ```

5. **Verify Migration Applied**
   ```bash
   # Check health endpoint
   curl https://bjhp-api-prod.azurewebsites.net/api/health/ready

   # Should show:
   # "Migrations": {"status": "Healthy", "pendingCount": 0}
   ```

### Option 2: Apply Migration via Azure CLI (ALTERNATE)

**Once Azure CLI authentication is working:**

```bash
# Set variables
RESOURCE_GROUP="BigJobHunterPro-Prod-RG"
APP_NAME="bjhp-api-prod"

# SSH into App Service
az webapp ssh --name $APP_NAME --resource-group $RESOURCE_GROUP

# Inside the container
cd /home/site/wwwroot
dotnet ef database update --no-build

# Exit SSH
exit

# Restart App Service
az webapp restart --name $APP_NAME --resource-group $RESOURCE_GROUP
```

### Option 3: Manual SQL Script (BACKUP OPTION)

**If migration tools fail:**

1. **Get Connection String**
   - Azure Portal → App Service → Configuration → Connection strings
   - Copy `DefaultConnection` value

2. **Connect to SQL Database**
   - Azure Portal → SQL Database → Query editor
   - Or use SQL Server Management Studio

3. **Run Migration SQL**
   ```sql
   -- Copy the exact SQL from migration file
   -- Located at: src/Infrastructure/Migrations/20260121090000_AddActivityEventsTable.cs
   -- Execute the CREATE TABLE and CREATE INDEX statements
   ```

4. **Update Migration History**
   ```sql
   INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
   VALUES ('20260121090000_AddActivityEventsTable', '8.0.22');
   ```

### Option 4: Enable Auto-Migration on Startup (FUTURE PREVENTION)

**Modify `src/Infrastructure/Data/SeedData.cs`:**

Currently:
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

This SHOULD already auto-migrate production on startup. Verify that `SeedData.InitializeAsync()` is being called in `Program.cs`.

---

## Verification Steps

### After Applying Migration

**1. Check Health Endpoint**
```bash
curl https://bjhp-api-prod.azurewebsites.net/api/health/ready | jq
```

Expected Response:
```json
{
  "status": "Healthy",
  "components": {
    "Migrations": {
      "status": "Healthy",
      "pendingCount": 0
    }
  }
}
```

**2. Test Activity Feed Endpoint**

**Important:** You need a valid JWT token from a logged-in user who is a member of a hunting party.

```bash
# Login first
TOKEN=$(curl -s -X POST https://bjhp-api-prod.azurewebsites.net/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"YOUR_EMAIL","password":"YOUR_PASSWORD"}' | jq -r '.token')

# Get user's party
PARTY_ID=$(curl -s https://bjhp-api-prod.azurewebsites.net/api/parties \
  -H "Authorization: Bearer $TOKEN" | jq -r '.id')

# Test activity feed
curl -s https://bjhp-api-prod.azurewebsites.net/api/parties/$PARTY_ID/activity?limit=50 \
  -H "Authorization: Bearer $TOKEN" | jq
```

Expected Response:
```json
{
  "partyId": "...",
  "events": [],
  "hasMore": false
}
```

**3. Test in Browser**

1. Navigate to: https://bigjobhunter.pro
2. Login with your credentials
3. Go to Hunting Party page
4. Open DevTools (F12) → Network tab
5. Look for: `GET /api/parties/{id}/activity?limit=50`
6. **Expected:** Status 200 OK
7. **Expected:** No errors in Console tab

---

## Troubleshooting

### If Migration Fails

**Error: "Migration already applied"**
- Check `__EFMigrationsHistory` table
- The migration might already be there
- Try restarting the App Service

**Error: "Cannot connect to database"**
- Check App Service → Configuration → Connection strings
- Verify `DefaultConnection` is set correctly
- Check SQL Database firewall rules

**Error: "dotnet ef not found"**
- EF tools not installed in production container
- Use Azure Portal SQL Query editor instead
- Run SQL script manually (Option 3)

### If Activity Feed Still Returns 500

**Check Application Insights:**
- Azure Portal → Application Insights → `bjhp-appinsights-prod`
- Go to: Failures → Exceptions
- Look for exceptions related to `ActivityEvents`

**Check App Service Logs:**
```bash
az webapp log tail --name bjhp-api-prod --resource-group BigJobHunterPro-Prod-RG
```

**Common Issues:**
- User not authenticated (401)
- User not member of party (403)
- Different error unrelated to table

---

## Prevention for Future Deployments

### Recommended Changes

**1. Add Migration Check to CI/CD**

Update `.github/workflows/main_bjhp-api-prod.yml`:
```yaml
- name: Check for pending migrations
  run: |
    dotnet ef migrations list --project src/Infrastructure --startup-project src/WebAPI
    # Alert if new migrations found
```

**2. Auto-Apply Migrations in Production**

Verify `Program.cs` calls migration in all environments:
```csharp
if (app.Environment.IsDevelopment())
{
    await Infrastructure.Data.SeedData.InitializeAsync(app.Services);
}
else
{
    // Also initialize for production (includes MigrateAsync)
    await Infrastructure.Data.SeedData.InitializeAsync(app.Services);
}
```

**3. Add Deployment Checklist**

Create `Meta/Docs/Deployment-Checklist.md`:
- [ ] All tests pass locally
- [ ] Migration files reviewed
- [ ] Database backup taken
- [ ] Migration applied to staging
- [ ] Migration applied to production
- [ ] Health check verified
- [ ] Feature tested in production

---

## Resource Information

### Azure Resources

**App Service:**
- Name: `bjhp-api-prod`
- Resource Group: `BigJobHunterPro-Prod-RG`
- URL: https://bjhp-api-prod.azurewebsites.net
- Region: (check portal)

**SQL Database:**
- Server: (check portal)
- Database: (check portal)
- Connection: Stored in App Service → Configuration → Connection strings

**Static Web App:**
- Frontend: https://bigjobhunter.pro
- Points to: bjhp-api-prod

### Key Files

**Migration:**
- `src/Infrastructure/Migrations/20260121090000_AddActivityEventsTable.cs`

**Database Context:**
- `src/Infrastructure/Data/ApplicationDbContext.cs`
- Includes `DbSet<ActivityEvent> ActivityEvents`

**Service Using Table:**
- `src/Infrastructure/Services/ActivityEventService.cs`
- Method: `GetPartyActivityAsync()`

**Controller:**
- `src/WebAPI/Controllers/HuntingPartiesController.cs`
- Endpoint: `GET /api/parties/{id}/activity`

---

## Communication Template

### For Users (If Needed)

```
🚧 Hunting Party Activity Feed Maintenance

We're aware that the Activity Feed in Hunting Parties is temporarily unavailable.
Our team has deployed a fix and is applying the final database update.

Expected Resolution: Within 1 hour
Impact: Activity Feed shows errors, other features work normally

Updates: We'll notify when the issue is resolved.
```

### For Stakeholders

```
Production Issue: Hunting Party Activity Feed (500 Error)
Status: Fix deployed, database migration pending
Root Cause: Missing ActivityEvents table in production database
Resolution: Apply pending migration (5-10 minutes)
Impact: Activity Feed feature non-functional
Timeline: Fixed locally, production resolution pending Azure access
```

---

## Next Steps (Action Items)

### Immediate (Required to Fix Production)

1. **⚠️ PRIORITY:** Establish Azure CLI authentication
   - Resolve login issues with Azure CLI
   - Ensure access to subscription `4f423755-8c18-43f7-afe2-0125a6f86faf`

2. **Apply Migration to Production**
   - Use one of the 4 options listed above
   - Recommended: Option 1 (Azure Portal) if CLI unavailable

3. **Restart App Service**
   ```bash
   az webapp restart --name bjhp-api-prod --resource-group BigJobHunterPro-Prod-RG
   ```

4. **Verify Fix**
   - Check health endpoint
   - Test activity feed endpoint
   - Test in browser

5. **Monitor**
   - Watch Application Insights for errors
   - Monitor App Service logs

### Follow-Up (Within 1 Week)

1. **Documentation**
   - Update deployment guide with migration steps
   - Create runbook for future migrations

2. **Process Improvement**
   - Add migration checks to CI/CD
   - Consider auto-migration for production
   - Create deployment checklist

3. **Testing**
   - Add integration test for activity feed
   - Test migration process in staging environment

---

## Contact Information

**Technical Owner:** Development Team
**Azure Subscription:** Azure subscription 1
**Support:** Application Insights, Azure Portal

---

## Appendix

### Full Migration File Reference

See: `src/Infrastructure/Migrations/20260121090000_AddActivityEventsTable.cs`

### Health Check API Response (Current)

```json
{
  "status": "Degraded",
  "timestampUtc": "2026-01-10T00:11:07.9777348Z",
  "components": {
    "Database": {
      "status": "Healthy",
      "description": "Database is connected and responsive",
      "duration": "00:00:00.0816625",
      "data": {
        "provider": "Microsoft.EntityFrameworkCore.SqlServer",
        "userCount": 5
      }
    },
    "JWT": {
      "status": "Healthy",
      "description": "JWT authentication is properly configured"
    },
    "Migrations": {
      "status": "Degraded",
      "description": "Database has pending migrations",
      "data": {
        "pendingCount": 1,
        "pendingMigrations": ["20260121090000_AddActivityEventsTable"]
      }
    },
    "Configuration": {
      "status": "Healthy",
      "description": "All critical configuration is present"
    }
  },
  "messages": ["Warning: 1 pending database migration(s)"],
  "duration": "00:00:00.1318339"
}
```

### Related Documentation

- `Meta/Sprints/Sprint-5/ACTIVITY-FEED-FIX-SUMMARY.md` - Local fix details
- `Meta/Sprints/Sprint-5/Health Fixes.md` - Previous troubleshooting
- `Meta/Docs/Project-Structure.md` - Architecture overview

---

**Report Generated:** 2026-01-09
**Status:** Ready for Migration Application
**Urgency:** HIGH - Production Feature Outage
