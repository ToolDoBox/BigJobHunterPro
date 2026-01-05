# Sprint 1 Plan - Foundation + Quick Capture

## Sprint Details

- **Sprint Number:** 1
- **Start Date:** 2026-01-04
- **End Date:** 2026-01-17
- **Duration:** 14 days (2 weeks)
- **Sprint Goal:** Deliver an authenticated app shell with Quick Capture and a basic Applications list so the friend group can log and review entries quickly

## Team Capacity

| Developer | Available Days | Velocity (SP/day) | Total Capacity (SP) |
|-----------|----------------|-------------------|---------------------|
| cadleta (Dev A - Backend) | 14 | ~0.4 | ~5-6 |
| realemmetts (Dev B - Frontend) | 14 | ~0.4 | ~5-6 |
| **Total** | **14** | - | **13** |

> **Note:** First sprint - using conservative estimate of 13 SP total. Will use actual velocity from this sprint to calculate capacity for Sprint 2.

## Sprint Goals

### Primary Goals
1. Working login and protected app shell
2. Quick Capture flow that meets the 15-second promise
3. Applications list to confirm entries are saved and visible

### Secondary Goals (Stretch)
- Application detail view
- Basic error handling and validation polish

## Selected User Stories

Total Committed Story Points: **13 SP**

| ID | User Story | Story Points | Priority | Assignee(s) | Status |
|----|------------|--------------|----------|-------------|---------|
| 1  | Authenticated app shell | 5 | High | cadleta + realemmetts | ⬜ Not Started |
| 2  | Quick Capture + points | 5 | High | cadleta + realemmetts | ⬜ Not Started |
| 3  | Applications list view | 3 | High | cadleta + realemmetts | ⬜ Not Started |

**Status Legend:**
- ⬜ Not Started
- 🔄 In Progress
- ✅ Completed
- 🚫 Blocked
- 📦 Moved to Next Sprint

## Definition of Done

A user story is considered complete when:

- [ ] All completion criteria are met (see individual story files)
- [ ] Code is written and peer-reviewed
- [ ] All tests pass (unit, integration, E2E as applicable)
- [ ] Code is merged to main branch
- [ ] Documentation is updated (README, API docs, etc.)
- [ ] Feature works in local dev environment
- [ ] No console errors or warnings

## Sprint Backlog Organization

Stories are broken down in individual files:
- `stories/story-1-authenticated-app-shell.md`
- `stories/story-2-quick-capture.md`
- `stories/story-3-applications-list.md`

## Dependencies & Risks

### Dependencies
- Dev A (Backend) and Dev B (Frontend) need to agree on API contracts before starting (see PHASE 0 in Story 1)
- Backend must be deployed/running for frontend integration testing

### Risks
- **Risk 1:** First time working with full-stack integration - may take longer than estimated
- **Risk 2:** Clean Architecture setup in .NET may have learning curve
- **Risk 3:** Retro-arcade theme styling may require iteration

**Mitigation:**
- Complete API contract agreement in first day (PHASE 0)
- Backend merges first, then frontend (reduces integration conflicts)
- Keep styling simple for MVP, iterate in future sprints

## Sprint Ceremonies Schedule

| Ceremony | Day/Time | Duration | Attendees |
|----------|----------|----------|-----------|
| Sprint Planning | 2026-01-04 | 2 hours | cadleta, realemmetts |
| Daily Standup | Daily (async via markdown files) | 15 min | cadleta, realemmetts |
| Sprint Review | 2026-01-17 | 1 hour | cadleta, realemmetts |
| Sprint Retrospective | 2026-01-17 | 45 min | cadleta, realemmetts |

## Technology Stack

### Backend (cadleta)
- **Framework:** ASP.NET Core 8 Web API
- **Architecture:** Clean Architecture (Domain → Application → Infrastructure → API)
- **ORM:** Entity Framework Core with MS SQL Server
- **Auth:** ASP.NET Core Identity + JWT tokens

### Frontend (realemmetts)
- **Framework:** React 18 + TypeScript + Vite
- **Styling:** Tailwind CSS with retro-arcade theme
- **State:** React Context (auth), later Redux Toolkit
- **Routing:** React Router v6

## Cross-Cutting Tasks

Included in story estimates:
- Baseline linting and formatting for frontend and backend
- Seed minimal dev data for local UI checks
- Update README with local run steps for API and frontend

## Out of Scope (Next Sprint Candidates)

- Application detail view and editing
- Hunting party creation and leaderboards
- Job-Hunt-Context import tool
- Activity feed and real-time updates
- SignalR integration
- Advanced error handling and retry logic

## Notes

- This is our first sprint using the new sprint management system
- Daily standups are async via markdown files (see `daily-standups/` folder)
- Each developer updates their own standup file daily to avoid merge conflicts
- Story files will be updated throughout the sprint as tasks are completed

---

**Created:** 2026-01-04
**Last Updated:** 2026-01-04
