# Story 18: Conversion Funnel and Stage Timing

## User Story

**As a** job seeker
**I want** a conversion funnel and stage timing metrics
**so that** I can see where applications stall

- **Story Points:** 5
- **Priority:** High
- **Sprint:** 7
- **Assignee(s):** cadleta + realemmetts
- **Status:** Not Started

**Status Legend:** Not Started | In Progress | Completed | Blocked | Moved to Next Sprint

---

## Desired Outcome

Users can see a funnel from Applied to Offer and average time between stages.

---

## Acceptance Criteria (Definition of Done)

### Functional Requirements
- [ ] Funnel chart shows counts for each stage
- [ ] Average time between stages is displayed
- [ ] Metrics are computed per user and per party (if applicable)

### Non-Functional Requirements
- [ ] Metrics load in under 1 second for typical data sets
- [ ] Works on mobile and desktop

### Technical Requirements
- [ ] Requires status history or timeline events to compute timings
- [ ] Integration tests validate funnel calculation

### Content/UX Requirements
- [ ] Stage names match status labels used in the app
- [ ] Tooltips explain how timing is calculated

---

## Components and Elements

### Frontend Components
- ConversionFunnelChart
- StageTimingSummary

### Backend Components
- GET /api/analytics/funnel
- GET /api/analytics/stage-timing

### Database Changes
- None

---

## Task Breakdown

### Backend Tasks
- [ ] Add queries for stage counts and transitions - **Est:** 4h - **Assignee:** cadleta
- [ ] Add stage timing aggregation - **Est:** 3h - **Assignee:** cadleta
- [ ] Write integration tests - **Est:** 2h - **Assignee:** cadleta

### Frontend Tasks
- [ ] Build funnel visualization and timing summary - **Est:** 5h - **Assignee:** realemmetts
- [ ] Wire data fetching and error states - **Est:** 2h - **Assignee:** realemmetts

**Total Estimated Hours:** 16h

---

## System Interactions

- Depends on status updates and timeline events

---

## Edge Cases and Error Handling

### Edge Cases
- Applications with no status history beyond Applied

### Error Scenarios
- Missing timeline data results in partial metrics

---

## Testing Notes

### Manual Test Cases
1. Verify funnel counts match application statuses
2. Verify timing averages with known data

### Automation Coverage
- Integration: funnel endpoint returns counts for all stages

---

**Created:** 2026-01-05
**Last Updated:** 2026-01-05 by cadleta
**Story File:** `Meta/Sprints/Sprint-7/stories/story-18-conversion-funnel-stage-timing.md`
