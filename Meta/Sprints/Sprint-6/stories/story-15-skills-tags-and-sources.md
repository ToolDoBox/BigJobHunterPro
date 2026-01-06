# Story 15: Skills Tagging and Source Tracking

## User Story

**As a** job seeker
**I want** to tag skills and track application sources
**so that** I can analyze which skills and sources perform best

- **Story Points:** 5
- **Priority:** High
- **Sprint:** 6
- **Assignee(s):** cadleta + realemmetts
- **Status:** Not Started

**Status Legend:** Not Started | In Progress | Completed | Blocked | Moved to Next Sprint

---

## Desired Outcome

Users can manage skills tags and sources for each application.

---

## Acceptance Criteria (Definition of Done)

### Functional Requirements
- [ ] Applications support multiple skill tags (required and optional)
- [ ] Application source can be selected from a defined list or free text
- [ ] Skills and source appear in list and detail views

### Non-Functional Requirements
- [ ] Tag selection works on mobile and desktop
- [ ] Source list is consistent across the app

### Technical Requirements
- [ ] Unit tests cover tag parsing and storage
- [ ] Integration tests cover source updates

### Content/UX Requirements
- [ ] Skill tags are visually distinct and readable
- [ ] Source labels match copy in analytics

---

## Components and Elements

### Frontend Components
- Skill tag input with multi-select
- Source dropdown or input

### Backend Components
- Update application skills endpoint
- Update application source endpoint

### Database Changes
- Skills table or JSON field on Application
- Source field (enum or string)

### External Dependencies
- Tag input component (existing or custom)

---

## Task Breakdown

### Backend Tasks
- [ ] Add skill and source fields to data model - **Est:** 3h - **Assignee:** cadleta
- [ ] Add endpoints for skill and source updates - **Est:** 2h - **Assignee:** cadleta
- [ ] Write tests for skill and source updates - **Est:** 2h - **Assignee:** cadleta

### Frontend Tasks
- [ ] Build skill tag input UI - **Est:** 4h - **Assignee:** realemmetts
- [ ] Build source selector UI - **Est:** 2h - **Assignee:** realemmetts
- [ ] Wire data fetching and save states - **Est:** 2h - **Assignee:** realemmetts

### QA and Polish
- [ ] Validate tag performance for 20+ skills - **Est:** 1h - **Assignee:** realemmetts

**Total Estimated Hours:** 16h

---

## System Interactions

- Skill and source data feeds analytics in Sprint 7

---

## Edge Cases and Error Handling

### Edge Cases
- Duplicate tags
- Empty source value

### Error Scenarios
- Validation error for unsupported source

---

## Testing Notes

### Manual Test Cases
1. Add multiple skill tags and confirm persistence
2. Change source and confirm list view update

### Automation Coverage
- Unit: Skill tag serialization
- Integration: Source update endpoint

---

**Created:** 2026-01-05
**Last Updated:** 2026-01-05 by cadleta
**Story File:** `Meta/Sprints/Sprint-6/stories/story-15-skills-tags-and-sources.md`
