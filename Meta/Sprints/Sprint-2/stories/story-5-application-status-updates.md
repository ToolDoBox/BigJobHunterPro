# Story 5: Application Status Updates

## User Story

**As a** job hunter
**I want** to update an application's status
**so that** my progress and points stay accurate

- **Story Points:** 3
- **Priority:** High
- **Sprint:** Sprint 2
- **Assignee(s):** cadleta (Backend) + realemmetts (Frontend)
- **Status:** ? Not Started

**Status Legend:** ? Not Started | ?? In Progress | ? Completed | ?? Blocked | ?? Moved to Next Sprint

---

## Desired Outcome

After completing this story, users can update an application's status from the detail view and immediately see points and status reflect the change.

---

## Acceptance Criteria (Definition of Done)

### Functional Requirements
- [ ] Status can be changed from the detail view
- [ ] Allowed statuses: Applied, Screening, Interview, Offer, Rejected
- [ ] Status updates persist via PATCH `/api/applications/{id}/status`
- [ ] Points are updated using the points calculation service
- [ ] Re-selecting the current status does not award additional points
- [ ] Status and points refresh in the UI after save

### Non-Functional Requirements
- [ ] Status update completes in <300ms at p95 for typical records
- [ ] Responsive layout on mobile, tablet, and desktop
- [ ] No console errors or warnings

### Technical Requirements
- [ ] PATCH `/api/applications/{id}/status` endpoint with validation
- [ ] PointsCalculationService used for status changes
- [ ] Integration tests for status update endpoint
- [ ] Frontend component tests for status control

### UX Requirements
- [ ] Status badge updates immediately after save
- [ ] Success toast confirms points awarded
- [ ] Error state shown when update fails

---

## Components & Elements

### Backend Components
- **ApplicationsController:** PATCH `/api/applications/{id}/status`
- **UpdateStatusRequest:** { status }
- **PointsCalculationService:** Consistent scoring rules

### Frontend Components
- **StatusSelect:** Dropdown or segmented control
- **StatusBadge:** Reuse from list/detail
- **Toast:** Success/error feedback

### Database Changes
- None (uses existing Applications table)

---

## Task Breakdown

> **Note:** Detailed task breakdown will be added when team starts work on this story.

### Backend Tasks (cadleta)
- [ ] Define status update request/response - **Est:** TBD
- [ ] Implement PATCH endpoint - **Est:** TBD
- [ ] Wire points calculation + update user totals - **Est:** TBD
- [ ] Add validation + integration tests - **Est:** TBD

### Frontend Tasks (realemmetts)
- [ ] Add status control to detail view - **Est:** TBD
- [ ] Wire API call and update UI state - **Est:** TBD
- [ ] Add success/error toasts - **Est:** TBD
- [ ] Write component tests - **Est:** TBD

---

## System Interactions

- Uses Detail View (Story 4)
- Uses Points Calculation Service (Story 6)
- Updates points display in App Shell (Story 7)

---

## Edge Cases & Error Handling

### Edge Cases
- User attempts invalid status -> show validation error
- User toggles status rapidly -> debounce or disable while saving

### Error Scenarios
- API 400 -> show server validation errors
- API 500 -> show generic error, allow retry

---

## Testing Notes

### Manual Test Cases
1. Change status -> points updated
2. Change status to same value -> no points awarded
3. Invalid status -> error shown

### Automation Coverage
- **Backend:** PATCH status endpoint
- **Frontend:** Status control update flow

---

**Created:** 2026-01-18
**Last Updated:** 2026-01-18
**Story File:** `Meta/Sprints/Sprint-2/stories/story-5-application-status-updates.md`
**Dependencies:** Story 4 (Application Detail View), Story 6 (Points Calculation Service)
