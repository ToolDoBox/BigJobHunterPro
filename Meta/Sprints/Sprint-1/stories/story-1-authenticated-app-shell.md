# Story 1: Authenticated App Shell

## User Story

**As a** job hunter
**I want** to register and log into the app
**so that** I can access my personal job tracking dashboard and keep my data secure

- **Story Points:** 5
- **Priority:** High
- **Sprint:** Sprint 1
- **Assignee(s):** cadleta (Backend) + realemmetts (Frontend)
- **Status:** ⬜ Not Started

**Status Legend:** ⬜ Not Started | 🔄 In Progress | ✅ Completed | 🚫 Blocked | 📦 Moved to Next Sprint

---

## Desired Outcome

After completing this story, users should be able to register a new account with email/password, log in securely with JWT authentication, access a protected app shell with navigation placeholders, and log out. Unauthenticated users should be automatically redirected to the login page when trying to access protected routes.

---

## Acceptance Criteria (Definition of Done)

### Functional Requirements
- [ ] User can register and log in via email/password
- [ ] Authenticated routes require a valid JWT; unauthenticated users are redirected to login
- [ ] Logout clears tokens and returns user to login
- [ ] API endpoints for applications reject unauthenticated requests with 401
- [ ] App shell layout renders with working navigation placeholders

### Non-Functional Requirements
- [ ] Password must meet policy (min 8 chars, require digit, require uppercase)
- [ ] JWT tokens expire after 7 days
- [ ] Form validation errors display within 100ms
- [ ] Login/register forms are responsive (mobile, tablet, desktop)
- [ ] No sensitive data (passwords, tokens) logged to console

### Technical Requirements
- [ ] Integration tests written and passing (backend auth endpoints)
- [ ] Code reviewed and approved by team member
- [ ] No console errors or warnings in browser
- [ ] API documentation updated (Swagger/OpenAPI)
- [ ] README updated with setup instructions

### UX Requirements
- [ ] Forms have clear validation error messages
- [ ] Loading states shown during API calls
- [ ] Retro-arcade theme applied consistently
- [ ] Keyboard navigation works (tab through forms, enter to submit)

---

## Components & Elements

### Backend Components
- **Solution Structure:** Clean Architecture (Domain, Application, Infrastructure, WebAPI)
- **ApplicationDbContext:** EF Core DbContext inheriting IdentityDbContext
- **ApplicationUser Entity:** Extends IdentityUser with custom properties
- **AuthController:** Register, Login, Logout, GetMe endpoints
- **JwtTokenService:** Generates and validates JWT tokens
- **CurrentUserService:** Extracts user ID from JWT claims

### Frontend Components
- **AppShell:** Main layout wrapper with header and content area
- **Header:** Top navigation with logo, nav links, user menu
- **NavLink:** Reusable navigation link component
- **Login Page:** Email/password form with validation
- **Register Page:** Display name, email, password, confirm password form
- **ProtectedRoute:** HOC that redirects unauthenticated users
- **AuthContext:** React context for auth state management
- **API Client:** Axios instance with JWT interceptors

### Database Changes
- **Users Table:** Created via EF Core Identity migration
- **Fields:** Id, Email, DisplayName, PasswordHash, Points (added later)

### External Dependencies
- **Backend:** Microsoft.AspNetCore.Identity, Microsoft.EntityFrameworkCore, JWT packages
- **Frontend:** react-router-dom, axios, Tailwind CSS, Press Start 2P font

---

## Task Breakdown

### PHASE 0: API CONTRACT (SYNC POINT - DO FIRST) ⚠️

Before either developer starts coding, agree on these shared contracts:

- [ ] **API Contract Review Meeting** - **Est:** 1h - **Assignee:** cadleta + realemmetts
  - Review auth endpoints table below
  - Confirm JWT claims structure
  - Agree on error response format
  - Document in this file

#### Auth Endpoints
| Method | Endpoint | Request Body | Response |
|--------|----------|--------------|----------|
| POST | /api/auth/register | { email, password, displayName } | { userId, email, token } |
| POST | /api/auth/login | { email, password } | { userId, email, token, expiresAt } |
| POST | /api/auth/logout | (none, uses auth header) | { success: true } |
| GET | /api/auth/me | (none, uses auth header) | { userId, email, displayName, points } |

