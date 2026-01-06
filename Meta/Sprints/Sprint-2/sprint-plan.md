# Sprint 2 Plan - Core Tracking Depth

## Sprint Details

- **Sprint Number:** 2
- **Start Date:** 2026-01-18
- **End Date:** 2026-01-31
- **Duration:** 14 days (2 weeks)
- **Sprint Goal:** Ship application detail + status updates with consistent points and expose totals in the app shell

## Team Capacity

| Developer | Available Days | Velocity (SP/day) | Total Capacity (SP) |
|-----------|----------------|-------------------|---------------------|
| cadleta (Dev A - Backend) | 14 | ~0.4 | ~5-6 |
| realemmetts (Dev B - Frontend) | 14 | ~0.4 | ~5-6 |
| **Total** | **14** | - | **13** |

> **Note:** Update capacity if Sprint 1 actual velocity differs from estimate.

## Sprint Goals

### Primary Goals
1. Application detail view with edit support for core fields
2. Status updates with points via a shared scoring service
3. Total points visible in the app shell/user profile

### Secondary Goals (Stretch)
- Spike API contract for hunting party creation if Sprint 2 finishes early
- Navigation polish between list and detail views

## Selected User Stories

Total Committed Story Points: **10 SP**

| ID | User Story | Story Points | Priority | Assignee(s) | Status |
|----|------------|--------------|----------|-------------|---------|
| 4  | Application detail view | 3 | High | cadleta + realemmetts | ? Not Started |
| 5  | Application status updates | 3 | High | cadleta + realemmetts | ? Not Started |
| 6  | Points calculation service | 2 | High | cadleta | ? Not Started |
| 7  | User profile with points display | 2 | High | realemmetts | ? Not Started |

**Status Legend:**
- ? Not Started
- ?? In Progress
- ? Completed
- ?? Blocked
- ?? Moved to Next Sprint

## Definition of Done

A user story is considered complete when:

- [ ] All completion criteria are met (see individual story files)
- [ ] Code is written and peer-reviewed
- [ ] All tests pass (unit, integration, E2E as applicable)
- [ ] Documentation is updated (README, API docs, etc.)
- [ ] Feature works in local dev environment
- [ ] No console errors or warnings

## Sprint Backlog Organization

Stories are broken down in individual files:
- `stories/story-4-application-detail-view.md`
- `stories/story-5-application-status-updates.md`
- `stories/story-6-points-calculation-service.md`
- `stories/story-7-user-profile-points-display.md`

## Dependencies & Risks

### Dependencies
- Story 4 depends on Story 3 (applications list) for navigation entry point
- Story 5 depends on Story 4 for the detail surface and Story 6 for scoring rules
- Scoring table needs alignment across strategy/scoping/backlog before coding

### Risks
- Points rules ambiguity causes rework across backend and UI
- Detail view scope creep (notes, skills, timeline) delays delivery

**Mitigation:**
- Decide the scoring table on Day 1 and document in Story 6
- Limit editable fields to those captured in Sprint 1 (company, role, source, source URL)

## Sprint Ceremonies Schedule

| Ceremony | Day/Time | Duration | Attendees |
|----------|----------|----------|-----------|
| Sprint Planning | 2026-01-18 | 2 hours | cadleta, realemmetts |
| Daily Standup | Daily (async via markdown files) | 15 min | cadleta, realemmetts |
| Sprint Review | 2026-01-31 | 1 hour | cadleta, realemmetts |
| Sprint Retrospective | 2026-01-31 | 45 min | cadleta, realemmetts |

## Notes

- Keep status enum and points table consistent across API and UI
- Out of scope: hunting party features, leaderboards, import tool, analytics

---

**Created:** 2026-01-18
**Last Updated:** 2026-01-18
