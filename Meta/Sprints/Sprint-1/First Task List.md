# First Task List - Story 1: Authenticated App Shell (5 SP)

Sprint: 1 | Start: 2026-01-04 | End: 2026-01-17

## Developer Assignment
- **Dev A (Backend)**: ASP.NET Core 8, EF Core, Identity, JWT auth
- **Dev B (Frontend)**: React 18, TypeScript, Vite, Tailwind CSS

---

## PHASE 0: API CONTRACT (SYNC POINT - DO FIRST)

Before either developer starts coding, agree on these shared contracts:

### Auth Endpoints
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
{ "error": string, "details"?: string[] }
```

---

## DEV A TASKS (BACKEND)

### A1. Create Solution Structure
**Directory:** `src/`
- [ ] Create .NET 8 solution with Clean Architecture layers
- [ ] Projects: Domain, Application, Infrastructure, WebAPI
- [ ] Add project references (WebAPI -> Infrastructure -> Application -> Domain)
- [ ] Add .gitignore for .NET

**Files to create:**
- `BigJobHunterPro.sln`
- `src/Domain/Domain.csproj`
- `src/Application/Application.csproj`
- `src/Infrastructure/Infrastructure.csproj`
- `src/WebAPI/WebAPI.csproj`

---

### A2. Configure ApplicationDbContext + Initial Migration
**Depends on:** A1
- [ ] Install EF Core + SQL Server packages
- [ ] Create ApplicationDbContext inheriting IdentityDbContext
- [ ] Configure connection string (use localdb or Docker SQL Server)
- [ ] Create initial migration for Users table
- [ ] Add seed data helper for dev environment

**Files to create:**
- `src/Infrastructure/Data/ApplicationDbContext.cs`
- `src/Infrastructure/Data/Migrations/` (generated)
- `src/WebAPI/appsettings.json`
- `src/WebAPI/appsettings.Development.json`

---

### A3. Configure ASP.NET Identity + Password Policy
**Depends on:** A2
- [ ] Configure Identity services in Program.cs
- [ ] Set password requirements (min 8 chars, require digit, require uppercase)
- [ ] Configure user lockout policy
- [ ] Add ApplicationUser entity extending IdentityUser

**Files to create/modify:**
- `src/Domain/Entities/ApplicationUser.cs`
- `src/WebAPI/Program.cs` (Identity config)

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

## DEV B TASKS (FRONTEND)

### B1. Scaffold React + Vite + TypeScript Project
**Directory:** `bigjobhunterpro-web/`
- [ ] Create Vite project with React + TypeScript template
- [ ] Install dependencies (react-router-dom, axios, etc.)
- [ ] Configure TypeScript strict mode
- [ ] Add .gitignore for Node

**Commands:**
```bash
npm create vite@latest bigjobhunterpro-web -- --template react-ts
cd bigjobhunterpro-web
npm install
```

---

### B2. Configure Tailwind CSS with Retro Theme
**Depends on:** B1
- [ ] Install Tailwind CSS + PostCSS + Autoprefixer
- [ ] Configure tailwind.config.js with theme colors:
  - Forest Green: #2E4600
  - Blaze Orange: #FF6700
  - CRT Amber: #FFB000
  - Terminal Green: #00FF00
  - Dark backgrounds, pixel-style accents
- [ ] Add "Press Start 2P" font for headers
- [ ] Add Inter font for body text

**Files to create:**
- `tailwind.config.js`
- `postcss.config.js`
- `src/index.css` (Tailwind directives + custom styles)

---

### B3. Build App Shell Layout with Nav Placeholders
**Depends on:** B2
- [ ] Create AppShell component with header, sidebar placeholder, main content area
- [ ] Add logo/title in header
- [ ] Add placeholder nav links (Dashboard, Applications, Party, Profile)
- [ ] Add user menu dropdown (placeholder)
- [ ] Style with retro-arcade theme

**Files to create:**
- `src/components/layout/AppShell.tsx`
- `src/components/layout/Header.tsx`
- `src/components/layout/NavLink.tsx`

---

### B4. Configure React Router with Protected Routes
**Depends on:** B3
- [ ] Install react-router-dom v6
- [ ] Create ProtectedRoute wrapper component
- [ ] Define routes:
  - /login (public)
  - /register (public)
  - /app/* (protected, uses AppShell)
  - /app/dashboard (placeholder)
- [ ] Redirect unauthenticated users to /login
- [ ] Redirect authenticated users away from /login

**Files to create:**
- `src/router/index.tsx`
- `src/router/ProtectedRoute.tsx`
- `src/pages/Dashboard.tsx` (placeholder)

---

### B5. Build Login Page with Validation
**Depends on:** B4
- [ ] Create Login page component
- [ ] Email input with validation (required, email format)
- [ ] Password input with validation (required, min 8 chars)
- [ ] Submit button with loading state
- [ ] Inline error messages (display within 100ms)
- [ ] Link to Register page
- [ ] Handle API errors gracefully

**Files to create:**
- `src/pages/Login.tsx`
- `src/components/forms/FormInput.tsx`
- `src/components/forms/FormError.tsx`

---

### B6. Build Register Page with Validation
**Depends on:** B5
- [ ] Create Register page component
- [ ] Display name input (required)
- [ ] Email input with validation
- [ ] Password input with validation
- [ ] Confirm password with match validation
- [ ] Submit button with loading state
- [ ] Link to Login page
- [ ] Auto-login after successful registration

**Files to create:**
- `src/pages/Register.tsx`

---

### B7. Implement API Client with Token Storage
**Depends on:** B1
- [ ] Create axios instance with base URL config
- [ ] Add request interceptor to attach JWT from storage
- [ ] Add response interceptor for 401 handling (redirect to login)
- [ ] Store token in localStorage (or secure alternative)
- [ ] Create auth service functions (login, register, logout, getMe)

**Files to create:**
- `src/services/api.ts`
- `src/services/auth.ts`
- `src/hooks/useAuth.ts`
- `src/context/AuthContext.tsx`

---

### B8. Implement Logout Flow
**Depends on:** B7
- [ ] Add logout button to AppShell header
- [ ] Clear token from storage on logout
- [ ] Redirect to /login after logout
- [ ] (Optional) Call /api/auth/logout to invalidate server-side

**Files to modify:**
- `src/components/layout/Header.tsx`
- `src/hooks/useAuth.ts`

---

## PHASE FINAL: INTEGRATION (SYNC POINT)

### Integration Checklist
- [ ] Dev B configures API base URL to Dev A's running backend
- [ ] Test: Register new user -> verify in database
- [ ] Test: Login with registered user -> receive valid JWT
- [ ] Test: Access protected route -> displays AppShell
- [ ] Test: Access protected route without token -> redirects to login
- [ ] Test: Logout -> clears token, redirects to login
- [ ] Test: Refresh page while logged in -> stays authenticated

---

## COMPLETION CRITERIA (from Sprint 1.md)

- [ ] User can register and log in via email/password
- [ ] Authenticated routes require a valid JWT; unauthenticated users are redirected to login
- [ ] Logout clears tokens and returns user to login
- [ ] API endpoints for applications reject unauthenticated requests with 401
- [ ] App shell layout renders with working navigation placeholders

---

## GIT WORKFLOW

### Branching Strategy
- Dev A: `feature/story1-backend-auth`
- Dev B: `feature/story1-frontend-auth`

### Merge Order
1. Dev A merges backend first (no conflicts expected)
2. Dev B merges frontend second (no conflicts expected)
3. Both verify integration on main branch

### Commit Convention
Use conventional commits: `feat:`, `fix:`, `refactor:`, `docs:`