#### JWT Claims
- `sub` (user ID)
- `email`
- `name` (display name)
- `exp` (expiration timestamp)

#### Error Response Format
```json
{ "error": "string", "details": ["string"] }
```

---

### Backend Tasks (cadleta)

#### A1. Create Solution Structure
- [ ] Create .NET 8 solution with Clean Architecture layers - **Est:** 2h - **Assignee:** cadleta
- [ ] Create projects: Domain, Application, Infrastructure, WebAPI - **Est:** 1h - **Assignee:** cadleta
- [ ] Add project references (WebAPI → Infrastructure → Application → Domain) - **Est:** 0.5h - **Assignee:** cadleta
- [ ] Add .gitignore for .NET - **Est:** 0.25h - **Assignee:** cadleta

**Files to create:**
- `BigJobHunterPro.sln`
- `src/Domain/Domain.csproj`
- `src/Application/Application.csproj`
- `src/Infrastructure/Infrastructure.csproj`
- `src/WebAPI/WebAPI.csproj`

---

#### A2. Configure ApplicationDbContext + Initial Migration
- [ ] Install EF Core + SQL Server packages - **Est:** 0.5h - **Assignee:** cadleta
- [ ] Create ApplicationDbContext inheriting IdentityDbContext - **Est:** 1h - **Assignee:** cadleta
- [ ] Configure connection string (localdb or Docker SQL Server) - **Est:** 1h - **Assignee:** cadleta
- [ ] Create initial migration for Users table - **Est:** 0.5h - **Assignee:** cadleta
- [ ] Add seed data helper for dev environment - **Est:** 1h - **Assignee:** cadleta

**Files to create:**
- `src/Infrastructure/Data/ApplicationDbContext.cs`
- `src/Infrastructure/Data/Migrations/` (generated)
- `src/WebAPI/appsettings.json`
- `src/WebAPI/appsettings.Development.json`

---

#### A3. Configure ASP.NET Identity + Password Policy
- [ ] Configure Identity services in Program.cs - **Est:** 1h - **Assignee:** cadleta
- [ ] Set password requirements (min 8, digit, uppercase) - **Est:** 0.5h - **Assignee:** cadleta
- [ ] Configure user lockout policy - **Est:** 0.5h - **Assignee:** cadleta
- [ ] Add ApplicationUser entity extending IdentityUser - **Est:** 1h - **Assignee:** cadleta

**Files to create/modify:**
- `src/Domain/Entities/ApplicationUser.cs`
- `src/WebAPI/Program.cs` (Identity config)

---

#### A4. Implement Register Endpoint
- [ ] Create RegisterRequest DTO with validation attributes - **Est:** 1h - **Assignee:** cadleta
- [ ] Create RegisterResponse DTO - **Est:** 0.5h - **Assignee:** cadleta
- [ ] Create AuthController with POST /api/auth/register - **Est:** 2h - **Assignee:** cadleta
- [ ] Validate email uniqueness - **Est:** 0.5h - **Assignee:** cadleta
- [ ] Hash password via Identity - **Est:** 0.5h - **Assignee:** cadleta
- [ ] Return JWT token on successful registration - **Est:** 1h - **Assignee:** cadleta

**Files to create:**
- `src/Application/DTOs/Auth/RegisterRequest.cs`
- `src/Application/DTOs/Auth/RegisterResponse.cs`
- `src/WebAPI/Controllers/AuthController.cs`

---

#### A5. Implement Login Endpoint + JWT Issuance
- [ ] Create LoginRequest DTO - **Est:** 0.5h - **Assignee:** cadleta
- [ ] Create LoginResponse DTO - **Est:** 0.5h - **Assignee:** cadleta
- [ ] Add POST /api/auth/login to AuthController - **Est:** 2h - **Assignee:** cadleta
- [ ] Validate credentials via Identity SignInManager - **Est:** 1h - **Assignee:** cadleta
- [ ] Create JWT token with configured claims - **Est:** 2h - **Assignee:** cadleta
- [ ] Set appropriate token expiration (7 days) - **Est:** 0.5h - **Assignee:** cadleta

