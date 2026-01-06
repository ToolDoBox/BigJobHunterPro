# Story 17: Analytics Dashboard Charts

## User Story

**As a** job seeker
**I want** an analytics dashboard with core charts
**so that** I can understand my progress at a glance

- **Story Points:** 5
- **Priority:** High
- **Sprint:** 7
- **Assignee(s):** cadleta + realemmetts
- **Status:** Not Started

**Status Legend:** Not Started | In Progress | Completed | Blocked | Moved to Next Sprint

---

## Desired Outcome

Users can view applications over time and response rate charts.

---

## Acceptance Criteria (Definition of Done)

### Functional Requirements
- [ ] Dashboard shows applications per week chart
- [ ] Dashboard shows response rate chart (interview or offer rate)
- [ ] Charts update when data changes

### Non-Functional Requirements
- [ ] Dashboard loads in under 1.5 seconds for typical data sets
- [ ] Charts render on mobile and desktop

### Technical Requirements
- [ ] API endpoint provides aggregated metrics
- [ ] Frontend chart components have unit tests

### Content/UX Requirements
- [ ] Chart labels and tooltips explain metrics clearly

---

## Components and Elements

### Frontend Components
- AnalyticsDashboard page
- ApplicationsPerWeekChart
- ResponseRateChart

### Backend Components
- GET /api/analytics/summary

### Database Changes
- None

---

## Task Breakdown

### Backend Tasks
- [ ] Add analytics summary endpoint - **Est:** 3h - **Assignee:** cadleta
- [ ] Add query for weekly counts and response rate - **Est:** 3h - **Assignee:** cadleta

### Frontend Tasks
- [ ] Build dashboard layout and charts - **Est:** 5h - **Assignee:** realemmetts
- [ ] Wire data fetching and error states - **Est:** 2h - **Assignee:** realemmetts

### QA and Polish
- [ ] Verify charts with sample data - **Est:** 1h - **Assignee:** realemmetts

**Total Estimated Hours:** 14h

---

## System Interactions

- Consumes skills and source data for cross-links in Story 19

---

## Edge Cases and Error Handling

### Edge Cases
- No applications logged yet

### Error Scenarios
- Analytics endpoint returns empty data set

---

## Testing Notes

### Manual Test Cases
1. Open dashboard with no data and verify empty state
2. Open dashboard with data and verify chart labels

### Automation Coverage
- Unit: Response rate calculation
- Integration: GET /api/analytics/summary

---

**Created:** 2026-01-05
**Last Updated:** 2026-01-05 by cadleta
**Story File:** `Meta/Sprints/Sprint-7/stories/story-17-analytics-dashboard.md`
