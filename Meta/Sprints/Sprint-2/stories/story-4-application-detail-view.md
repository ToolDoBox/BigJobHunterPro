# Story 4: Application Detail View

## User Story

**As a** job hunter
**I want** to view and edit a single application
**so that** I can keep my application data accurate and understand current status

- **Story Points:** 3
- **Priority:** High
- **Sprint:** Sprint 2
- **Assignee(s):** cadleta (Backend) + realemmetts (Frontend)
- **Status:** ✅ Completed (with minor gaps)

**Status Legend:** ? Not Started | ?? In Progress | ? Completed | ?? Blocked | ?? Moved to Next Sprint

---

## Desired Outcome

After completing this story, users can open an application detail view from the list, see core fields at a glance, and update the basic fields captured during Quick Capture.

---

## Acceptance Criteria (Definition of Done)

### Functional Requirements
- [x] Clicking an application row opens a detail view (route: `/app/applications/{id}`)
- [x] Detail view shows company, role title, status, source name, source URL (if present), created date, points, work mode, location, salary range, job description, required skills, nice-to-have skills, and AI parsing status
- [x] User can edit company, role title, source name, source URL, status, work mode, location, salary range, job description, required skills, and nice-to-have skills with Save/Cancel controls
- [x] User can delete an application log from the detail view with confirmation (Modal: "DELETE APPLICATION" with NEVERMIND/CONFIRM buttons)
- [x] Saving updates the record via PUT `/api/applications/{id}` and refreshes displayed values
- [x] Invalid IDs show a not found state; unauthenticated users are redirected to login (404: "TARGET LOST" page with "BACK TO ARMORY" button)

### Non-Functional Requirements
- [x] Detail view loads in <400ms at p95 for typical records
- [x] Responsive layout on mobile, tablet, and desktop (Tested at 375x667 mobile resolution - responsive)
- [x] No console errors or warnings

### Technical Requirements
- [x] GET `/api/applications/{id}` returns a detail DTO (including AI parsing fields)
- [x] PUT `/api/applications/{id}` validates input and updates audit timestamps
- [x] DELETE `/api/applications/{id}` removes the application log and adjusts points (Returns 204, correctly deducts points)
- [x] Integration tests for GET/PUT endpoints (tests/WebAPI.IntegrationTests/Controllers/ApplicationsControllerTests.cs exists)
- [!] Frontend component tests for render + edit flow (Not verified in codebase)

### UX Requirements
- [x] Status badge styling matches list view
- [!] Inline validation for required fields (Fields are editable but no real-time validation observed)
- [x] Clear success feedback on save (Toast: "Application updated successfully!" / "Application deleted.")

---

## Components & Elements

### Backend Components
- **ApplicationsController:** GET/PUT `/api/applications/{id}`
- **ApplicationDetailDto:** Full detail response
- **UpdateApplicationRequest:** Editable fields only
- **Validation:** Required fields and max length checks

### Frontend Components
- **ApplicationDetailPage:** Layout and routing
- **EditableField:** Reusable input component
- **SaveCancelBar:** Action controls
- **Loading/Error States:** Skeleton and retry

### Database Changes
- None (uses existing Applications table)

---

## Task Breakdown

> **Note:** Detailed task breakdown will be added when team starts work on this story.

### Backend Tasks (cadleta)
- [ ] Define detail DTO + update request model - **Est:** TBD
- [ ] Implement GET `/api/applications/{id}` - **Est:** TBD
- [ ] Implement PUT `/api/applications/{id}` - **Est:** TBD
- [ ] Add validation + integration tests - **Est:** TBD

### Frontend Tasks (realemmetts)
- [ ] Build detail page layout - **Est:** TBD
- [ ] Wire data fetch with loading/error states - **Est:** TBD
- [ ] Add edit mode with validation - **Est:** TBD
- [ ] Implement save/cancel flow + success toast - **Est:** TBD
- [ ] Write component tests - **Est:** TBD

---

## System Interactions

- Launched from Applications List (Story 3)
- Status updates handled in Story 5

---

## Edge Cases & Error Handling

### Edge Cases
- Missing source URL -> display placeholder
- Very long company/role -> truncate with tooltip
- Unsaved edits -> confirm before navigation

### Error Scenarios
- API 404 -> not found state
- API 500 -> error state with retry
- Network offline -> show retry option

---

## Testing Notes

### Manual Test Cases
1. Open detail from list -> correct data displayed
2. Edit fields -> save -> changes persist
3. Cancel edit -> no changes saved
4. Invalid ID -> not found state

### Automation Coverage
- **Backend:** GET/PUT detail endpoints
- **Frontend:** Detail view render + edit flow

---

**Created:** 2026-01-18
**Last Updated:** 2026-01-19
**Story File:** `Meta/Sprints/Sprint-2/stories/story-4-application-detail-view.md`
**Dependencies:** Story 3 (Applications List View)
