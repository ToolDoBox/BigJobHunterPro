# Story 6: Points Calculation Service

## User Story

**As a** product owner
**I want** a single points calculation service
**so that** scoring is consistent across creation and status updates

- **Story Points:** 2
- **Priority:** High
- **Sprint:** Sprint 2
- **Assignee(s):** cadleta (Backend)
- **Status:** ? Completed

**Status Legend:** ? Not Started | ?? In Progress | ? Completed | ?? Blocked | ?? Moved to Next Sprint

---

## Desired Outcome

After completing this story, points for application creation and status changes are computed by one service with a documented scoring table.

---

## Acceptance Criteria (Definition of Done)

### Functional Requirements
- [x] Points are calculated through a single service (no duplicated logic)
- [x] Scoring table is documented and used by application create + status update flows
- [x] Re-applying the same status does not award points again

### Scoring Table (Draft - confirm Day 1)
- [x] Applied: +1 (on creation)
- [x] Screening: +2
- [x] Interview: +5
- [x] Offer: +50
- [x] Rejected: +5

### Technical Requirements
- [x] PointsCalculationService has unit tests for each status
- [x] Application creation flow uses the service (Story 2)
- [x] Status update flow uses the service (Story 5)
- [x] User total points updated transactionally with application changes

---

## Components & Elements

### Backend Components
- **PointsCalculationService:** Scoring rules + helper methods
- **PointsRules:** Central constants or config

### Database Changes
- None (uses existing Applications and Users tables)

---

## Task Breakdown

> **Note:** Detailed task breakdown will be added when team starts work on this story.

### Backend Tasks (cadleta)
- [ ] Define scoring rules and service API - **Est:** TBD
- [ ] Wire into create + status update flows - **Est:** TBD
- [ ] Add unit tests for scoring logic - **Est:** TBD
- [ ] Verify user points update is consistent - **Est:** TBD

---

## Edge Cases & Error Handling

- Status update to same value -> no points change
- Status downgrade -> no points refund (future story if needed)

---

## Testing Notes

### Automation Coverage
- **Unit:** PointsCalculationService scoring table
- **Integration:** Create + status update use service

---

**Created:** 2026-01-18
**Last Updated:** 2026-01-19
**Story File:** `Meta/Sprints/Sprint-2/stories/story-6-points-calculation-service.md`
**Dependencies:** Story 2 (Quick Capture) and Story 5 (Status Updates)
