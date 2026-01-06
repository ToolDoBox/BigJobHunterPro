# Sprint 7 Plan - Analytics and Insights

## Sprint Details

- **Sprint Number:** 7
- **Start Date:** 2026-03-30
- **End Date:** 2026-04-12
- **Duration:** 14 days (2 weeks)
- **Sprint Goal:** Ship the first analytics dashboard with funnel and success metrics

## Team Capacity

| Developer | Available Days | Velocity (SP/day) | Total Capacity (SP) |
|-----------|----------------|-------------------|---------------------|
| cadleta (Dev A - Backend) | 14 | ~0.45 | ~6 |
| realemmetts (Dev B - Frontend) | 14 | ~0.45 | ~6 |
| **Total** | **14** | - | **13** |

> **Note:** Adjust capacity if actual velocity from prior sprints differs.

## Sprint Goals

### Primary Goals
1. Analytics dashboard with core charts
2. Conversion funnel and average time between stages
3. Success rate breakdowns by source, skill, and company size

### Secondary Goals (Stretch)
- Add export-to-image for analytics widgets
- Add a dashboard empty state

## Selected User Stories

Total Committed Story Points: **13 SP**

| ID | User Story | Story Points | Priority | Assignee(s) | Status |
|----|------------|--------------|----------|-------------|---------|
| 17 | Analytics dashboard charts | 5 | High | cadleta + realemmetts | Not Started |
| 18 | Conversion funnel and stage timing | 5 | High | cadleta + realemmetts | Not Started |
| 19 | Success rate breakdowns | 3 | Medium | cadleta + realemmetts | Not Started |

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
- `stories/story-17-analytics-dashboard.md`
- `stories/story-18-conversion-funnel-stage-timing.md`
- `stories/story-19-success-rate-breakdowns.md`

## Dependencies and Risks

### Dependencies
- Data model must include status history or timeline events
- Skills and source data from Sprint 6

### Risks
- Analytics queries become slow as data grows
- Ambiguous definitions for conversion rate

**Mitigation:**
- Use indexed queries and caching for aggregates
- Define metrics in API docs and UI tooltips

## Sprint Ceremonies Schedule

| Ceremony | Day/Time | Duration | Attendees |
|----------|----------|----------|-----------|
| Sprint Planning | 2026-03-30 | 2 hours | cadleta, realemmetts |
| Daily Standup | Daily (async via markdown files) | 15 min | cadleta, realemmetts |
| Sprint Review | 2026-04-12 | 1 hour | cadleta, realemmetts |
| Sprint Retrospective | 2026-04-12 | 45 min | cadleta, realemmetts |

## Notes

- Align analytics metric definitions with Project-Scoping doc

---

**Created:** 2026-01-05
**Last Updated:** 2026-01-05
