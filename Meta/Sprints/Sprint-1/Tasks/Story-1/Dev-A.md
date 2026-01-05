# cadleta Task List - Backend (Dev A)

**Story:** Authenticated App Shell (5 SP)
**Sprint:** 1 | Jan 4–17, 2026
**Tech Stack:** ASP.NET Core 8, EF Core, Identity, JWT
**Branch:** `feature/story1-backend-auth`

---

## PHASE 0: API CONTRACT (SYNC POINT - DO FIRST)

Before coding, confirm these contracts with realemmetts:

### Auth Endpoints (You Build These)
| Method | Endpoint | Request Body | Response |
|--------|----------|--------------|----------|
| POST | /api/auth/register | { email, password, displayName } | { userId, email, token } |
| POST | /api/auth/login | { email, password } | { userId, email, token, expiresAt } |
| POST | /api/auth/logout | (none, uses auth header) | { success: true } |
| GET | /api/auth/me | (none, uses auth header) | { userId, email, displayName, points } |

### JWT Claims
- `sub` (user ID)
- `email`
- `name` (display name)
- `exp` (expiration timestamp)

### Error Response Format
```json
{ "error": "string", "details": ["string"] }
```

---

## YOUR TASKS

### A0. Set Up Azure SQL Database ✅ COMPLETED
**Status:** ✅ Completed on 2026-01-04
- [x] Azure SQL Database (Free Offer) provisioned in Azure Portal
- [x] Created new logical SQL server: `bjhp-dev-sql-cgta`
- [x] Configured networking with firewall rule for local dev access (client public IP added)
- [x] Retrieved connection details for application configuration
- [x] **Note:** Connection string and credentials stored outside repository for security

**Azure Resources Created:**
- **Server:** `bjhp-dev-sql-cgta.database.windows.net`
- **Database:** BigJobHunterPro (Free/Basic tier)
- **Region:** [As configured in Azure Portal]
- **Firewall:** Local development IP whitelisted

---

### A1. Create Solution Structure ✅ COMPLETED
**Status:** ✅ Completed on 2026-01-04
**Directory:** `src/`
- [x] Create .NET 8 solution with Clean Architecture layers
- [x] Projects: Domain, Application, Infrastructure, WebAPI
- [x] Add project references (WebAPI -> Infrastructure -> Application -> Domain)
- [x] Add .gitignore for .NET

**Files to create:**
```
BigJobHunterPro.sln
src/
├── Domain/Domain.csproj
├── Application/Application.csproj
├── Infrastructure/Infrastructure.csproj
└── WebAPI/WebAPI.csproj
```

**Commands:**
```bash
dotnet new sln -n BigJobHunterPro
mkdir src
dotnet new classlib -o src/Domain -n Domain
dotnet new classlib -o src/Application -n Application
dotnet new classlib -o src/Infrastructure -n Infrastructure
dotnet new webapi -o src/WebAPI -n WebAPI
dotnet sln add src/Domain src/Application src/Infrastructure src/WebAPI
```

---

### A2. Configure ApplicationDbContext + Initial Migration
**Depends on:** A1
**Status:** In progress (uncommitted)
- [x] Install EF Core + SQL Server packages
- [x] Create ApplicationDbContext inheriting IdentityDbContext
- [x] Configure connection string (placeholders + user secrets guidance)
- [x] Create initial migration for Users table
- [ ] Add seed data helper for dev environment

**Notes:**
- Migrations exist in `src/Infrastructure/Migrations/` (tracked) and `src/Infrastructure/Data/Migrations/` (untracked). Pick one location and remove the duplicate.

**Files to create:**
- `src/Infrastructure/Data/ApplicationDbContext.cs`
- `src/Infrastructure/Data/Migrations/` (generated)
- `src/WebAPI/appsettings.json`
- `src/WebAPI/appsettings.Development.json`

**Packages to install:**
```bash
cd src/Infrastructure
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore

cd ../WebAPI
dotnet add package Microsoft.EntityFrameworkCore.Design
```

---

### A3. Configure ASP.NET Identity + Password Policy
**Depends on:** A2
**Status:** In progress (uncommitted)
- [x] Configure Identity services in Program.cs
- [x] Set password requirements (min 8 chars, require digit, require uppercase)
- [ ] Configure user lockout policy
- [x] Add ApplicationUser entity extending IdentityUser

**Files to create/modify:**
- `src/Domain/Entities/ApplicationUser.cs`
- `src/WebAPI/Program.cs` (Identity config)

