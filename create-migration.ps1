# create-migration.ps1
# Safe migration creation script that enforces SQL Server context

param(
    [Parameter(Mandatory=$true)]
    [string]$MigrationName
)

Write-Host "Creating migration: $MigrationName" -ForegroundColor Cyan

# Ensure we're using SQL Server connection (not SQLite)
$env:ASPNETCORE_ENVIRONMENT = "Production"

# Check if connection string is configured
if (-not $env:ConnectionStrings__DefaultConnection) {
    Write-Host "ERROR: SQL Server connection string not configured!" -ForegroundColor Red
    Write-Host "Set environment variable or use --connection parameter" -ForegroundColor Yellow
    exit 1
}

# Verify it's not SQLite
if ($env:ConnectionStrings__DefaultConnection -like "*Data Source=*db") {
    Write-Host "ERROR: Cannot create migrations against SQLite database!" -ForegroundColor Red
    Write-Host "Configure a SQL Server connection string first." -ForegroundColor Yellow
    exit 1
}

Write-Host "Using SQL Server provider" -ForegroundColor Green

# Create migration
Set-Location src
dotnet ef migrations add $MigrationName `
    --project Infrastructure `
    --startup-project WebAPI `
    --verbose

if ($LASTEXITCODE -eq 0) {
    Write-Host "`n✓ Migration created successfully!" -ForegroundColor Green
    Write-Host "⚠ IMPORTANT: Review the generated migration before applying!" -ForegroundColor Yellow
    Write-Host "   Check that it only contains your intended changes." -ForegroundColor Yellow
} else {
    Write-Host "`n✗ Migration creation failed!" -ForegroundColor Red
    exit 1
}
