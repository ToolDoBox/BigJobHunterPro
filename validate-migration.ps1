# validate-migration.ps1
# Validates that a migration doesn't contain dangerous SQLite conversions

param(
    [Parameter(Mandatory=$true)]
    [string]$MigrationFile
)

Write-Host "Validating migration: $MigrationFile" -ForegroundColor Cyan

if (-not (Test-Path $MigrationFile)) {
    Write-Host "ERROR: Migration file not found!" -ForegroundColor Red
    exit 1
}

$content = Get-Content $MigrationFile -Raw

# Check for dangerous SQLite type conversions
$dangerousPatterns = @(
    'type:\s*"TEXT".*oldType:\s*"nvarchar"',
    'type:\s*"TEXT".*oldType:\s*"datetime2"',
    'type:\s*"TEXT".*oldType:\s*"uniqueidentifier"',
    'type:\s*"INTEGER".*oldType:\s*"int"',
    'type:\s*"INTEGER".*oldType:\s*"bit"'
)

$foundIssues = @()

foreach ($pattern in $dangerousPatterns) {
    if ($content -match $pattern) {
        $foundIssues += "Found SQLite conversion pattern: $pattern"
    }
}

# Check for excessive AlterColumn operations (sign of full schema conversion)
$alterColumnCount = ([regex]::Matches($content, "AlterColumn")).Count
if ($alterColumnCount -gt 20) {
    $foundIssues += "WARNING: $alterColumnCount AlterColumn operations detected (expected < 20 for normal migrations)"
}

# Report results
if ($foundIssues.Count -gt 0) {
    Write-Host "`nMIGRATION VALIDATION FAILED!" -ForegroundColor Red
    Write-Host "This migration contains dangerous operations:`n" -ForegroundColor Red

    foreach ($issue in $foundIssues) {
        Write-Host "  - $issue" -ForegroundColor Yellow
    }

    Write-Host "`nThis migration will likely corrupt your database!" -ForegroundColor Red
    Write-Host "Review the migration file and regenerate it with the proper database provider." -ForegroundColor Yellow
    exit 1
} else {
    Write-Host "`nMigration validation passed." -ForegroundColor Green
    Write-Host "No dangerous SQLite conversions detected." -ForegroundColor Green
}
