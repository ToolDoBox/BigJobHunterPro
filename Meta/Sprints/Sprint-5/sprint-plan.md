# Sprint 5 Plan - Activity Feed

## Sprint Details

- **Sprint Number:** 5
- **Start Date:** 2026-03-02
- **End Date:** 2026-03-15
- **Duration:** 14 days (2 weeks)
- **Sprint Goal:** Deliver a party activity feed with real-time updates and milestone notifications

## Team Capacity

| Developer | Available Days | Velocity (SP/day) | Total Capacity (SP) |
|-----------|----------------|-------------------|---------------------|
| cadleta (Dev A - Backend) | 14 | ~0.45 | ~6 |
| realemmetts (Dev B - Frontend) | 14 | ~0.45 | ~6 |
| **Total** | **14** | - | **13** |

> **Note:** Adjust capacity if actual velocity from prior sprints differs.

## Sprint Goals

### Primary Goals
1. Store activity events with clear types and party context
2. Ship a party activity feed API and UI list
3. Push real-time updates and milestone notifications

### Secondary Goals (Stretch)
- Add feed filtering by activity type
- Improve empty states and loading skeletons

## Selected User Stories

Total Committed Story Points: **13 SP**

| ID | User Story | Story Points | Priority | Assignee(s) | Status |
|----|------------|--------------|----------|-------------|---------|
| 11 | Activity event model and logging | 5 | High | cadleta | Not Started |
| 12 | Party activity feed API and UI list | 5 | High | cadleta + realemmetts | Not Started |
| 13 | Real-time feed updates and milestone notifications | 3 | High | cadleta + realemmetts | Not Started |

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
- `stories/story-11-activity-event-model.md`
- `stories/story-12-party-activity-feed-ui.md`
- `stories/story-13-realtime-feed-notifications.md`

## Dependencies & Risks

### Dependencies
- Hunting party membership is required to scope events to a party
- Points service must emit event metadata for scoring actions
- SignalR hub needs to be available for push updates

### Risks
- Feed noise if events are too granular
- Real-time updates degrade performance under load

**Mitigation:**
- Start with core event types only and expand later
- Use paging and server-side limits; cache recent events

## Sprint Ceremonies Schedule

| Ceremony | Day/Time | Duration | Attendees |
|----------|----------|----------|-----------|
| Sprint Planning | 2026-03-02 | 2 hours | cadleta, realemmetts |
| Daily Standup | Daily (async via markdown files) | 15 min | cadleta, realemmetts |
| Sprint Review | 2026-03-15 | 1 hour | cadleta, realemmetts |
| Sprint Retrospective | 2026-03-15 | 45 min | cadleta, realemmetts |

## Notes

- Align activity event naming with the scoring table
- Validate privacy settings before broadcasting details

---

**Created:** 2026-01-05
**Last Updated:** 2026-01-05
