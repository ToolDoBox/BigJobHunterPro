# Story 3: Applications List View

## User Story

**As a** job hunter
**I want** to see all my logged applications in one place
**so that** I can review what I've applied to and track my progress

- **Story Points:** 3
- **Priority:** High
- **Sprint:** Sprint 1
- **Assignee(s):** cadleta (Backend) + realemmetts (Frontend)
- **Status:** ⬜ Not Started

**Status Legend:** ⬜ Not Started | 🔄 In Progress | ✅ Completed | 🚫 Blocked | 📦 Moved to Next Sprint

---

## Desired Outcome

After completing this story, users should be able to view a list/table of all their logged applications, sorted by newest first, showing key information (company, role, status, date). If no applications exist, they should see an empty state with a clear call-to-action to log their first application.

---

## Acceptance Criteria (Definition of Done)

From `Meta/Sprint 1.md`:

### Functional Requirements
- [ ] List shows all logged applications for the user, newest first
- [ ] Each row shows company, role, status, and created date
- [ ] Empty state provides clear CTA to open Quick Capture
- [ ] List view loads in <400ms at p95 for typical datasets

### Non-Functional Requirements
- [ ] Pagination or virtualization for large datasets (100+ applications)
- [ ] Responsive design (card view on mobile, table on desktop)
- [ ] Loading skeleton while fetching data
- [ ] Error state if API call fails

### Technical Requirements
- [ ] GET /api/applications endpoint with sorting
- [ ] DTO projection to avoid overfetching
- [ ] Integration test for GET endpoint
- [ ] Frontend component test for list rendering

### UX Requirements
- [ ] Status badges color-coded by application status
- [ ] Dates formatted in human-readable format ("2 days ago")
- [ ] Smooth loading transition (skeleton → data)
- [ ] Retro-arcade styling consistent with theme

---

## Components & Elements

### Backend Components
- **ApplicationsController:** GET /api/applications endpoint
- **ApplicationListDto:** Lightweight DTO (Id, Company, RoleTitle, Status, CreatedDate)
- **Sorting/Filtering Logic:** Order by CreatedDate descending

### Frontend Components
- **ApplicationsList:** Main list/table component
- **ApplicationRow:** Individual application row/card
- **StatusBadge:** Color-coded status indicator
- **EmptyState:** "No hunts logged yet" with CTA button
- **LoadingState:** Skeleton loader
- **ErrorState:** "Failed to load" with retry button

### Database Changes
- None (uses existing Applications table from Story 2)

---

## Task Breakdown

> **Note:** Detailed task breakdown will be added when team starts work on this story.
> Reference Story 1 structure for task format.

### Backend Tasks (cadleta)
- [ ] Create ApplicationListDto - **Est:** TBD
- [ ] Implement GET /api/applications endpoint - **Est:** TBD
- [ ] Add filtering by current user (from JWT) - **Est:** TBD
- [ ] Add sorting (CreatedDate descending) - **Est:** TBD
- [ ] Add pagination support (optional for MVP) - **Est:** TBD
- [ ] Write integration tests - **Est:** TBD

### Frontend Tasks (realemmetts)
- [ ] Create ApplicationsList component - **Est:** TBD
- [ ] Create ApplicationRow/Card component - **Est:** TBD
- [ ] Create StatusBadge component - **Est:** TBD
- [ ] Implement API call to GET /api/applications - **Est:** TBD
- [ ] Add loading skeleton - **Est:** TBD
- [ ] Add empty state with CTA - **Est:** TBD
- [ ] Add error state with retry - **Est:** TBD
- [ ] Format dates (relative time) - **Est:** TBD
- [ ] Style as table (desktop) and cards (mobile) - **Est:** TBD
- [ ] Write component tests - **Est:** TBD

---

## System Interactions

- Applications list is shown on main dashboard/applications page
- Refreshes automatically after Quick Capture submission (Story 2)
- Clicking an application row opens detail view (future story)
- Status badge colors match overall application status color scheme

---

## Edge Cases & Error Handling

### Edge Cases
- User has 0 applications → Show empty state
- User has 1000+ applications → Pagination or virtual scroll
- Application has very long company name → Truncate with ellipsis
- Date is from 2 years ago → Show full date instead of "730 days ago"

### Error Scenarios
- API returns 500 → Show error state with retry button
- Network offline → Show offline message
- Invalid token (401) → Redirect to login (handled by interceptor)

---

## Testing Notes

### Manual Test Cases
1. User with 0 applications → sees empty state with CTA
2. User with 5 applications → sees list, newest first
3. Click empty state CTA → opens Quick Capture modal
4. After logging new application → list refreshes with new entry at top
5. Network offline → shows error state

### Automation Coverage
- **Backend:** GET /api/applications returns 200 with application array
- **Backend:** GET /api/applications filters by current user only
- **Backend:** GET /api/applications sorts by CreatedDate desc
- **Frontend:** List renders correctly with mock data
- **Frontend:** Empty state shows when data is empty
- **Frontend:** Loading state shows while fetching

---

## Design References

### List View (Desktop)
- **Table Columns:**
  - Company (bold, larger font)
  - Role Title
  - Status (badge)
  - Date Applied (relative time)
- **Row Hover:** Subtle highlight with retro border effect
- **Status Badges:**
  - Applied: Terminal Green (#00FF00)
  - Screening: CRT Amber (#FFB000)
  - Interview: Blaze Orange (#FF6700)
  - Offer: Forest Green (#2E4600)
  - Rejected: Gray

### List View (Mobile)
- **Card Layout:** Stacked cards with all info visible
- **Touch Target:** Full card is tappable (44px min height)

### Empty State
- **Icon:** Retro binoculars or crosshair icon
- **Message:** "NO HUNTS LOGGED YET"
- **CTA Button:** "LOG YOUR FIRST HUNT" → Opens Quick Capture

---

## Notes & Decisions

### Technical Decisions
- Using simple GET request, no real-time updates for MVP
- Pagination deferred unless performance issues arise
- Date formatting using `date-fns` or similar library

### Open Questions
- Should we support filtering by status? → Backlog for future sprint
- Should we support search by company name? → Backlog for future sprint

### Blockers
- Depends on Story 2 (Quick Capture) being complete so there's data to display

---

## Progress Log

> Will be updated as work progresses

---

**Created:** 2026-01-04
**Last Updated:** 2026-01-04
**Story File:** `Meta/Sprints/Sprint-1/stories/story-3-applications-list.md`
**Dependencies:** Story 2 (Quick Capture) must be complete
