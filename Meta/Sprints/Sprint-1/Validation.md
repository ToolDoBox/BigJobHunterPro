# Sprint 1 Validation

This document captures the current Sprint 1 work status, demo steps, and validation commands.

## Current Work Summary

### Backend (Completed)
- Auth stack: Identity + JWT configuration and middleware in `src/WebAPI/Program.cs`
- Auth endpoints: register, login, get-me in `src/WebAPI/Controllers/AuthController.cs`
- Current user helper: `src/Infrastructure/Services/CurrentUserService.cs`
- JWT token service: `src/Infrastructure/Services/JwtTokenService.cs`
- EF Core Identity context and migrations: `src/Infrastructure/Data/ApplicationDbContext.cs` and `src/Infrastructure/Migrations/`
- Dev seed users: `src/Infrastructure/Data/SeedData.cs`
- Health endpoints: `src/WebAPI/Controllers/HealthController.cs`
- Integration tests scaffold: `tests/WebAPI.IntegrationTests/`

### Frontend (Not Started)
- `bigjobhunterpro-web/` directory not present yet (see `Meta/Sprints/Sprint-1/Tasks/Dev-B.md`)

## Gaps / Risks

- `/api/auth/logout` is in the agreed contract but not implemented.
- CORS is not configured in the API yet (frontend dev server will be blocked).
- Health endpoint response shape differs from the task spec (status key and timestamp key).
- Story status files still show "Not Started" despite backend progress.

## Demo Guide (Backend API)

### 1) Configure user secrets (required)
From `e:\Dev\BigJobHunterPro\src\WebAPI`:

```powershell
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=tcp:YOUR_SERVER.database.windows.net,1433;Initial Catalog=BigJobHunterPro;Persist Security Info=False;User ID=YOUR_USER;Password=YOUR_PASSWORD;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
dotnet user-secrets set "JwtSettings:Secret" "your-256-bit-secret-key-here-make-it-long-enough-for-HS256"
```

### 2) Run the API
```powershell
dotnet run --launch-profile https
```

Default local URLs (from `src/WebAPI/Properties/launchSettings.json`):
- Swagger: `https://localhost:7168/swagger`
- HTTP base: `http://localhost:5074`

### 3) Health checks
```powershell
$baseUrl = "https://localhost:7168"
Invoke-RestMethod -Method Get -Uri "$baseUrl/api/health"
Invoke-RestMethod -Method Get -Uri "$baseUrl/api/health/db"
```

### 4) Register a user
```powershell
$registerBody = @{
    email = "demo@test.com"
    password = "Demo1234"
    displayName = "Demo Hunter"
} | ConvertTo-Json

$register = Invoke-RestMethod -Method Post -Uri "$baseUrl/api/auth/register" -ContentType "application/json" -Body $registerBody
$register
```

### 5) Log in
```powershell
$loginBody = @{
    email = "demo@test.com"
    password = "Demo1234"
} | ConvertTo-Json

$login = Invoke-RestMethod -Method Post -Uri "$baseUrl/api/auth/login" -ContentType "application/json" -Body $loginBody
$token = $login.token
$login
```

Note: seeded dev users in `src/Infrastructure/Data/SeedData.cs` include:
- `hunter@test.com` / `Hunter123!`
- `newbie@test.com` / `Newbie123!`
- `pro@test.com` / `ProHunter123!`

### 6) Call protected endpoint
```powershell
Invoke-RestMethod -Method Get -Uri "$baseUrl/api/auth/me" -Headers @{ Authorization = "Bearer $token" }
```

### 7) Verify 401 without token
```powershell
Invoke-RestMethod -Method Get -Uri "$baseUrl/api/auth/me"
```

## Integration Tests (Optional)

```powershell
dotnet test tests/WebAPI.IntegrationTests/WebAPI.IntegrationTests.csproj
```

Known issue: `dotnet test` may fail due to deps resolution (see `tests/WebAPI.IntegrationTests/README.md`); IDE runners are the current workaround.
