# Sprint 1 - Foundation + Quick Capture (Phase 1)

Start Date: 2026-01-04
End Date: 2026-01-17
Length: 14 days

Sprint Goal:
- Deliver an authenticated app shell with Quick Capture and a basic Applications list so the friend group can log and review entries quickly.

Sprint Planning

1) Review Backlog (Top Candidates)
- Backend schema + EF Core migrations
- Auth (email/password, JWT)
- Application CRUD endpoints
- Quick Capture modal + API
- Points calculation service
- Applications list view
- Application detail view
- Hunting party + leaderboard
- Job-Hunt-Context import tool

2) Define Goals
- Working login and protected app shell
- Quick Capture flow that meets the 15-second promise
- Applications list to confirm entries are saved and visible

3) Select Stories (Sprint Backlog)
1. Authenticated app shell (5 SP)
2. Quick Capture + points (5 SP)
3. Applications list view (3 SP)
Total: 13 SP

4) Estimate Effort
- Story points use Fibonacci scale (1, 2, 3, 5, 8).
- Anything above 8 is split.

5) Build Sprint Backlog (Comprehensive)
Story 1: Authenticated app shell (5 SP)
Backend
- Create solution structure (Api, Application, Domain, Infrastructure).
- Configure ApplicationDbContext + initial migration (Users table).
- Configure ASP.NET Identity + password policy.
- Implement register/login endpoints with JWT issuance.
- Add auth middleware and user id claim helper.
Frontend
- Add app shell layout with placeholder nav and route outlets.
- Configure React Router protected routes.
- Build login/register screens with validation.
- Implement API client with token storage and auth headers.
- Implement logout flow (clear tokens, redirect to login).
QA/Tests
- Integration tests: register/login and 401 on protected endpoints.
- Manual check: login, logout, redirect behavior.

Story 2: Quick Capture + points (5 SP)
Backend
- Add Application entity + DTOs + validation.
- POST /api/applications with default status "Applied".
- Points calculation service (+1 applied) wired to create flow.
- Add audit fields (CreatedDate, UpdatedDate).
Frontend
- Quick Capture modal with minimal inputs and validation.
- Enter key submits; inline errors display within 100ms.
- Success toast, close modal, refresh list.
QA/Tests
- API test: validation errors and success payload.
- Manual check: capture flow completes in <= 15 seconds.

Story 3: Applications list view (3 SP)
Backend
- GET /api/applications sorted by CreatedDate desc.
- DTO projection to avoid overfetching.
Frontend
- List/table view with company, role, status, created date.
- Empty state with "Log your first hunt" CTA.
- Loading and error states.
QA/Tests
- Manual check: list shows newly created entries.

Cross-Cutting Tasks (Included in Story Estimates)
- Baseline linting and formatting for frontend and backend.
- Seed minimal dev data for local UI checks.
- Update README with local run steps for API and frontend.

6) Set Duration
- 2026-01-04 to 2026-01-17 (2 weeks)

Completion Criteria

Story 1: Authenticated app shell (5 SP)
- User can register and log in via email/password.
- Authenticated routes require a valid JWT; unauthenticated users are redirected to login.
- Logout clears tokens and returns user to login.
- API endpoints for applications reject unauthenticated requests with 401.
- App shell layout renders with working navigation placeholders.

Story 2: Quick Capture + points (5 SP)
- Quick Capture can be opened from the app shell in one click.
- Minimal fields: company name, role title, source name, source URL (optional).
- Enter key submits the form and shows inline validation within 100ms.
- Logging an application completes in <= 15 seconds for a typical user.
- New application is stored with status "Applied" and +1 point awarded.
- Success toast confirms the log and points update.

Story 3: Applications list view (3 SP)
- List shows all logged applications for the user, newest first.
- Each row shows company, role, status, and created date.
- Empty state provides clear CTA to open Quick Capture.
- List view loads in <400ms at p95 for typical datasets.

Components and UI Elements (for design reference)

Quick Capture
- Modal container
- Text inputs (company name, role title, source name, source URL)
- Primary submit button
- Cancel button
- Inline validation messages

Applications List
- Table or card list
- Status badge
- Empty state panel with CTA

Out of Scope (Next Sprint Candidates)
- Application detail view and editing
- Hunting party creation and leaderboards
- Job-Hunt-Context import tool
- Activity feed and real-time updates