**Files to create:**
- `src/Application/DTOs/Auth/LoginRequest.cs`
- `src/Application/DTOs/Auth/LoginResponse.cs`
- `src/Infrastructure/Services/JwtTokenService.cs`
- `src/Application/Interfaces/IJwtTokenService.cs`

---

#### A6. Add Auth Middleware + User Claim Helpers
- [ ] Configure JWT Bearer authentication in Program.cs - **Est:** 1.5h - **Assignee:** cadleta
- [ ] Add [Authorize] attribute support - **Est:** 0.5h - **Assignee:** cadleta
- [ ] Create CurrentUserService to extract user ID from claims - **Est:** 1h - **Assignee:** cadleta
- [ ] Create GET /api/auth/me endpoint (protected) - **Est:** 1h - **Assignee:** cadleta

**Files to create:**
- `src/Application/Interfaces/ICurrentUserService.cs`
- `src/Infrastructure/Services/CurrentUserService.cs`

---

#### A7. Create Health Check Endpoint
- [ ] Add GET /api/health endpoint (unauthenticated) - **Est:** 0.5h - **Assignee:** cadleta
- [ ] Return { status: "healthy", timestamp: ISO8601 } - **Est:** 0.25h - **Assignee:** cadleta

---

#### A8. Write Integration Tests
- [ ] Create test project - **Est:** 1h - **Assignee:** cadleta
- [ ] Test: Register with valid data returns 200 + token - **Est:** 1h - **Assignee:** cadleta
- [ ] Test: Register with duplicate email returns 400 - **Est:** 0.5h - **Assignee:** cadleta
- [ ] Test: Login with valid credentials returns 200 + token - **Est:** 0.5h - **Assignee:** cadleta
- [ ] Test: Login with invalid credentials returns 401 - **Est:** 0.5h - **Assignee:** cadleta
- [ ] Test: Protected endpoint without token returns 401 - **Est:** 0.5h - **Assignee:** cadleta
- [ ] Test: Protected endpoint with valid token returns 200 - **Est:** 0.5h - **Assignee:** cadleta

**Files to create:**
- `tests/WebAPI.IntegrationTests/WebAPI.IntegrationTests.csproj`
- `tests/WebAPI.IntegrationTests/AuthControllerTests.cs`

---

### Frontend Tasks (realemmetts)

#### B1. Scaffold React + Vite + TypeScript Project
- [ ] Create Vite project with React + TypeScript template - **Est:** 0.5h - **Assignee:** realemmetts
- [ ] Install dependencies (react-router-dom, axios, etc.) - **Est:** 0.5h - **Assignee:** realemmetts
- [ ] Configure TypeScript strict mode - **Est:** 0.5h - **Assignee:** realemmetts
- [ ] Add .gitignore for Node - **Est:** 0.25h - **Assignee:** realemmetts

**Directory:** `bigjobhunterpro-web/`

---

#### B2. Configure Tailwind CSS with Retro Theme
- [ ] Install Tailwind CSS + PostCSS + Autoprefixer - **Est:** 0.5h - **Assignee:** realemmetts
- [ ] Configure tailwind.config.js with theme colors - **Est:** 1h - **Assignee:** realemmetts
  - Forest Green: #2E4600
  - Blaze Orange: #FF6700
  - CRT Amber: #FFB000
  - Terminal Green: #00FF00
- [ ] Add "Press Start 2P" font for headers - **Est:** 0.5h - **Assignee:** realemmetts
- [ ] Add Inter font for body text - **Est:** 0.25h - **Assignee:** realemmetts

**Files to create:**
- `tailwind.config.js`
- `postcss.config.js`
- `src/index.css`

---

#### B3. Build App Shell Layout with Nav Placeholders
- [ ] Create AppShell component with header and main content area - **Est:** 2h - **Assignee:** realemmetts
- [ ] Add logo/title in header - **Est:** 0.5h - **Assignee:** realemmetts
- [ ] Add placeholder nav links (Dashboard, Applications, Party, Profile) - **Est:** 1h - **Assignee:** realemmetts
- [ ] Add user menu dropdown (placeholder) - **Est:** 1h - **Assignee:** realemmetts
- [ ] Style with retro-arcade theme - **Est:** 2h - **Assignee:** realemmetts

