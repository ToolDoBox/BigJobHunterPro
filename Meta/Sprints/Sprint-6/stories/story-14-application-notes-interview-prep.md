# Story 14: Application Notes and Interview Prep Notes

## User Story

**As a** job seeker
**I want** to capture notes for each application
**so that** I can track interview preparation and context

- **Story Points:** 5
- **Priority:** High
- **Sprint:** 6
- **Assignee(s):** cadleta + realemmetts
- **Status:** Not Started

**Status Legend:** Not Started | In Progress | Completed | Blocked | Moved to Next Sprint

---

## Desired Outcome

Users can add and edit notes per application, including interview prep notes.

---

## Acceptance Criteria (Definition of Done)

### Functional Requirements
- [ ] Notes are stored per application and editable
- [ ] Interview prep notes are a dedicated section
- [ ] Notes are visible in the application detail view

### Non-Functional Requirements
- [ ] Notes save without page refresh
- [ ] Works on mobile, tablet, and desktop

### Technical Requirements
- [ ] Unit tests for notes service
- [ ] Integration tests for notes API endpoints

### Content/UX Requirements
- [ ] Clear labels for general notes vs interview prep
- [ ] Empty state hints when no notes exist

---

## Components and Elements

### Frontend Components
- Notes panel in application detail view
- Interview prep notes textarea

### Backend Components
- ApplicationNotes fields on Application entity
- Notes update endpoint (PATCH /api/applications/{id}/notes)

### Database Changes
- Add Notes and InterviewPrepNotes columns

### External Dependencies
- None

---

## Task Breakdown

### Backend Tasks
- [ ] Add notes fields to entity and migration - **Est:** 2h - **Assignee:** cadleta
- [ ] Add notes update endpoint - **Est:** 2h - **Assignee:** cadleta
- [ ] Write tests for notes updates - **Est:** 2h - **Assignee:** cadleta

### Frontend Tasks
- [ ] Build notes UI in detail view - **Est:** 4h - **Assignee:** realemmetts
- [ ] Wire save interactions and loading states - **Est:** 2h - **Assignee:** realemmetts

### QA and Polish
- [ ] Verify auto-save or explicit save UX - **Est:** 1h - **Assignee:** realemmetts

**Total Estimated Hours:** 13h

---

## System Interactions

- Notes can influence analytics in Sprint 7 when content is parsed later

---

## Edge Cases and Error Handling

### Edge Cases
- Notes exceed maximum length

### Error Scenarios
- Save fails due to validation or network error

---

## Testing Notes

### Manual Test Cases
1. Add notes and confirm persistence after refresh
2. Add interview prep notes and verify display

### Automation Coverage
- Integration: PATCH notes returns updated payload

---

**Created:** 2026-01-05
**Last Updated:** 2026-01-05 by cadleta
**Story File:** `Meta/Sprints/Sprint-6/stories/story-14-application-notes-interview-prep.md`