**Password Policy:**
```csharp
options.Password.RequiredLength = 8;
options.Password.RequireDigit = true;
options.Password.RequireUppercase = true;
options.Password.RequireLowercase = true;
options.Password.RequireNonAlphanumeric = false;
```

---

### A4. Implement Register Endpoint
**Depends on:** A3
- [ ] Create RegisterRequest DTO with validation attributes
- [ ] Create RegisterResponse DTO
- [ ] Create AuthController with POST /api/auth/register
- [ ] Validate email uniqueness
- [ ] Hash password via Identity
- [ ] Return JWT token on successful registration

**Files to create:**
- `src/Application/DTOs/Auth/RegisterRequest.cs`
- `src/Application/DTOs/Auth/RegisterResponse.cs`
- `src/WebAPI/Controllers/AuthController.cs`

---

### A5. Implement Login Endpoint + JWT Issuance
**Depends on:** A4
- [ ] Create LoginRequest DTO
- [ ] Create LoginResponse DTO
- [ ] Add POST /api/auth/login to AuthController
- [ ] Validate credentials via Identity SignInManager
- [ ] Create JWT token with configured claims
- [ ] Set appropriate token expiration (e.g., 7 days)

**Files to create:**
- `src/Application/DTOs/Auth/LoginRequest.cs`
- `src/Application/DTOs/Auth/LoginResponse.cs`
- `src/Infrastructure/Services/JwtTokenService.cs`
- `src/Application/Interfaces/IJwtTokenService.cs`

**JWT Config (appsettings.json):**
```json
{
  "Jwt": {
    "Key": "your-256-bit-secret-key-here-make-it-long",
    "Issuer": "BigJobHunterPro",
    "Audience": "BigJobHunterPro",
    "ExpirationDays": 7
  }
}
```

---

### A6. Add Auth Middleware + User Claim Helpers
**Depends on:** A5
- [ ] Configure JWT Bearer authentication in Program.cs
- [ ] Add [Authorize] attribute support
- [ ] Create CurrentUserService to extract user ID from claims
- [ ] Create GET /api/auth/me endpoint (protected)

**Files to create:**
- `src/Application/Interfaces/ICurrentUserService.cs`
- `src/Infrastructure/Services/CurrentUserService.cs`

---

### A7. Create Health Check Endpoint
**Depends on:** A1
- [ ] Add GET /api/health endpoint (unauthenticated)
- [ ] Return { status: "healthy", timestamp: ISO8601 }
- [ ] Useful for frontend to verify API is running

**Response:**
```json
{ "status": "healthy", "timestamp": "2026-01-04T12:00:00Z" }
```

---

### A8. Write Integration Tests
**Depends on:** A6
- [ ] Create test project
- [ ] Test: Register with valid data returns 200 + token
- [ ] Test: Register with duplicate email returns 400
- [ ] Test: Login with valid credentials returns 200 + token
- [ ] Test: Login with invalid credentials returns 401
- [ ] Test: Protected endpoint without token returns 401
- [ ] Test: Protected endpoint with valid token returns 200

**Files to create:**
- `tests/WebAPI.IntegrationTests/WebAPI.IntegrationTests.csproj`
- `tests/WebAPI.IntegrationTests/AuthControllerTests.cs`

---

## INTEGRATION CHECKLIST (with realemmetts)

When realemmetts is ready to connect:
- [ ] Ensure API is running on known port (e.g., https://localhost:5001)
- [ ] Verify CORS is configured to allow frontend origin
- [ ] Test: Register creates user in database
- [ ] Test: Login returns valid JWT
- [ ] Test: /api/auth/me returns user info with valid token
- [ ] Test: Protected endpoints return 401 without token

---

## COMPLETION CRITERIA

- [ ] User can register via POST /api/auth/register
- [ ] User can login via POST /api/auth/login
- [ ] JWT token is returned on successful auth
- [ ] GET /api/auth/me returns user info when authenticated
- [ ] Protected endpoints return 401 when unauthenticated
- [ ] Integration tests pass

---

## GIT WORKFLOW

**Your Branch:** `feature/story1-backend-auth`

```bash
git checkout -b feature/story1-backend-auth
# ... do your work ...
git add .
git commit -m "feat: add auth endpoints with JWT"
git push -u origin feature/story1-backend-auth
```

**Commit Convention:** `feat:`, `fix:`, `refactor:`, `docs:`

**Merge Order:** You merge first (no conflicts expected), then realemmetts merges frontend.