**Files to create:**
- `src/components/layout/AppShell.tsx`
- `src/components/layout/Header.tsx`
- `src/components/layout/NavLink.tsx`

---

#### B4. Configure React Router with Protected Routes
- [ ] Install react-router-dom v6 - **Est:** 0.25h - **Assignee:** realemmetts
- [ ] Create ProtectedRoute wrapper component - **Est:** 1.5h - **Assignee:** realemmetts
- [ ] Define routes (login, register, app/*, app/dashboard) - **Est:** 1h - **Assignee:** realemmetts
- [ ] Redirect unauthenticated users to /login - **Est:** 0.5h - **Assignee:** realemmetts
- [ ] Redirect authenticated users away from /login - **Est:** 0.5h - **Assignee:** realemmetts

**Files to create:**
- `src/router/index.tsx`
- `src/router/ProtectedRoute.tsx`
- `src/pages/Dashboard.tsx` (placeholder)

---

#### B5. Build Login Page with Validation
- [ ] Create Login page component - **Est:** 2h - **Assignee:** realemmetts
- [ ] Email input with validation (required, email format) - **Est:** 1h - **Assignee:** realemmetts
- [ ] Password input with validation (required, min 8 chars) - **Est:** 1h - **Assignee:** realemmetts
- [ ] Submit button with loading state - **Est:** 0.5h - **Assignee:** realemmetts
- [ ] Inline error messages (display within 100ms) - **Est:** 1h - **Assignee:** realemmetts
- [ ] Link to Register page - **Est:** 0.25h - **Assignee:** realemmetts
- [ ] Handle API errors gracefully - **Est:** 1h - **Assignee:** realemmetts

**Files to create:**
- `src/pages/Login.tsx`
- `src/components/forms/FormInput.tsx`
- `src/components/forms/FormError.tsx`

---

#### B6. Build Register Page with Validation
- [ ] Create Register page component - **Est:** 2h - **Assignee:** realemmetts
- [ ] Display name input (required) - **Est:** 0.5h - **Assignee:** realemmetts
- [ ] Email input with validation - **Est:** 0.5h - **Assignee:** realemmetts
- [ ] Password input with validation - **Est:** 0.5h - **Assignee:** realemmetts
- [ ] Confirm password with match validation - **Est:** 1h - **Assignee:** realemmetts
- [ ] Submit button with loading state - **Est:** 0.5h - **Assignee:** realemmetts
- [ ] Link to Login page - **Est:** 0.25h - **Assignee:** realemmetts
- [ ] Auto-login after successful registration - **Est:** 0.5h - **Assignee:** realemmetts

**Files to create:**
- `src/pages/Register.tsx`

---

#### B7. Implement API Client with Token Storage
- [ ] Create axios instance with base URL config - **Est:** 1h - **Assignee:** realemmetts
- [ ] Add request interceptor to attach JWT from storage - **Est:** 1h - **Assignee:** realemmetts
- [ ] Add response interceptor for 401 handling - **Est:** 1h - **Assignee:** realemmetts
- [ ] Store token in localStorage - **Est:** 0.5h - **Assignee:** realemmetts
- [ ] Create auth service functions (login, register, logout, getMe) - **Est:** 2h - **Assignee:** realemmetts
- [ ] Create AuthContext for state management - **Est:** 2h - **Assignee:** realemmetts
- [ ] Create useAuth hook - **Est:** 1h - **Assignee:** realemmetts

**Files to create:**
- `src/services/api.ts`
- `src/services/auth.ts`
- `src/hooks/useAuth.ts`
- `src/context/AuthContext.tsx`

---

#### B8. Implement Logout Flow
- [ ] Add logout button to AppShell header - **Est:** 0.5h - **Assignee:** realemmetts
- [ ] Clear token from storage on logout - **Est:** 0.5h - **Assignee:** realemmetts
- [ ] Redirect to /login after logout - **Est:** 0.25h - **Assignee:** realemmetts
- [ ] (Optional) Call /api/auth/logout to invalidate server-side - **Est:** 0.5h - **Assignee:** realemmetts

**Files to modify:**
- `src/components/layout/Header.tsx`
- `src/hooks/useAuth.ts`

---

## PHASE FINAL: INTEGRATION (SYNC POINT)

### Integration Checklist
- [ ] Dev B configures API base URL to Dev A's running backend - **Est:** 0.25h - **Assignee:** realemmetts
- [ ] Configure CORS on backend to allow frontend origin - **Est:** 0.5h - **Assignee:** cadleta
- [ ] Test: Register new user → verify in database - **Est:** 0.5h - **Assignee:** Both
- [ ] Test: Login with registered user → receive valid JWT - **Est:** 0.5h - **Assignee:** Both
- [ ] Test: Access protected route → displays AppShell - **Est:** 0.5h - **Assignee:** Both
- [ ] Test: Access protected route without token → redirects to login - **Est:** 0.5h - **Assignee:** Both
- [ ] Test: Logout → clears token, redirects to login - **Est:** 0.5h - **Assignee:** Both
- [ ] Test: Refresh page while logged in → stays authenticated - **Est:** 0.5h - **Assignee:** Both

---

## System Interactions

- After successful login/register, JWT token is stored in browser localStorage
- All subsequent API requests include JWT in Authorization header
- Backend validates JWT on protected endpoints, returns 401 if invalid/missing
- Frontend intercepts 401 responses and redirects to login
- App shell mounts after successful authentication check

---

## Edge Cases & Error Handling

### Edge Cases
- User enters email without @ symbol → Show validation error
- User's password doesn't meet requirements → Show specific requirements
- User tries to register with already-taken email → Show "Email already exists" error
- Session expires while user is filling form → Redirect to login on next API call
- User presses Enter instead of clicking submit → Form should submit

### Error Scenarios
- Network timeout → Show retry button with clear message
- Invalid credentials → Show "Email or password is incorrect" message
- Server error (500) → Show generic error, log details to console
- CORS issues → Developer needs to check backend CORS config

---

## Testing Notes

### Manual Test Cases
1. Happy path - Register new user → auto-login → see dashboard
2. Happy path - Login with valid credentials → see dashboard
3. Invalid email format → show validation error before submit
4. Wrong password → show auth error after submit
5. Empty fields → disable submit button
6. Network offline → show connection error
7. Logout → redirect to login, cannot access protected routes
8. Refresh page while logged in → stay logged in
9. Open protected route in new tab without login → redirect to login

### Automation Coverage
- **Backend Unit:** Password validation, email uniqueness check
- **Backend Integration:** All auth endpoints (register, login, logout, getMe)
- **Frontend Component:** Login form validation, Register form validation
- **E2E:** Full auth flow (register → login → logout)

---

## Design References

### Style Guide
- **Colors:**
  - Primary action buttons: Blaze Orange (#FF6700)
  - Success states: Terminal Green (#00FF00)
  - Headers/accent: Forest Green (#2E4600)
  - Highlights: CRT Amber (#FFB000)
- **Typography:**
  - Headers: Press Start 2P (12-16px for retro feel)
  - Body: Inter (14-16px for readability)
- **Spacing:** 16px between form fields, 24px between sections

### Terminology
- Use "The Lodge" for dashboard
- Use "Hunter" for user (e.g., "Welcome back, Hunter!")
- Buttons: "Start Hunting", "Join the Hunt", etc.

---

## Notes & Decisions

### Technical Decisions
- **JWT in localStorage:** Simple for MVP, will consider httpOnly cookies in future for better security
- **Clean Architecture:** Overkill for MVP but sets good foundation for scaling
- **Async standups:** Using markdown files to avoid Zoom fatigue and time zone issues

### Open Questions
- ~~Should we support OAuth login (Google/GitHub)?~~ → **Answer:** Not in MVP, backlog item
- ~~Password requirements?~~ → **Answer:** Min 8 chars, require digit, require uppercase

### Blockers
- None currently

---

## Progress Log

### 2026-01-04 - Sprint Start
- Sprint planning completed
- Tasks broken down and assigned
- API contract agreed upon in PHASE 0
- Both developers starting work on assigned tasks

---

**Created:** 2026-01-04
**Last Updated:** 2026-01-04
**Story File:** `Meta/Sprints/Sprint-1/stories/story-1-authenticated-app-shell.md`
**Git Branches:** `feature/story1-backend-auth` (cadleta), `feature/story1-frontend-auth` (realemmetts)
