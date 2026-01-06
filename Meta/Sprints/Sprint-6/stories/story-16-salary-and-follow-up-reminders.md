# Story 16: Salary Expectations and Follow-Up Reminders

## User Story

**As a** job seeker
**I want** to track salary expectations and follow-up reminders
**so that** I can stay organized during the pipeline

- **Story Points:** 3
- **Priority:** Medium
- **Sprint:** 6
- **Assignee(s):** cadleta + realemmetts
- **Status:** Not Started

**Status Legend:** Not Started | In Progress | Completed | Blocked | Moved to Next Sprint

---

## Desired Outcome

Users can record salary expectations and a follow-up date per application.

---

## Acceptance Criteria (Definition of Done)

### Functional Requirements
- [ ] Applications store salaryMin, salaryMax, and currency
- [ ] Applications store a follow-up reminder date
- [ ] Detail view shows salary and reminder fields with edit support

### Non-Functional Requirements
- [ ] Inputs validate numeric ranges and date format
- [ ] Works on mobile, tablet, and desktop

### Technical Requirements
- [ ] Unit tests for salary validation
- [ ] Integration test for reminder update endpoint

### Content/UX Requirements
- [ ] Salary fields show helper text for expected format
- [ ] Reminder date displays in local time

---

## Components and Elements

### Frontend Components
- Salary range inputs
- Follow-up date picker

### Backend Components
- Salary and reminder fields on Application
- PATCH /api/applications/{id}/tracking

### Database Changes
- Add SalaryMin, SalaryMax, SalaryCurrency, FollowUpDate columns

---

## Task Breakdown

### Backend Tasks
- [ ] Add salary and reminder fields to entity and migration - **Est:** 2h - **Assignee:** cadleta
- [ ] Add update endpoint and validation - **Est:** 2h - **Assignee:** cadleta
- [ ] Write tests for validation and updates - **Est:** 2h - **Assignee:** cadleta

### Frontend Tasks
- [ ] Build salary and reminder inputs - **Est:** 3h - **Assignee:** realemmetts
- [ ] Wire updates and error states - **Est:** 2h - **Assignee:** realemmetts

**Total Estimated Hours:** 11h

---

## System Interactions

- Reminder date can be used for future notification features

---

## Edge Cases and Error Handling

### Edge Cases
- Salary min greater than max
- Reminder date in the past

### Error Scenarios
- Validation error returns clear message

---

## Testing Notes

### Manual Test Cases
1. Enter salary range and confirm validation
2. Set a reminder date and confirm persistence

### Automation Coverage
- Unit: Salary range validation
- Integration: Update tracking endpoint

---

**Created:** 2026-01-05
**Last Updated:** 2026-01-05 by cadleta
**Story File:** `Meta/Sprints/Sprint-6/stories/story-16-salary-and-follow-up-reminders.md`
