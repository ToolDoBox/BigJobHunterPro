# Story 11: Activity Event Model and Logging

## User Story

**As a** hunting party member
**I want** my actions recorded as activity events
**so that** my party can see progress in real time

- **Story Points:** 5
- **Priority:** High
- **Sprint:** 5
- **Assignee(s):** cadleta
- **Status:** Completed

**Status Legend:** Not Started | In Progress | Completed | Blocked | Moved to Next Sprint

---

## Desired Outcome

Core user actions create consistent, queryable activity events tied to a hunting party.

---

## Acceptance Criteria (Definition of Done)

### Functional Requirements
- [x] ActivityEvent entity supports core types (application logged, status updated, offer received, milestone hit)
- [x] Events include PartyId, UserId, pointsDelta, and created timestamp
- [x] Scoring actions create a corresponding ActivityEvent

### Non-Functional Requirements
- [x] Event creation adds no more than 200ms to write paths
- [x] Event payloads avoid sensitive fields (company name optional)
- [x] Works on mobile, tablet, and desktop clients

### Technical Requirements
- [x] Unit tests cover event creation rules
- [x] Integration test validates event creation on status update
- [x] No console or server errors

### Content/UX Requirements
- [x] Event type labels are consistent with UI copy
- [x] Timestamps are stored in UTC

---

## Components and Elements

### Backend Components
- ActivityEvent entity and ActivityEventType enum
- ActivityEventService (create, normalize labels)
- Domain event or service hook from scoring paths

### Database Changes
- ActivityEvents table with indexes on PartyId, CreatedAt

### External Dependencies
- None

---

## Task Breakdown

### Design and Planning
- [x] Confirm core event types with product goals - **Est:** 1h - **Assignee:** cadleta

### Backend Tasks
- [x] Add ActivityEvent entity and enum - **Est:** 3h - **Assignee:** cadleta
- [x] Add persistence and indexes - **Est:** 2h - **Assignee:** cadleta
- [x] Hook event creation into scoring paths - **Est:** 3h - **Assignee:** cadleta
- [x] Write unit and integration tests - **Est:** 3h - **Assignee:** cadleta

### QA and Polish
- [x] Verify event creation latency is acceptable - **Est:** 1h - **Assignee:** cadleta

**Total Estimated Hours:** 10h

---

## System Interactions

- Events are consumed by the party activity feed API (Story 12)
- Events are broadcast to SignalR clients (Story 13)

---

## Edge Cases and Error Handling

### Edge Cases
- Scoring action is undone or rolled back
- User is not in a party at event time

### Error Scenarios
- Database write fails; action completes but event is not created

---

## Testing Notes

### Manual Test Cases
1. Log an application and confirm ActivityEvent record exists
2. Update status to Interview and verify event type and pointsDelta

### Automation Coverage
- Unit: ActivityEventService maps event types correctly
- Integration: Status update creates ActivityEvent row

---

## Notes and Decisions

### Technical Decisions
- Store event labels in UI layer, keep database type names short

---

**Created:** 2026-01-05
**Last Updated:** 2026-01-05 by cadleta
**Story File:** `Meta/Sprints/Sprint-5/stories/story-11-activity-event-model.md`
