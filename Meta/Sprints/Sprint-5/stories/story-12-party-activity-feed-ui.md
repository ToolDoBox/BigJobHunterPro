# Story 12: Party Activity Feed API and UI List

## User Story

**As a** hunting party member
**I want** a shared activity feed
**so that** I can see recent updates from my party

- **Story Points:** 5
- **Priority:** High
- **Sprint:** 5
- **Assignee(s):** cadleta + realemmetts
- **Status:** Not Started

**Status Legend:** Not Started | In Progress | Completed | Blocked | Moved to Next Sprint

---

## Desired Outcome

Party members can view a chronological feed of recent activity events.

---

## Acceptance Criteria (Definition of Done)

### Functional Requirements
- [ ] API returns recent events for a party in reverse-chronological order
- [ ] UI renders an activity list with avatar, label, points, and timestamp
- [ ] Feed supports pagination or a limit parameter

### Non-Functional Requirements
- [ ] Feed loads in under 800ms for 50 events
- [ ] Empty state explains how to generate activity
- [ ] Accessible list semantics and focus states are present

### Technical Requirements
- [ ] Integration test for GET /api/parties/{id}/activity
- [ ] Frontend unit test for list rendering
- [ ] No console errors or warnings

### Content/UX Requirements
- [ ] Copy is aligned with the hunting party tone
- [ ] Timestamps render as relative time (e.g., 2h ago)

---

## Components and Elements

### Frontend Components
- PartyActivityFeed component
- ActivityEventCard component
- Empty state component

### Backend Components
- GET /api/parties/{id}/activity?limit=50
- ActivityEventQuery service

### Database Changes
- None

### External Dependencies
- Date formatting library (existing)

---

## Task Breakdown

### Design and Planning
- [ ] Define event card layout and fields - **Est:** 2h - **Assignee:** realemmetts

### Backend Tasks
- [ ] Implement activity feed query and endpoint - **Est:** 3h - **Assignee:** cadleta
- [ ] Add pagination parameters - **Est:** 1h - **Assignee:** cadleta
- [ ] Write integration tests - **Est:** 2h - **Assignee:** cadleta

### Frontend Tasks
- [ ] Build feed list and card components - **Est:** 4h - **Assignee:** realemmetts
- [ ] Wire data fetching and loading states - **Est:** 2h - **Assignee:** realemmetts
- [ ] Add empty state and relative timestamps - **Est:** 2h - **Assignee:** realemmetts

### QA and Polish
- [ ] Verify mobile layout and scroll behavior - **Est:** 1h - **Assignee:** realemmetts

**Total Estimated Hours:** 17h

---

## System Interactions

- Consumes ActivityEvent data created in Story 11
- Receives real-time updates from Story 13

---

## Edge Cases and Error Handling

### Edge Cases
- Party has no events yet
- User is removed from party while viewing feed

### Error Scenarios
- API returns 403 for non-members

---

## Testing Notes

### Manual Test Cases
1. Load party feed with no events and confirm empty state
2. Load party feed with 10 events and verify ordering

### Automation Coverage
- Integration: GET activity for member returns 200
- Unit: Event list renders label, points, and timestamp

---

**Created:** 2026-01-05
**Last Updated:** 2026-01-05 by realemmetts
**Story File:** `Meta/Sprints/Sprint-5/stories/story-12-party-activity-feed-ui.md`
