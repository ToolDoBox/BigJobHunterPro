# Supabase Migration Session Overview

## What we did
- Created Meta/Sprints/Sprint-9 and moved all Supabase migration docs from Meta/Docs into it.
- Implemented repository pattern and UnitOfWork across Application/Infrastructure and updated services to use it.
- Reworked EF Core migrations for PostgreSQL (Npgsql) and removed the earlier SQL Server migration set.
- Added and hardened the one-time data migration tool at src/Tools/DataMigration (UTC normalization, retries, per-entity saves).
- Exported an Azure SQL BACPAC backup (BigJobHunterProDev) to storage account bjhpdevsqlbackup01 in container sql-backups.
- Migrated Azure SQL BigJobHunterProDev data to Supabase PostgreSQL and validated record counts.
- Ran local build/test steps: dotnet restore/build/test, dotnet publish; frontend npm ci and npm run build; workflow validation scripts.
- Cut over production to Supabase by setting the Key Vault secret ConnectionStrings--DefaultConnection, then setting ConnectionStrings__DefaultConnection directly in App Service to the Supabase pooler connection string and restarting the app.
- Verified production health endpoints /api/health and /api/health/db returned 200 after the cutover.

## What still needs to be done
- Run production smoke tests that require a real user: login, fetch data, create data, and any SignalR checks.
- Decide whether to keep the App Service connection string direct or fix the Key Vault reference and switch back to Key Vault.
- Update Phase 3 status/sign-off in the migration docs with date, production URL, and owner sign-off.
- Monitor production for at least 24 hours and review Supabase DB metrics.
- Plan Azure SQL decommissioning after a stability window (recommend 1 week) and remove old Azure SQL secrets/connection strings when ready.
- Add RLS policies in Supabase when you are ready to re-enable them.
