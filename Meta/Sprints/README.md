# Sprint Management System

## Overview

This directory contains our Agile sprint management system designed for 2-developer teams with git-friendly practices to minimize merge conflicts.

## Directory Structure

```
Meta/Sprints/
├── README.md                    # This file - system overview
├── backlog.md                   # Product backlog (all pending user stories)
├── Sprint-{N}/                  # One folder per sprint
│   ├── sprint-plan.md           # Sprint goals, dates, capacity, selected stories
│   ├── burndown.md              # Daily story point tracking (append-only)
│   ├── stories/                 # Individual user story task breakdowns
│   │   ├── story-{ID}-{name}.md # One file per story
│   │   └── ...
│   ├── daily-standups/          # Individual developer updates
│   │   ├── YYYY-MM-DD-{dev-name}.md
│   │   └── ...
│   └── sprint-review.md         # Created at sprint end (retrospective)
└── templates/                   # Templates for creating new files
    ├── sprint-plan-template.md
    ├── story-template.md
    └── daily-standup-template.md
```

## Git-Friendly Design Principles

### 1. **File Separation by Developer**
- Each developer has their own daily standup files (`YYYY-MM-DD-{dev-name}.md`)
- Prevents concurrent edits to the same file
- Use your name or initials consistently (e.g., `dev1`, `christian`, `john`)

### 2. **Append-Only Files**
- `burndown.md` uses append-only daily entries
- New entries always go at the bottom
- Minimizes merge conflicts

### 3. **Story-Based Isolation**
- Each user story gets its own file in `stories/`
- Developers can work on different stories simultaneously
- Story files are edited by whoever is working on that story

### 4. **Timestamped Entries**
- Daily standups use date-based filenames
- Easy to identify and merge chronological updates

## Workflow

### Starting a New Sprint

1. **Create Sprint Directory**
   ```bash
   mkdir -p "Meta/Sprints/Sprint-{N}/stories"
   mkdir -p "Meta/Sprints/Sprint-{N}/daily-standups"
   ```

2. **Copy and Fill Sprint Plan**
   ```bash
   cp Meta/Sprints/templates/sprint-plan-template.md "Meta/Sprints/Sprint-{N}/sprint-plan.md"
   # Edit sprint-plan.md with goals, dates, selected stories
   ```

3. **Create Story Files**
   ```bash
   # For each selected story:
   cp Meta/Sprints/templates/story-template.md "Meta/Sprints/Sprint-{N}/stories/story-{ID}-{name}.md"
   # Fill in completion criteria and task breakdown
   ```

4. **Initialize Burndown**
   - `burndown.md` starts with sprint capacity
   - Update daily with remaining story points

### Daily Workflow

1. **Morning Standup** (each developer creates their own file)
   ```bash
   cp Meta/Sprints/templates/daily-standup-template.md "Meta/Sprints/Sprint-{N}/daily-standups/2026-01-04-yourname.md"
   ```
   - Fill in: Yesterday's work, Today's plan, Blockers

2. **Update Story Tasks** (throughout the day)
   - Open the story file you're working on
   - Check off completed tasks
   - Add new tasks if discovered
   - Update assignee if switching work

3. **End of Day** (update burndown)
   - Append a new daily entry to `burndown.md`
   - Calculate remaining story points for incomplete stories
   - Commit and push changes

### Ending a Sprint

1. **Complete Sprint Review**
   ```bash
   cp Meta/Sprints/templates/sprint-review-template.md "Meta/Sprints/Sprint-{N}/sprint-review.md"
   ```
   - Review completed vs. planned story points
   - Document successes and challenges
   - Identify improvements for next sprint

2. **Update Backlog**
   - Move incomplete stories back to `backlog.md`
   - Add new stories discovered during sprint
   - Re-prioritize based on learnings

## Merge Conflict Prevention

### Best Practices

1. **Pull before editing**
   ```bash
   git pull origin main
   ```

2. **Edit only your assigned files**
   - Your daily standup files
   - Stories you're actively working on
   - Burndown (append only at end of day)

3. **Commit frequently with clear messages**
   ```bash
   git add Meta/Sprints/Sprint-1/stories/story-1-auth-shell.md
   git commit -m "feat: complete backend auth setup tasks (Story 1)"
   ```

4. **Communicate on shared files**
   - If two devs need to edit the same story, communicate first
   - Use feature branches for major story work
   - Merge frequently

### If Conflicts Occur

For append-only files (burndown, standups):
- Accept both changes
- Ensure chronological order
- Most git tools handle this automatically

For story task files:
- Coordinate with team member
- Accept both task updates
- Ensure checkbox states are correct

## Quick Reference

### Story Point Estimation Scale
- **1 point:** Simple task, <2 hours, no unknowns
- **2 points:** Straightforward task, 2-4 hours
- **3 points:** Moderate complexity, half day
- **5 points:** Complex task, full day, some unknowns
- **8 points:** Very complex, 2 days, many unknowns
- **13+ points:** Too large - break down into smaller stories

### Story States
- `⬜ Not Started` - Story in sprint backlog
- `🔄 In Progress` - At least one task started
- `✅ Completed` - All completion criteria met
- `🚫 Blocked` - Cannot proceed, needs resolution
- `📦 Moved to Next Sprint` - Incomplete, carried over

### Task States (within stories)
- `[ ]` - Not started
- `[x]` - Completed
- `[~]` - In progress
- `[!]` - Blocked

## Tools Integration

### VS Code Extensions (Optional)
- **Markdown All in One:** Better markdown editing
- **Markdown Checkboxes:** Quick checkbox toggling
- **GitLens:** See who edited what and when

### Git Aliases (Optional)
```bash
# Add to .gitconfig
[alias]
  sprint-status = !git diff --name-only HEAD origin/main | grep "Meta/Sprints"
  standup = !git log --since='1 day ago' --author='$(git config user.name)' --oneline
```

## Getting Started

1. Read this file
2. Review `templates/` directory
3. Check `backlog.md` for available user stories
4. Start Sprint 1 using the workflow above
5. Customize templates to fit your team's needs

---

**Remember:** The system works best when:
- Each developer owns their files (standups, assigned tasks)
- Updates are frequent and small
- Communication is clear on shared work
- Git pull happens before every edit session
