# Sprint {N} Review & Retrospective

**Sprint Number:** {N}
**Start Date:** YYYY-MM-DD
**End Date:** YYYY-MM-DD
**Duration:** {X} days ({Y} weeks)
**Review Date:** YYYY-MM-DD
**Attendees:** {Dev 1}, {Dev 2}, {Product Owner}, {Stakeholders}

---

## Sprint Goals Review

### Sprint Goal
> {Original sprint goal from sprint-plan.md}

**Goal Achievement:** ✅ Fully Met | ⚠️ Partially Met | ❌ Not Met

**Explanation:**
{1-2 sentences on why the goal was/wasn't met}

---

## Story Points & Velocity

### Committed vs. Completed

| Metric | Planned | Actual | Variance |
|--------|---------|--------|----------|
| Story Points Committed | {X} SP | {Y} SP | {+/- Z} SP |
| Stories Committed | {X} | {Y} | {+/- Z} |
| Story Points Completed | - | {Y} SP | - |
| Stories Completed | - | {Y} | - |
| **Completion Rate** | - | **{Y}%** | - |

### Velocity Trend

| Sprint | Committed SP | Completed SP | Velocity | Notes |
|--------|-------------|--------------|----------|-------|
| Sprint {N-2} | {X} | {Y} | {Y} SP | {Context} |
| Sprint {N-1} | {X} | {Y} | {Y} SP | {Context} |
| **Sprint {N}** | **{X}** | **{Y}** | **{Y} SP** | **Current** |

**Velocity Assessment:**
- **Trend:** {Increasing | Stable | Decreasing}
- **Recommended Capacity for Next Sprint:** {X} SP
- **Confidence Level:** {High | Medium | Low}

---

## Completed Stories

### ✅ Stories Delivered

#### Story {ID}: {Story Name} - {SP} SP
- **Assignee(s):** {Dev name(s)}
- **Completion Date:** YYYY-MM-DD
- **Demo:** {Link to demo video/screenshots OR brief description}
- **Outcome:** {What value this delivers to users}
- **Highlights:** {What went particularly well}

#### Story {ID}: {Story Name} - {SP} SP
- **Assignee(s):** {Dev name(s)}
- **Completion Date:** YYYY-MM-DD
- **Demo:** {Description}
- **Outcome:** {Value delivered}

---

## Incomplete Stories

### 📦 Moved to Next Sprint

#### Story {ID}: {Story Name} - {SP} SP (Original)
- **Completion:** {X}% done
- **Remaining Work:** {What's left to finish}
- **Reason Not Completed:** {Underestimated | Blocker | Deprioritized}
- **Revised Estimate:** {Y} SP for remaining work
- **Next Sprint Priority:** {High | Medium | Low}

---

## What Went Well ✅

Based on Iterative Cycles methodology: "What went well during that sprint?"

### Team Performance
1. {Positive 1: Strong collaboration on Story X led to early completion}
2. {Positive 2: Daily standups kept everyone aligned}
3. {Positive 3: Pairing session resolved tricky bug quickly}

### Technical Wins
1. {Win 1: New CI/CD pipeline reduced deployment time by 50%}
2. {Win 2: Refactored auth module now easier to test}
3. {Win 3: Improved DB query performance in Story X}

### Process Improvements
1. {Improvement 1: Breaking stories into smaller tasks helped with progress visibility}
2. {Improvement 2: Using separate standup files eliminated merge conflicts}

### Individual Highlights
- **{Dev 1}:** {Accomplishment or contribution}
- **{Dev 2}:** {Accomplishment or contribution}

---

## Challenges Faced 🚧

Based on Iterative Cycles methodology: "What are some of the challenges that you ran into?"

### Technical Challenges
1. {Challenge 1: API integration more complex than expected}
   - **Impact:** Delayed Story 2 by 1 day
   - **Resolution:** Paired with senior dev, got unblocked
   - **Learning:** Always check API docs before estimating

2. {Challenge 2: Flaky tests in CI pipeline}
   - **Impact:** Slowed down PR merges
   - **Resolution:** Added retries, investigating root cause

### Process Challenges
1. {Challenge 1: Unclear acceptance criteria on Story 3}
   - **Impact:** Rework needed after initial implementation
   - **Resolution:** Added more detailed criteria template

2. {Challenge 2: Mid-sprint scope change}
   - **Impact:** Added 3 SP, had to descope other story
   - **Resolution:** Communicated trade-offs, got stakeholder buy-in

### External Blockers
1. {Blocker 1: Waiting 2 days for design assets}
   - **Impact:** Couldn't start frontend work on Story 4
   - **Mitigation:** Started backend work earlier

---

## Three Things to Change for Next Sprint 🔄

Based on Iterative Cycles methodology: "What are 3 things that you would like to change for the next sprint?"

### Improvement 1: {Change Title}
- **Current Problem:** {What's not working}
- **Proposed Solution:** {How to fix it}
- **Owner:** {Who will implement this}
- **Success Metric:** {How we'll know it worked}

### Improvement 2: {Change Title}
- **Current Problem:** {What's not working}
- **Proposed Solution:** {How to fix it}
- **Owner:** {Who will implement this}
- **Success Metric:** {How we'll know it worked}

### Improvement 3: {Change Title}
- **Current Problem:** {What's not working}
- **Proposed Solution:** {How to fix it}
- **Owner:** {Who will implement this}
- **Success Metric:** {How we'll know it worked}

---

## Learnings & Insights 💡

### Technical Learnings
- {Learning 1: Discovered new library X that simplifies Y}
- {Learning 2: Learned that Z pattern doesn't work well for our use case}

### Estimation Accuracy
- **Overestimated:** {Story types we tend to overestimate}
- **Underestimated:** {Story types we tend to underestimate}
- **Adjustment:** {How to improve estimates next sprint}

### Team Dynamics
- {Insight 1: Pairing works better for complex stories}
- {Insight 2: Async standups work well for our team}

---

## Quality Metrics

### Code Quality
- **Code Reviews:** {X} PRs reviewed, avg review time {Y} hours
- **Test Coverage:** {X}% (Target: {Y}%)
- **Bugs Found:** {X} (before deployment), {Y} (after deployment)
- **Technical Debt:** {Added | Reduced | Neutral}

### Performance
- **Lighthouse Scores:** {Average or specific page scores}
- **API Response Times:** {Average or endpoint-specific}
- **Build Times:** {X} minutes (Previous: {Y} minutes)

### Accessibility
- **WCAG Compliance:** {AA | AAA | Issues found}
- **Screen Reader Testing:** {Done | Not Done | Partially}
- **Keyboard Navigation:** {Fully functional | Issues noted}

---

## Stakeholder Feedback

### Demos Conducted
- {Demo 1: Login flow demo to Product Owner}
  - **Feedback:** {Positive comments, change requests}
  - **Action Items:** {What we'll adjust based on feedback}

### User Testing (if applicable)
- {Test 1: 5 users tested quick capture flow}
  - **Results:** {Success rate, time to complete, pain points}
  - **Action Items:** {Changes to make}

---

## Sprint Artifacts Delivered

### Deployed Features
- {Feature 1: User authentication (staging)}
- {Feature 2: Application quick capture (production)}

### Documentation Updated
- {Doc 1: API documentation for auth endpoints}
- {Doc 2: User guide for quick capture feature}

### Technical Debt Addressed
- {Debt 1: Refactored auth module for testability}
- {Debt 2: Upgraded to React 18}

---

## Action Items for Next Sprint

### From This Retrospective
- [ ] {Action 1: Implement Improvement 1 above} - **Owner:** {Dev}
- [ ] {Action 2: Schedule design review earlier in sprint} - **Owner:** {Dev}
- [ ] {Action 3: Add more detailed acceptance criteria} - **Owner:** {Team}

### Carry-Over Work
- [ ] {Task 1: Complete remaining 30% of Story X} - **Owner:** {Dev}
- [ ] {Task 2: Address feedback from Demo Y} - **Owner:** {Dev}

### Technical Debt
- [ ] {Debt 1: Fix flaky tests in CI} - **Owner:** {Dev}
- [ ] {Debt 2: Investigate performance issue in Z} - **Owner:** {Dev}

---

## Celebration & Recognition 🎉

**Sprint Achievements:**
- {Achievement 1: Delivered first production feature!}
- {Achievement 2: Maintained 100% uptime in staging}
- {Achievement 3: Completed all committed stories for first time}

**Individual Recognition:**
- {Dev 1}: {What they did exceptionally well}
- {Dev 2}: {What they did exceptionally well}

**Team Wins:**
- {Win 1: Best collaboration we've had}
- {Win 2: Fastest story completion time}

---

## Backlog Updates

### New Stories Created
- {Story: Bug fix for X} - {SP} SP - **Priority:** {High}
- {Story: Enhancement for Y} - {SP} SP - **Priority:** {Medium}

### Re-Prioritized Stories
- {Story X}: Moved from Low to High priority because {reason}
- {Story Y}: Moved to backlog (de-scoped from upcoming sprint)

### Removed Stories
- {Story Z}: No longer needed because {reason}

---

## Next Sprint Preview

**Tentative Sprint {N+1} Goals:**
- {Goal 1: Complete feature X end-to-end}
- {Goal 2: Address performance bottlenecks}

**Estimated Capacity:** {X} SP
**Top Priority Stories:**
1. {Story Name} - {SP} SP
2. {Story Name} - {SP} SP
3. {Story Name} - {SP} SP

**Sprint {N+1} Start Date:** YYYY-MM-DD

---

## Appendix

### Links & References
- Sprint Plan: `Meta/Sprints/Sprint-{N}/sprint-plan.md`
- Burndown Chart: `Meta/Sprints/Sprint-{N}/burndown.md`
- Story Files: `Meta/Sprints/Sprint-{N}/stories/`
- Daily Standups: `Meta/Sprints/Sprint-{N}/daily-standups/`

### Sprint Photos/Artifacts
- {Link to team photo, whiteboard sketches, etc.}

---

**Review Completed:** YYYY-MM-DD
**Next Review:** YYYY-MM-DD
**Retrospective Facilitator:** {Name}
**Notes Taken By:** {Name}
