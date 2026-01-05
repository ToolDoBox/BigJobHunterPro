# Sprint Management Quick Start Guide

Welcome to the new Agile sprint management system! This guide will get you up and running in 5 minutes.

## For cadleta (Dev A - Backend) and realemmetts (Dev B - Frontend)

---

## Daily Workflow

### Every Morning (5 minutes)

1. **Pull latest changes**
   ```bash
   git pull origin main
   ```

2. **Create your daily standup file**
   ```bash
   cp Meta/Sprints/templates/daily-standup-template.md "Meta/Sprints/Sprint-1/daily-standups/$(date +%Y-%m-%d)-yourname.md"

   # For cadleta:
   cp Meta/Sprints/templates/daily-standup-template.md "Meta/Sprints/Sprint-1/daily-standups/2026-01-05-cadleta.md"

   # For realemmetts:
   cp Meta/Sprints/templates/daily-standup-template.md "Meta/Sprints/Sprint-1/daily-standups/2026-01-05-realemmetts.md"
   ```

3. **Fill out your standup** (2 minutes)
   - What you completed yesterday
   - What you're working on today
   - Any blockers

4. **Commit and push**
   ```bash
   git add Meta/Sprints/Sprint-1/daily-standups/
   git commit -m "chore: daily standup for $(date +%Y-%m-%d)"
   git push
   ```

5. **Read your teammate's standup**
   - Open their file from yesterday
   - See what they're working on
   - Offer help if they have blockers

---

### Throughout the Day

1. **Update story tasks as you complete them**
   - Open the story file you're working on: `Meta/Sprints/Sprint-1/stories/story-1-authenticated-app-shell.md`
   - Find your task in the list
   - Change `[ ]` to `[x]` when complete
   - Change `[ ]` to `[~]` when in progress
   - Change `[ ]` to `[!]` if blocked

2. **Commit task updates**
   ```bash
   git add Meta/Sprints/Sprint-1/stories/
   git commit -m "feat: complete backend auth setup tasks (Story 1)"
   git push
   ```

3. **Communicate**
   - If you finish a major task, let your teammate know (Slack, Discord, etc.)
   - If you're blocked, update your standup file immediately
   - If you discover new tasks, add them to the story file

---

### End of Day (Optional but Recommended)

1. **Update burndown chart** (one person, rotating daily)
   - Open `Meta/Sprints/Sprint-1/burndown.md`
   - Scroll to bottom of "Daily Progress" section
   - Copy the template entry
   - Fill in today's completed stories and remaining SP
   - Commit and push

---

## Sprint 1 is Already Set Up!

You're currently in **Sprint 1** (2026-01-04 to 2026-01-17). Here's what's ready for you:

### Your Files

