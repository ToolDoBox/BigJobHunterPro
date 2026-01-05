# Sprint {N} Burndown Chart

## Sprint Overview

- **Total Story Points:** {X} SP
- **Sprint Duration:** {Y} days
- **Start Date:** YYYY-MM-DD
- **End Date:** YYYY-MM-DD
- **Ideal Daily Burn Rate:** {X/Y} SP per day

---

## Daily Progress (Append-Only)

> **Instructions:** Add a new entry at the end of each day. DO NOT edit previous entries. This makes git merges easy.

### Template Entry
```markdown
#### YYYY-MM-DD (Day X of Y)

**Stories Completed Today:**
- Story {ID}: {Story Name} - {SP} SP ✅

**Remaining Story Points:** {X} SP
**Ideal Remaining:** {Y} SP
**Status:** 🟢 On Track | 🟡 At Risk | 🔴 Behind Schedule

**Notes:**
- {Any significant events, blockers added/removed, scope changes}
```

---

#### {Start Date} (Day 0 - Sprint Start)

**Stories Committed:** {X} stories, {Y} SP total
**Remaining Story Points:** {Y} SP
**Ideal Remaining:** {Y} SP
**Status:** 🟢 Sprint Started

**Sprint Backlog:**
- Story 1: {Name} - {SP} SP - ⬜ Not Started
- Story 2: {Name} - {SP} SP - ⬜ Not Started
- Story 3: {Name} - {SP} SP - ⬜ Not Started

**Notes:**
- Sprint planning completed
- Stories broken down into tasks
- Initial velocity target: {X} SP

---

#### YYYY-MM-DD (Day 1)

**Stories Completed Today:**
- None (setup/planning day)

**Remaining Story Points:** {X} SP
**Ideal Remaining:** {Y} SP
**Status:** 🟢 On Track

**Notes:**
- Team ramping up on new stories
- Dev environment setup complete

---

#### YYYY-MM-DD (Day 2)

**Stories Completed Today:**
- Story 1: {Story Name} - {SP} SP ✅

**Remaining Story Points:** {X} SP
**Ideal Remaining:** {Y} SP
**Status:** 🟢 On Track

**Notes:**
- First story delivered ahead of schedule
- Dev 1 picked up Story 2

---

#### YYYY-MM-DD (Day 3)

**Stories Completed Today:**
- None (work in progress on Story 2)

**Remaining Story Points:** {X} SP
**Ideal Remaining:** {Y} SP
**Status:** 🟢 On Track

**Notes:**
- Story 2 is 70% complete (expect completion tomorrow)
- Discovered technical debt in auth module - added tech debt story to backlog

---

## Mid-Sprint Checkpoint (Day {X})

**Date:** YYYY-MM-DD
**Completed SP:** {X} / {Total} ({Y}%)
**Remaining SP:** {X} SP
**Days Remaining:** {X}

### Velocity Analysis
- **Actual Daily Velocity:** {X} SP/day
- **Required Velocity to Finish:** {Y} SP/day
- **Assessment:** {On track to complete all committed stories | Need to descope | Add stretch stories}

### Risks & Adjustments
- {Risk 1: Story 4 more complex than estimated - may need +2 SP}
- {Adjustment 1: Moving Story 5 to next sprint to focus on core goals}

---

## Burndown Data (For Charting)

| Day | Date | Ideal Remaining | Actual Remaining | Completed Today | Notes |
|-----|------|----------------|------------------|-----------------|-------|
| 0 | {Date} | {X} | {X} | 0 | Sprint start |
| 1 | {Date} | {X} | {X} | 0 | Setup day |
| 2 | {Date} | {X} | {X} | {Y} | Story 1 done |
| 3 | {Date} | {X} | {X} | 0 | In progress |
| 4 | {Date} | {X} | {X} | {Y} | Story 2 done |
| ... | ... | ... | ... | ... | ... |

> **Tip:** Paste this table into a spreadsheet or charting tool to visualize the burndown.

---

## Sprint Scope Changes

### Added Stories
- **Date:** YYYY-MM-DD
- **Story:** {Story Name} - {SP} SP
- **Reason:** {Critical bug found | Customer request | Dependency discovered}
- **Impact:** +{X} SP to sprint

### Removed Stories
- **Date:** YYYY-MM-DD
- **Story:** {Story Name} - {SP} SP
- **Reason:** {Deprioritized | Blocked | Moved to next sprint}
- **Impact:** -{X} SP from sprint

### Estimate Adjustments
- **Date:** YYYY-MM-DD
- **Story:** {Story Name}
- **Original:** {X} SP → **Revised:** {Y} SP
- **Reason:** {Underestimated complexity | Simpler than expected | Found existing solution}

---

## Final Sprint Summary

**Date:** {End Date}
**Completed Story Points:** {X} / {Committed} ({Y}%)
**Velocity This Sprint:** {X} SP
**Stories Completed:** {X} / {Y}
**Stories Moved to Next Sprint:** {X}

### Burndown Chart Summary
- **Trend:** {Steady burn | Front-loaded | Back-loaded | Erratic}
- **Comparison to Ideal:** {Matched closely | Ahead of schedule | Behind schedule}
- **Contributing Factors:** {Good estimation | Strong collaboration | Unexpected blockers}

**Velocity for Next Sprint:** {Recommended SP based on this sprint's actual velocity}

---

## Git-Friendly Usage

### Daily Update Process
1. **Pull latest changes:** `git pull origin main`
2. **Scroll to bottom of this file**
3. **Copy the template entry**
4. **Fill in today's data**
5. **Paste at the bottom (append-only!)**
6. **Commit:** `git add burndown.md && git commit -m "chore: update burndown for Day X"`
7. **Push:** `git push origin main`

### Why Append-Only?
- Multiple developers can update on the same day without conflicts
- Git will merge chronological entries automatically
- Historical data is never overwritten
- Easy to see progress over time

### Alternative: One Update Per Day
- Designate one person (Scrum Master, rotating daily) to update burndown
- Others update their individual standups and story files
- Reduces chance of any merge conflicts

---

**Created:** YYYY-MM-DD
**Last Updated:** YYYY-MM-DD
