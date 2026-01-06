# Story 13: Real-Time Feed Updates and Milestone Notifications

## User Story

**As a** hunting party member
**I want** activity updates to appear in real time
**so that** I do not need to refresh to see progress

- **Story Points:** 3
- **Priority:** High
- **Sprint:** 5
- **Assignee(s):** cadleta + realemmetts
- **Status:** Not Started

**Status Legend:** Not Started | In Progress | Completed | Blocked | Moved to Next Sprint

---

## Desired Outcome

Activity feed updates appear within seconds and highlight milestones.

---

## Acceptance Criteria (Definition of Done)

### Functional Requirements
- [ ] SignalR broadcasts new ActivityEvent to party members
- [ ] UI appends incoming events to the feed without reload
- [ ] Milestone events trigger a distinct notification style

### Non-Functional Requirements
- [ ] Real-time update latency is under 2 seconds
- [ ] Connection loss shows a non-blocking warning

### Technical Requirements
- [ ] Integration test validates SignalR broadcast payload
- [ ] Frontend handles reconnect logic

### Content/UX Requirements
- [ ] Milestone events are labeled clearly (e.g., 10 applications logged)

---

## Components and Elements

### Frontend Components
- SignalR client connection in party area
- Milestone notification banner or toast

### Backend Components
- ActivityHub SignalR hub
- Broadcast on ActivityEvent creation

### Database Changes
- None

### External Dependencies
- SignalR client package

---

## Task Breakdown

### Backend Tasks
- [ ] Add ActivityHub and broadcast on event creation - **Est:** 3h - **Assignee:** cadleta
- [ ] Add milestone detection rules - **Est:** 2h - **Assignee:** cadleta

### Frontend Tasks
- [ ] Subscribe to ActivityHub and update feed state - **Est:** 2h - **Assignee:** realemmetts
- [ ] Add milestone notification styling - **Est:** 2h - **Assignee:** realemmetts
- [ ] Add reconnect status indicator - **Est:** 1h - **Assignee:** realemmetts

### QA and Polish
- [ ] Test real-time updates with two users - **Est:** 1h - **Assignee:** cadleta

**Total Estimated Hours:** 11h

---

## System Interactions

- Consumes ActivityEvent from Story 11
- Enhances feed UI from Story 12

---

## Edge Cases and Error Handling

### Edge Cases
- User opens feed in multiple tabs
- User not in party tries to connect

### Error Scenarios
- SignalR disconnect during high activity

---

## Testing Notes

### Manual Test Cases
1. Trigger an activity event and confirm it appears in another client
2. Trigger a milestone event and verify notification styling

### Automation Coverage
- Integration: Broadcast payload includes partyId and eventId

---

**Created:** 2026-01-05
**Last Updated:** 2026-01-05 by cadleta
**Story File:** `Meta/Sprints/Sprint-5/stories/story-13-realtime-feed-notifications.md`
