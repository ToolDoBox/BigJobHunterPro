# Story 2: Quick Capture + Points

## User Story

**As a** job hunter
**I want** to log a new job application in 15 seconds or less
**so that** I can quickly track my applications without disrupting my job search flow

- **Story Points:** 5
- **Priority:** High
- **Sprint:** Sprint 1
- **Assignee(s):** cadleta (Backend) + realemmetts (Frontend)
- **Status:** ⬜ Not Started

**Status Legend:** ⬜ Not Started | 🔄 In Progress | ✅ Completed | 🚫 Blocked | 📦 Moved to Next Sprint

---

## Desired Outcome

After completing this story, users should be able to open a Quick Capture modal in one click, fill in minimal fields (company, role, source), submit with Enter key, and see confirmation within 15 seconds. The application should be stored with "Applied" status and award +1 point.

---

## Acceptance Criteria (Definition of Done)

From `Meta/Sprint 1.md`:

### Functional Requirements
- [ ] Quick Capture can be opened from the app shell in one click
- [ ] Minimal fields: company name, role title, source name, source URL (optional)
- [ ] Enter key submits the form and shows inline validation within 100ms
- [ ] Logging an application completes in <= 15 seconds for a typical user
- [ ] New application is stored with status "Applied" and +1 point awarded
- [ ] Success toast confirms the log and points update

### Non-Functional Requirements
- [ ] Form validation errors display within 100ms
- [ ] API response time < 200ms for application creation
- [ ] Modal opens/closes smoothly with animation
- [ ] Responsive on mobile, tablet, desktop

### Technical Requirements
- [ ] API endpoint for POST /api/applications
- [ ] Points calculation service wired to create flow
- [ ] Audit fields (CreatedDate, UpdatedDate) populated
- [ ] API validation tests passing
- [ ] Frontend component tests for Quick Capture modal

### UX Requirements
- [ ] Modal has clear visual hierarchy (retro-arcade theme)
- [ ] Success feedback is immediate and celebratory
- [ ] Error messages are helpful and specific
- [ ] Focus management (cursor in first field when modal opens)

---

## Components & Elements

### Backend Components
- **Application Entity:** Company, RoleTitle, Source, SourceUrl, Status, Points, CreatedDate, UpdatedDate
- **CreateApplicationDto:** Request DTO with validation
- **ApplicationResponseDto:** Response DTO
- **ApplicationsController:** POST /api/applications endpoint
- **PointsCalculationService:** Awards +1 point for "Applied" status

### Frontend Components
- **QuickCaptureModal:** Modal container with form
- **Text Inputs:** Company name, role title, source name, source URL (optional)
- **Submit/Cancel Buttons:** Primary action + secondary cancel
- **Inline Validation Messages:** Real-time error display
- **Success Toast:** Confirmation message with points update

### Database Changes
- **Applications Table:** Id, UserId, Company, RoleTitle, Source, SourceUrl, Status, Points, CreatedDate, UpdatedDate

---

## Task Breakdown

> **Note:** Detailed task breakdown will be added when team starts work on this story.
> Reference Story 1 structure for task format.

### Backend Tasks (cadleta)
- [ ] Create Application entity and DbContext mapping - **Est:** TBD
- [ ] Create migration for Applications table - **Est:** TBD
- [ ] Create DTOs (CreateApplicationDto, ApplicationResponseDto) - **Est:** TBD
- [ ] Implement POST /api/applications endpoint - **Est:** TBD
- [ ] Create PointsCalculationService (+1 for Applied) - **Est:** TBD
- [ ] Add validation (required fields, max lengths) - **Est:** TBD
- [ ] Write integration tests for application creation - **Est:** TBD

### Frontend Tasks (realemmetts)
- [ ] Create QuickCaptureModal component - **Est:** TBD
- [ ] Add modal trigger button to AppShell - **Est:** TBD
- [ ] Implement form inputs with validation - **Est:** TBD
- [ ] Wire up API call to POST /api/applications - **Est:** TBD
- [ ] Implement Enter key submit behavior - **Est:** TBD
- [ ] Add success toast notification - **Est:** TBD
- [ ] Test 15-second completion goal - **Est:** TBD
- [ ] Write component tests for Quick Capture - **Est:** TBD

---

## System Interactions

- Quick Capture modal is accessible from any page in the app shell
- On submit, calls POST /api/applications with JWT auth
- Backend creates application record and calculates points
- Frontend refreshes applications list (Story 3) after successful creation
- Points are immediately reflected in user profile

---

## Edge Cases & Error Handling

### Edge Cases
- User clicks outside modal while filling form → Show confirmation dialog
- User submits with empty required fields → Show validation errors
- User enters very long company name (>200 chars) → Truncate and warn
- Network is slow/offline → Show loading state, retry button

### Error Scenarios
- Duplicate application detection (future) → Warn user
- API returns 500 error → Show generic error, log details
- Validation fails on backend → Display server-side errors

---

## Testing Notes

### Manual Test Cases
1. Open modal → fills in 1 second
2. Fill all fields → completes in <10 seconds
3. Submit with Enter → success in <15 seconds total
4. Empty required field → validation error before submit
5. Network offline → shows error, allows retry

### Automation Coverage
- **Backend:** POST /api/applications returns 201 with valid data
- **Backend:** POST /api/applications returns 400 with invalid data
- **Frontend:** Quick Capture modal opens/closes correctly
- **Frontend:** Form validation works on all fields
- **E2E:** Full quick capture flow (open → fill → submit → see in list)

---

## Design References

### Quick Capture Components
- **Modal Container:** Retro arcade border, centered overlay
- **Header:** "LOG A HUNT" in Press Start 2P font
- **Inputs:** Compact, clear labels, placeholder text
- **Buttons:**
  - Submit: Blaze Orange (#FF6700), "LOCK IT IN"
  - Cancel: Gray, "NEVERMIND"
- **Success Toast:** Terminal Green (#00FF00), "+1 POINT! Hunt logged."

---

## Notes & Decisions

### Technical Decisions
- Using simple modal state (open/close) in React context
- Points calculated on backend (single source of truth)
- Source URL is optional (many applications don't track where they came from)

### Open Questions
- Should we auto-save draft applications? → Backlog for future sprint
- Should we allow bulk import in Quick Capture? → No, separate import tool (Story TBD)

### Blockers
- Depends on Story 1 (auth) being complete

---

## Progress Log

> Will be updated as work progresses

---

**Created:** 2026-01-04
**Last Updated:** 2026-01-04
**Story File:** `Meta/Sprints/Sprint-1/stories/story-2-quick-capture.md`
**Dependencies:** Story 1 (Authenticated App Shell) must be complete
