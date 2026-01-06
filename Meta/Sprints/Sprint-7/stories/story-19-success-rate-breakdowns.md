# Story 19: Success Rate Breakdowns

## User Story

**As a** job seeker
**I want** success rate breakdowns by source, skill, and company size
**so that** I can focus on what works best

- **Story Points:** 3
- **Priority:** Medium
- **Sprint:** 7
- **Assignee(s):** cadleta + realemmetts
- **Status:** Not Started

**Status Legend:** Not Started | In Progress | Completed | Blocked | Moved to Next Sprint

---

## Desired Outcome

Users can compare success rates across sources, skills, and company size.

---

## Acceptance Criteria (Definition of Done)

### Functional Requirements
- [ ] Success rate by source is displayed
- [ ] Success rate by skill is displayed
- [ ] Success rate by company size is displayed

### Non-Functional Requirements
- [ ] Metrics use consistent definitions across charts
- [ ] Tables render correctly on mobile and desktop

### Technical Requirements
- [ ] Aggregations are cached for 5 minutes
- [ ] Unit tests cover calculation logic

### Content/UX Requirements
- [ ] Metrics include short definitions and tooltips

---

## Components and Elements

### Frontend Components
- SuccessRateBySourceTable
- SuccessRateBySkillTable
- SuccessRateByCompanySizeTable

### Backend Components
- GET /api/analytics/success-rate

### Database Changes
- None

---

## Task Breakdown

### Backend Tasks
- [ ] Implement success rate aggregation by dimension - **Est:** 4h - **Assignee:** cadleta
- [ ] Add caching for aggregate queries - **Est:** 2h - **Assignee:** cadleta
- [ ] Write unit tests - **Est:** 2h - **Assignee:** cadleta

### Frontend Tasks
- [ ] Build tables and summary cards - **Est:** 4h - **Assignee:** realemmetts
- [ ] Wire data fetching and tooltips - **Est:** 2h - **Assignee:** realemmetts

**Total Estimated Hours:** 14h

---

## System Interactions

- Consumes skill tags and sources from Sprint 6

---

## Edge Cases and Error Handling

### Edge Cases
- No data for a specific dimension

### Error Scenarios
- Aggregation returns empty data set

---

## Testing Notes

### Manual Test Cases
1. Verify sources list matches application sources
2. Verify company size buckets are correct

### Automation Coverage
- Unit: success rate calculation per dimension

---

**Created:** 2026-01-05
**Last Updated:** 2026-01-05 by cadleta
**Story File:** `Meta/Sprints/Sprint-7/stories/story-19-success-rate-breakdowns.md`
