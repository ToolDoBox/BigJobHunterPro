# Product Backlog

**Last Updated:** 2026-01-04
**Product Owner:** Team
**Total Backlog Items:** 12+

---

## Backlog Overview

This file contains all user stories that are not yet assigned to a sprint. Stories are organized by priority and epic. Use this as the source for sprint planning.

## Prioritization

- **P0 (Critical):** Must have for MVP
- **P1 (High):** Important for launch
- **P2 (Medium):** Nice to have, improves UX
- **P3 (Low):** Future enhancements

---

## Sprint 1 (In Progress)

| ID | User Story | Story Points | Priority | Status |
|----|------------|--------------|----------|--------|
| 1  | Authenticated app shell | 5 | P0 | 🔄 In Sprint 1 |
| 2  | Quick Capture + points | 5 | P0 | 🔄 In Sprint 1 |
| 3  | Applications list view | 3 | P0 | 🔄 In Sprint 1 |

**Total:** 13 SP

---

## Ready for Next Sprint (Estimated and Refined)

| ID | User Story | Story Points | Priority | Epic | Notes |
|----|------------|--------------|----------|------|-------|
| 4  | Application detail view | 3 | P0 | Core Tracking | View/edit individual application details |
| 5  | Application status updates | 3 | P0 | Core Tracking | Update status (Screening, Interview, Rejected, Offer) with points |
| 6  | Points calculation service | 2 | P0 | Gamification | Award points for status changes (+2 screening, +5 interview, etc.) |
| 7  | User profile with points display | 2 | P0 | Core App Shell | Show total points, streak info in nav |
| 8  | Create hunting party | 5 | P1 | Social Features | Private friend groups with invite codes |
| 9  | Hunting party leaderboard | 5 | P1 | Social Features | Ranked list of party members by points |
| 10 | Rivalry panel | 3 | P1 | Social Features | Show who's directly ahead/behind and gap |

---

## Backlog (Not Yet Estimated)

### Epic: Import & Bulk Operations
- Job-Hunt-Context JSON import tool (with retroactive points calculation)
- Bulk edit applications (status, tags, delete)
- Export to CSV/Excel

### Epic: Activity Feed
- Real-time activity feed (SignalR) showing party members' actions
- Activity types: application logged, status updated, offer received, milestones hit
- Notifications for party milestones

### Epic: Advanced Tracking
- Application notes/journal entries
- Skills/technologies tagging
- Application source tracking (LinkedIn, Indeed, Company Website, etc.)
- Interview preparation notes
- Salary expectations tracking
- Follow-up reminders

### Epic: Streaks & Achievements
- Daily streak tracker (consecutive days with >= 1 application)
- Achievement system (badges/trophies)
  - "First Blood" - Log first application
  - "Hunter's Dozen" - Log 13 applications
  - "Century Club" - Log 100 applications
  - "Perfect Week" - Log 7 days in a row
  - "Interview Pro" - Complete 10 interviews
  - "The Offer" - Receive first offer
  - "The Comeback" - Continue after 20 rejections

### Epic: Analytics & Insights
- Dashboard with charts (applications per week, response rates)
- Conversion funnel (Applied → Screening → Interview → Offer)
- Average time between stages
- Success rate by source/skill/company size

### Epic: UI/UX Polish
- Dark mode toggle
- Sound effects (retro arcade bleeps/bloops) on key actions
- Animations and transitions (retro pixel effects)
- Onboarding tour for new users
- Keyboard shortcuts (hotkeys for Quick Capture, navigation)

### Epic: Mobile Experience
- PWA (Progressive Web App) setup
- Mobile-optimized Quick Capture
- Push notifications for reminders
- Offline mode with sync

### Epic: Admin & Settings
- Account settings (email, password, display name)
- Privacy settings (hide from leaderboards)
- Export all data (GDPR compliance)
- Delete account

### Epic: Performance & Infrastructure
- Caching strategy (Redis)
- Database indexing optimization
- API rate limiting
- Monitoring and logging (Application Insights)
- Automated backups

### Epic: Advanced Social Features
- Multiple hunting parties per user
- Party chat/comments
- Challenge system (compete on specific metrics)
- Public leaderboards (opt-in)

---

## Ice Box (Future Considerations)

- Integration with LinkedIn job search
- Resume/cover letter storage and versioning
- Interview scheduling integration (Google Calendar, Outlook)
- AI-powered job recommendations
- Salary negotiation tips based on offer data
- Networking contacts tracker
- Company research notes and ratings
- Email integration (auto-log applications from sent emails)

---

## Story Template

When adding new stories to backlog, use this template:

```markdown
### Story {ID}: {Title}

**As a** {user type}
**I want** {goal}
**so that** {benefit}

**Priority:** P0/P1/P2/P3
**Story Points:** TBD (estimate during refinement)
**Epic:** {Epic name}
**Dependencies:** {Story IDs this depends on}

**Notes:**
- {Any additional context, technical considerations, or questions}
```

---

## Refinement Schedule

- **Weekly Backlog Grooming:** Every Wednesday
- **Sprint Planning:** First day of sprint (every 2 weeks)
- **Acceptance Criteria:** Defined during sprint planning for selected stories

---

## Completed Stories Archive

Stories completed in previous sprints:

### Sprint 1 (2026-01-04 to 2026-01-17)
- Story 1: Authenticated App Shell - 5 SP - ✅ Completed (TBD)
- Story 2: Quick Capture + Points - 5 SP - ✅ Completed (TBD)
- Story 3: Applications List View - 3 SP - ✅ Completed (TBD)

---

**Backlog Management Notes:**
- Review and re-prioritize backlog weekly
- Archive completed stories at end of each sprint
- Break down stories >8 SP before sprint planning
- Add new discoveries to backlog as they arise
- Keep backlog INVESTED (Independent, Negotiable, Valuable, Estimable, Small, Testable)

---

## Reference

- **Fibonacci Scale:** 1, 2, 3, 5, 8, 13, 21 (break down if >8)
- **Velocity Target:** ~13 SP per 2-week sprint (based on Sprint 1 estimate)
- **MVP Target:** Stories 1-10 (core auth, tracking, points, parties, leaderboards)
- **Phase 1 Target:** Stories 1-6 (auth + basic tracking + points)
- **Phase 2 Target:** Stories 7-10 (social features, leaderboards)

---

**Created:** 2026-01-04
**Last Updated:** 2026-01-04
