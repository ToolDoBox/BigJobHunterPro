# Story 3 Validation Guide - Applications List

**Story:** Applications List View (3 SP)
**Sprint:** 1
**Owner(s):** cadleta (Backend) + realemmetts (Frontend)
**Purpose:** Provide a checklist to verify Story 3 is complete and matches acceptance criteria.

---

## Backend Validation Checklist

### DTOs and Contracts
- [ ] `src/Application/DTOs/Applications/ApplicationListDto.cs` exists
- [ ] ApplicationListDto fields: Id, CompanyName, RoleTitle, Status, CreatedDate
- [ ] `src/Application/DTOs/Applications/ApplicationsListResponse.cs` exists
- [ ] ApplicationsListResponse fields: Items, Page, PageSize, HasMore

### Service Layer
- [ ] `src/Application/Interfaces/IApplicationService.cs` includes GetApplicationsAsync
- [ ] `src/Infrastructure/Services/ApplicationService.cs` implements GetApplicationsAsync
- [ ] Filters by current user (UserId from JWT)
- [ ] Orders by CreatedDate descending
- [ ] Uses projection to ApplicationListDto (no overfetching)
- [ ] Uses AsNoTracking for list queries
- [ ] Pagination uses page and pageSize, with hasMore calculated

### Controller Endpoint
- [ ] GET `/api/applications` exists in `src/WebAPI/Controllers/ApplicationsController.cs`
- [ ] Returns 200 with ApplicationsListResponse
- [ ] Returns 400 for invalid page or pageSize
- [ ] Returns 401 when not authenticated

### Integration Tests
- [ ] `tests/WebAPI.IntegrationTests/Controllers/ApplicationsControllerTests.cs` includes GET list tests
- [ ] Tests cover: empty list, newest-first ordering, user isolation, pagination, unauthenticated access

---

## Frontend Validation Checklist

### Route and Data Fetching
- [ ] `/app/applications` renders Applications page
- [ ] Calls GET `/api/applications` on load
- [ ] Handles pagination with "Load more"
- [ ] Refreshes list after Quick Capture success

### UI States
- [ ] Loading skeleton visible while fetching
- [ ] Empty state displays when list is empty
- [ ] Empty state CTA opens Quick Capture modal
- [ ] Error state shows retry option when API fails
- [ ] Error message shown if load-more fails

### Layout and Styling
- [ ] Desktop uses table layout (>=768px)
- [ ] Mobile uses stacked cards (<768px)
- [ ] Status badges color-coded by status
- [ ] Long company and role names truncate with ellipsis
- [ ] Retro-arcade styling consistent with existing theme
- [ ] Honors prefers-reduced-motion for loading animation

### Data Formatting
- [ ] Dates show relative time (e.g., "2 days ago")
- [ ] Dates older than 2 years show full date

---

## Manual Test Script

1. Start backend API (see `Meta/Sprints/Sprint-1/Validation.md` for setup).
2. Start frontend dev server (`bigjobhunterpro-web`).
3. Log in and navigate to `/app/applications`.
4. Verify empty state appears for a new user.
5. Click "LOG YOUR FIRST HUNT" to open Quick Capture.
6. Create a new application and confirm list refreshes with newest entry at top.
7. Create a second application and confirm order remains newest-first.
8. Resize viewport to <768px; confirm card layout.
9. Stop backend API and refresh list; confirm error state and retry button.
10. Restart backend API and retry; confirm list loads again.

---

## API Contract Snapshot

**GET /api/applications?page=1&pageSize=25**

Response:
```json
{
  "items": [
    {
      "id": "guid",
      "companyName": "string",
      "roleTitle": "string",
      "status": "Applied",
      "createdDate": "2026-01-04T10:30:00Z"
    }
  ],
  "page": 1,
  "pageSize": 25,
  "hasMore": false
}
```

---

## Sign-Off

**Verified By:** ______________________
**Date:** ______________________
**Status:** [ ] PASS / [ ] FAIL

**Notes:**
1.
2.
3.
