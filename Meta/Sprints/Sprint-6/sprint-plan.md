# Sprint 6 Plan - Advanced Tracking

## Sprint Details

- **Sprint Number:** 6
- **Start Date:** 2026-03-16
- **End Date:** 2026-03-29
- **Duration:** 14 days (2 weeks)
- **Sprint Goal:** Add deeper tracking fields for notes, skills, sources, salary, and follow-ups

## Team Capacity

| Developer | Available Days | Velocity (SP/day) | Total Capacity (SP) |
|-----------|----------------|-------------------|---------------------|
| cadleta (Dev A - Backend) | 14 | ~0.45 | ~6 |
| realemmetts (Dev B - Frontend) | 14 | ~0.45 | ~6 |
| **Total** | **14** | - | **13** |

> **Note:** Adjust capacity if actual velocity from prior sprints differs.

## Sprint Goals

### Primary Goals
1. Capture application notes and interview prep notes
2. Tag skills and track application sources
3. Track salary expectations and follow-up reminders

### Secondary Goals (Stretch)
- Enable inline editing in the detail view
- Add quick filters for skills and sources

## Selected User Stories

Total Committed Story Points: **13 SP**

| ID | User Story | Story Points | Priority | Assignee(s) | Status |
|----|------------|--------------|----------|-------------|---------|
| 14 | Application notes and interview prep notes | 5 | High | cadleta + realemmetts | Not Started |
| 15 | Skills tagging and source tracking | 5 | High | cadleta + realemmetts | Not Started |
| 16 | Salary expectations and follow-up reminders | 3 | Medium | cadleta + realemmetts | Not Started |

**Status Legend:** Not Started | In Progress | Completed | Blocked | Moved to Next Sprint

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
- `stories/story-14-application-notes-interview-prep.md`
- `stories/story-15-skills-tags-and-sources.md`
- `stories/story-16-salary-and-follow-up-reminders.md`

## Dependencies and Risks

### Dependencies
- Application detail view must support new fields
- Privacy settings may restrict fields shared in activity feed

### Risks
- Scope creep on notes and reminders
- Data model changes impact import/export

**Mitigation:**
- Keep notes text-only in Sprint 6
- Add fields to import/export mapping with defaults

## Sprint Ceremonies Schedule

| Ceremony | Day/Time | Duration | Attendees |
|----------|----------|----------|-----------|
| Sprint Planning | 2026-03-16 | 2 hours | cadleta, realemmetts |
| Daily Standup | Daily (async via markdown files) | 15 min | cadleta, realemmetts |
| Sprint Review | 2026-03-29 | 1 hour | cadleta, realemmetts |
| Sprint Retrospective | 2026-03-29 | 45 min | cadleta, realemmetts |

## Notes

- Keep the quick capture flow fast; defer advanced inputs to detail view

---

**Created:** 2026-01-05
**Last Updated:** 2026-01-05