📂 **Meta/Sprints/Sprint-1/**
- `sprint-plan.md` - Sprint goals, dates, committed stories
- `burndown.md` - Daily progress tracker
- `stories/story-1-authenticated-app-shell.md` - **START HERE** - All your tasks for Story 1
- `stories/story-2-quick-capture.md` - Story 2 (start after Story 1)
- `stories/story-3-applications-list.md` - Story 3 (start after Story 2)
- `daily-standups/` - Your daily updates go here

### What to Do Right Now

1. **Read Story 1 file** (10 minutes)
   - Open `Meta/Sprints/Sprint-1/stories/story-1-authenticated-app-shell.md`
   - Read the entire file to understand the story
   - Find your tasks (search for your name: "cadleta" or "realemmetts")
   - Note the PHASE 0 sync point - you both need to review the API contract first!

2. **Create your first standup** (2 minutes)
   - Use the command above to create today's standup file
   - Fill it out with your plan for today

3. **Start working!**
   - Follow the task breakdown in Story 1
   - Check off tasks as you complete them
   - Commit and push regularly

---

## Git Best Practices

### Avoid Merge Conflicts

✅ **DO:**
- Edit only your own daily standup files
- Pull before editing any shared files
- Commit small, focused changes
- Push frequently

❌ **DON'T:**
- Edit your teammate's standup files
- Edit story files at the exact same time (coordinate in chat)
- Batch up many changes before committing

### File Ownership

- **Your standup files:** Only you edit (no conflicts!)
- **Story files:** Whoever is working on that task edits
- **Burndown:** One person updates daily (rotate)
- **Sprint plan:** Rarely edited after sprint starts

---

## Quick Reference

### Task Checkbox States
- `[ ]` - Not started
- `[~]` - In progress (currently working on it)
- `[x]` - Completed
- `[!]` - Blocked (needs help or external dependency)

### Story States
- ⬜ Not Started
- 🔄 In Progress
- ✅ Completed
- 🚫 Blocked
- 📦 Moved to Next Sprint

### Commit Message Format
- `feat:` - New feature (e.g., "feat: add login endpoint")
- `fix:` - Bug fix (e.g., "fix: correct JWT expiration")
- `refactor:` - Code refactor (e.g., "refactor: extract auth service")
- `docs:` - Documentation (e.g., "docs: update API docs")
- `chore:` - Daily tasks (e.g., "chore: daily standup")

---

## Where to Find Things

| What | Where |
|------|-------|
| Sprint goals and plan | `Meta/Sprints/Sprint-1/sprint-plan.md` |
| Story tasks and details | `Meta/Sprints/Sprint-1/stories/story-{N}-{name}.md` |
| Daily progress tracker | `Meta/Sprints/Sprint-1/burndown.md` |
| Your daily updates | `Meta/Sprints/Sprint-1/daily-standups/{date}-{yourname}.md` |
| Product backlog | `Meta/Sprints/backlog.md` |
| Templates for new files | `Meta/Sprints/templates/` |
| System documentation | `Meta/Sprints/README.md` |

---

## Tips for Success

### Communication
- Update your standup every day (even if "no changes")
- Read your teammate's standup every day
- If blocked, say so immediately (don't wait)
- Celebrate completed tasks in standups (🎉 Wins section)

### Task Management
- Break tasks down if they feel too big
- Estimate honestly (it's okay to be wrong)
- Ask for help if stuck for >2 hours
- Update task estimates if you discover new complexity

### Story Completion
- Don't mark story as complete until ALL acceptance criteria are met
- Test each other's work before marking complete
- Write tests as you go (don't save for end)
- Update documentation as you build

---

## FAQ

**Q: Do I need to update burndown every day?**
A: It's helpful but not required. One person can do it daily, or update it every 2-3 days.

**Q: What if I finish all my tasks early?**
A: Pick up tasks from the next story, help your teammate, or add polish tasks to current story.

**Q: What if I'm running behind?**
A: Update your standup with the blocker/challenge. Discuss with teammate about adjusting scope or moving stories to next sprint.

**Q: Can I add new tasks to a story?**
A: Yes! If you discover new work, add it to the task breakdown in the story file.

**Q: What if we disagree on an approach?**
A: Discuss in chat/call, document the decision in story file's "Notes & Decisions" section.

**Q: Do we need to fill out every section in the standup template?**
A: No, remove sections that aren't relevant for that day. Keep it quick!

---

## Example Daily Routine

### cadleta's Day (Backend Dev)

**9:00 AM** - Pull latest, create standup file, fill it out (5 min)
**9:05 AM** - Read realemmetts' standup from yesterday (2 min)
**9:07 AM** - Start coding on backend auth (Story 1, Task A3)
**11:30 AM** - Complete Task A3, update story file, commit (5 min)
**11:35 AM** - Start Task A4
**1:00 PM** - Lunch
**2:00 PM** - Continue Task A4
**4:00 PM** - Complete Task A4, update story file, commit (5 min)
**4:05 PM** - Start writing tests (Task A8)
**5:30 PM** - Update burndown for the day, commit, push (5 min)
**5:35 PM** - Done!

---

## Need Help?

- **System questions:** Read `Meta/Sprints/README.md`
- **Template questions:** Check `Meta/Sprints/templates/`
- **Sprint 1 questions:** Check `Meta/Sprints/Sprint-1/sprint-plan.md`
- **Story questions:** Check the specific story file
- **Still stuck:** Ask your teammate or check the Agile methodology in `Meta/Ref/Iterative Cycles.md`

---

## Let's Go! 🚀

You're all set! Start with Story 1, complete the PHASE 0 API contract review together, then dive into your tasks.

Remember:
- ✅ Daily standups (your own file, no conflicts)
- ✅ Update story tasks as you complete them
- ✅ Commit and push frequently
- ✅ Communicate blockers immediately
- ✅ Have fun building! 🎮

---

**Questions? Improvements?**
This system is a living process. If you find something that doesn't work, update it! Add notes to this file or the README.

Good hunting! 🎯

---

**Created:** 2026-01-04
**Your First Sprint:** Sprint 1 (2026-01-04 to 2026-01-17)
**Your First Task:** Read `Meta/Sprints/Sprint-1/stories/story-1-authenticated-app-shell.md`
