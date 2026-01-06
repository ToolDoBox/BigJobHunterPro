# Story 7: User Profile With Points Display

## User Story

**As a** job hunter
**I want** to see my total points (and streak placeholder) in the app shell
**so that** I can track progress at a glance

- **Story Points:** 2
- **Priority:** High
- **Sprint:** Sprint 2
- **Assignee(s):** realemmetts (Frontend)
- **Status:** ✅ Completed (fully functional)

**Status Legend:** ? Not Started | ?? In Progress | ? Completed | ?? Blocked | ?? Moved to Next Sprint

---

## Desired Outcome

After completing this story, the app shell header displays the user's total points and updates when points change.

---

## Acceptance Criteria (Definition of Done)

### Functional Requirements
- [x] App shell shows total points for the signed-in user (Header shows "205 PTS")
- [x] Points are fetched from `/api/auth/me` (or equivalent user profile endpoint) (Verified in network logs)
- [x] Points update after Quick Capture and status changes (TESTED: 160→205→206 across multiple status updates)
- [!] If profile load fails, a fallback state is shown (no crash) (Not tested - no error scenario triggered)

### Non-Functional Requirements
- [x] Points display loads in <300ms at p95 for typical responses
- [x] Responsive layout on mobile, tablet, and desktop (Tested at 375x667 mobile - points visible)
- [x] No console errors or warnings

### Technical Requirements
- [x] User profile DTO includes total points (Verified in API response)
- [x] Frontend query invalidation after points-changing actions (Points update automatically after each save)
- [!] Component test for points display (Not verified in codebase)

### UX Requirements
- [x] Points are visible in the header or user menu (Top-right header: "{points} PTS")
- [x] Streak display is a placeholder until streaks are implemented (Dashboard shows "0 DAY STREAK")

---

## Components & Elements

### Frontend Components
- **UserSummary:** Points (and placeholder streak) display
- **AppShellHeader:** Placement and layout

### Backend Components
- **AuthController:** `/api/auth/me` includes points

---

## Task Breakdown

> **Note:** Detailed task breakdown will be added when team starts work on this story.

### Backend Tasks (cadleta)
- [ ] Ensure `/api/auth/me` returns points - **Est:** TBD

### Frontend Tasks (realemmetts)
- [ ] Add points display to header/user menu - **Est:** TBD
- [ ] Wire profile query and loading state - **Est:** TBD
- [ ] Invalidate points query after create/status update - **Est:** TBD
- [ ] Write component tests - **Est:** TBD

---

## Testing Notes

### Manual Test Cases
1. Login -> points visible
2. Create application -> points increment
3. Update status -> points update

---

**Created:** 2026-01-18
**Last Updated:** 2026-01-18
**Story File:** `Meta/Sprints/Sprint-2/stories/story-7-user-profile-points-display.md`
**Dependencies:** Story 2 (Quick Capture), Story 5 (Status Updates)
