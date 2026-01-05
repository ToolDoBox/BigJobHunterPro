# Big Job Hunter Pro — Project Scoping Document

**Project:** Big Job Hunter Pro
**Domain:** bigjobhunter.pro
**Scoping Version:** v1.0
**Date:** 2026-01-04
**Status:** Scoping & Planning Phase

---

## EXECUTIVE SUMMARY

This document defines the feature scope, user stories, and implementation roadmap for Big Job Hunter Pro—a collaborative gamified job tracking application built by and for a friend group of developers experiencing job search burnout.

**Key Context:**
- **Origin:** Friend group collaboration solving immediate, lived problem (not hypothetical future users)
- **Existing System:** Job-Hunt-Context (24+ applications tracked, sophisticated JSON schemas, but fundamentally solo)
- **Core Innovation:** Social accountability + gamification (hunting parties, leaderboards, competitive motivation)
- **Timeline:** Aggressive 4-week Phase 1 (we need this working NOW)

---

## TASK FLOW ANALYSIS

Walking through the user's journey from job search frustration to successful job placement, identifying pain points and opportunities for Big Job Hunter Pro to add value.

### TASK FLOW 1: Logging a New Job Application

**Scenario:** A friend group member finds a job posting on Indeed/LinkedIn and wants to log it quickly before continuing their application spree.

**Current Workflow (Job-Hunt-Context):**
1. Find job posting on job board
2. Copy job details (company, title, URL, skills required)
3. Open code editor or file manager
4. Create new JSON file with naming convention `YYYY-MM-DD-company-role.json`
5. Copy JSON schema template
6. Manually fill 50+ fields (company info, role details, skills, requirements, compensation)
7. Save file to `04-Applications/Open/` folder
8. Refresh localhost:8000 dashboard to see update
9. **Total time: 5-10 minutes**
10. **Friction point: Too slow when applying to 10+ jobs per day**

**Target Workflow (Big Job Hunter Pro - AI-Powered):**
1. Find job posting on job board (Indeed, LinkedIn, etc.)
2. Select all content (Ctrl+A), copy (Ctrl+C)
3. Click "Quick Capture" button (or keyboard shortcut)
4. Paste job listing URL in first field
5. Paste full page content in second field (large text area)
6. Click "Lock & Load" - AI parses content in background
7. **AI Processing:** Cheap AI API extracts structured data (company, role, skills, requirements, salary, etc.)
8. **Total time: 10-15 seconds** (including copy/paste)
9. +1 point feedback appears instantly (parsing happens async)
10. See leaderboard update immediately (competitive feedback)
11. Optional: Review/edit AI-parsed data later via application detail view
12. **Happy Path:** User logs 10 applications in 2 minutes, AI handles all data extraction, user sees leaderboard position improve

**Steps Breakdown:**
1. **Trigger:** User finds job posting worth tracking
2. **Action:** Copy entire page content (Ctrl+A, Ctrl+C)
3. **Input:**
   - Paste job listing URL (for reference)
   - Paste full page content (raw HTML/text)
4. **Submit:** Click "Lock & Load"
5. **AI Processing (Async):**
   - Send content to cheap AI API (e.g., Claude Haiku, GPT-3.5-turbo)
   - AI extracts: Company name, role title, required skills, nice-to-have skills, salary range, work mode, location, job description summary
   - AI generates: Potential interview questions based on role/requirements
   - Store structured JSON in database
6. **Feedback:** +1 point animation, score updates, leaderboard refreshes
7. **Result:** Application logged with rich data, user returns to job search

**Decision Points:**
- Should I review AI-parsed data immediately or trust it? (Review later = faster)
- Should I manually edit fields if AI missed something? (Yes, via detail view)

**Obstacles:**
- AI parsing fails or returns incomplete data → Fallback to manual entry form
- Duplicate application detected by URL → User chooses to skip or override
- Internet connection lost → Queue for parsing when back online
- AI API rate limit hit → Show warning, queue for retry

**Simplification Opportunities:**
- Browser extension auto-captures page content with one click (Phase 4)
- AI learns from user corrections to improve parsing accuracy
- Batch import: paste multiple job URLs, AI processes all in background
- AI-generated interview prep questions stored per application

---

### TASK FLOW 2: Checking Leaderboard & Getting Motivated

**Scenario:** A friend group member is feeling burnt out after receiving two rejection emails. They need a motivation boost to keep applying.

**Current Workflow (Job-Hunt-Context):**
1. Receive rejection email
2. Feel demotivated, consider giving up for the day
3. Open localhost:8000 dashboard (if remembered)
4. See beautiful charts showing progress
5. Feel slightly better but still isolated (no one else sees this progress)
6. **Pain Point:** Charts don't prevent motivation decay—no external accountability

**Target Workflow (Big Job Hunter Pro):**
1. Receive rejection email
2. Open Big Job Hunter Pro (muscle memory from daily streak habit)
3. Log rejection via Quick Capture (+5 points for resilience)
4. See **Rivalry Panel:** "Jordan is 12 points ahead. They have 2 more interviews than you. Close the gap: apply to 3 more jobs today!"
5. Check **Hunting Party Leaderboard:** See friend just overtook your #3 position
6. Feel competitive spark: "I can catch up"
7. **Activity Feed:** "Alex just landed an interview at Google! +5 pts"
8. Feel part of a team; celebrate friend's win
9. Apply to 3 more jobs to close the gap
10. **Happy Path:** Rejection reframed as +5 points; rivalry motivates action instead of demotivation

**Steps Breakdown:**
1. **Trigger:** User opens app (daily habit or after rejection)
2. **Dashboard View:** Personal stats + Hunting Party sidebar
3. **Rivalry Panel:** Shows user directly above, exact gap, actionable nudge
4. **Leaderboard:** Top 10 list, current user highlighted
5. **Activity Feed:** Recent party member wins/milestones
6. **Decision:** Apply to more jobs OR check analytics to optimize strategy

**Decision Points:**
- Check personal stats or leaderboard first? (Dashboard shows both)
- React to rivalry panel nudge? (Close the gap vs. ignore)
- Celebrate friend's win or feel FOMO? (Positive reinforcement culture)

**Obstacles:**
- Leaderboard shows user in last place → Could be demotivating
  - **Mitigation:** Rivalry panel focuses on "next person up" not #1 leader
  - **Mitigation:** Multiple leaderboards (weekly reset, most improved, resilience champion)
- No friends online → Activity feed is stale
  - **Mitigation:** Show personal achievements, historical milestones

---

### TASK FLOW 3: Updating Application Status After Interview

**Scenario:** A friend group member just completed a first-round interview and wants to log the progression and celebrate the milestone.

**Current Workflow (Job-Hunt-Context):**
1. Complete interview
2. Open file manager, navigate to application JSON file
3. Edit `timeline_events` array, add new event:
   ```json
   {
     "event_type": "interview",
     "stage": "interview",
     "timestamp": "2026-01-04T14:30:00Z",
     "notes": "Went well, discussed React project"
   }
   ```
4. Update `status_stage` to `"interview"`
5. Save file
6. Refresh localhost:8000 to see Sankey diagram update
7. **Total time: 2-3 minutes**
8. **Pain Point:** No celebration, no one knows, no points earned

**Target Workflow (Big Job Hunter Pro):**
1. Complete interview
2. Open Big Job Hunter Pro mobile PWA (on commute home)
3. Tap application in list, tap "Update Status"
4. Select "Interview 1 Completed" from dropdown
5. (Optional) Add quick notes via voice input
6. Tap "Update"
7. **Confetti animation:** "Interview Complete! +5 points!"
8. **Achievement Unlocked:** "First Interview" badge appears
9. **Leaderboard Updates:** See position improve from #4 to #3
10. **Activity Feed Post:** "Chris just completed an interview at Infosys! 🎯"
11. Friend group members see update, send encouraging messages
12. **Happy Path:** Milestone celebrated publicly, points earned, motivation sustained

**Steps Breakdown:**
1. **Trigger:** Interview completed (real-world event)
2. **Action:** Open app, navigate to application detail
3. **Update:** Change status via dropdown or quick action
4. **Feedback:** +5 points, achievement unlock, confetti
5. **Social:** Activity feed broadcasts win to hunting party
6. **Result:** Progress logged, team celebrates, user feels accomplished

**Decision Points:**
- Add detailed notes about interview or skip? (Optional)
- Share company name in activity feed or keep private? (Privacy settings)

**Obstacles:**
- Forgot to log interview → Reminder notifications (opt-in)
- Multiple interview rounds confusing → Interview 1, 2, 3, 4+ auto-numbered
- Privacy concerns → Company names hidden, only "completed interview" shown

---

### TASK FLOW 4: Importing Existing Applications from Job-Hunt-Context

**Scenario:** Friend group member (Christian) has 24 applications tracked in Job-Hunt-Context JSON files and wants to migrate to Big Job Hunter Pro without losing historical data.

**Current Workflow (Manual Migration - Nightmare Scenario):**
1. Export each JSON file
2. Copy-paste company, role, skills into new system
3. Manually recreate timeline events
4. **Total time: 10-15 minutes × 24 applications = 4-6 hours**
5. **Pain Point:** Absolutely unacceptable; would abandon tool

**Target Workflow (Big Job Hunter Pro - Import Tool):**
1. Navigate to Settings → Import Data
2. Drag-and-drop all 24 JSON files from Job-Hunt-Context folder
3. Click "Import & Calculate Points"
4. **System Processing:**
   - Parse JSON schema v1
   - Map to Big Job Hunter Pro entities
   - Extract timeline_events for Sankey diagram
   - Calculate retroactive points (Applied +1, Interview +5, Rejection +5)
   - Import skills, company data, requirements
5. Progress bar: "Importing 24 applications... 100%"
6. **Success Screen:** "Imported 24 applications! You've earned 47 points from past work!"
7. Dashboard loads with full history, Sankey diagram shows pipeline
8. **Total time: 2 minutes**
9. **Happy Path:** All historical data preserved, points credited, ready to use immediately

**Steps Breakdown:**
1. **Trigger:** New user onboarding (friend group member)
2. **Action:** Access import tool
3. **Upload:** Bulk file selection or drag-and-drop
4. **Processing:** Schema mapping, validation, point calculation
5. **Review:** Preview imported data (optional dry-run)
6. **Confirm:** Execute import
7. **Feedback:** Success message, point total displayed
8. **Result:** Full history migrated, user starts from running start

**Decision Points:**
- Import all at once or incrementally? (All at once preferred)
- Override existing data or merge? (First import = clean slate)
- Review each application or trust import? (Dry-run preview option)

**Obstacles:**
- JSON schema mismatch → Version detection + mapping rules
- Duplicate applications detected → User chooses to skip or override
- Timeline events out of order → Auto-sort by timestamp
- Missing required fields → Use sensible defaults, flag for review

**Simplification Opportunities:**
- One-click import from GitHub repo (friend group shares repo)
- Batch edit imported applications (update skills across multiple)
- Export back to JSON (bidirectional sync for backup)

---

### TASK FLOW 5: Friend Group Weekly Retrospective

**Scenario:** Friend group schedules weekly Sunday evening check-ins to review progress, share learnings, and set weekly challenges.

**Current Workflow (Ad-hoc Discord/Zoom):**
1. Jump on Discord voice call
2. Each person manually reports: "I applied to 7 jobs, got 1 interview"
3. Someone takes notes in shared Google Doc
4. Discuss what's working: "Indeed seems better than LinkedIn"
5. Set informal goals: "Let's all apply to 10 jobs this week"
6. **Pain Point:** No structured data, relies on memory, goals aren't tracked

**Target Workflow (Big Job Hunter Pro - Group Retrospective):**
1. Open Big Job Hunter Pro "Hunting Party" tab
2. Click "Weekly Retrospective" (auto-generated report)
3. **System Shows:**
   - **Group Stats:** 32 applications, 4 interviews, 2 rejections this week
   - **Top Performer:** Jordan (12 applications, 2 interviews)
   - **Most Improved:** Chris (+8 applications vs. last week)
   - **Resilience Champion:** Alex (logged 3 rejections, kept applying)
   - **Source Analysis:** Indeed (18 apps, 3 interviews) vs. LinkedIn (14 apps, 1 interview)
   - **Skill Gaps:** "React" mentioned in 8 rejections → Consider upskilling
4. Group discusses insights on call, references dashboard live
5. **Weekly Challenge:** "Everyone apply to 10 jobs by Friday"
6. System tracks challenge progress (progress bars for each member)
7. Friday: Notification "Challenge ends in 24 hours! You're at 7/10 applications"
8. **Happy Path:** Data-driven retrospective, collaborative goal-setting, accountability via challenge tracking

**Steps Breakdown:**
1. **Trigger:** Weekly scheduled time (Sunday evening)
2. **Preparation:** System auto-generates retrospective report
3. **Review:** Group reviews stats together (screen share)
4. **Discussion:** Identify patterns, celebrate wins, discuss obstacles
5. **Goal-Setting:** Create weekly challenge in system
6. **Tracking:** Dashboard shows challenge progress throughout week
7. **Result:** Structured accountability, data-driven optimization

**Decision Points:**
- Create new challenge or extend existing? (Fresh start each week)
- Individual goals or collective goal? (Both supported)
- Share retrospective publicly or keep private? (Party-only)

**Obstacles:**
- Not everyone participates → Progress bars show who's behind (friendly pressure)
- Challenge too aggressive → Adjust mid-week (flexible editing)
- Timezone differences → Async retrospective comments

---

## BRAINSTORMING FEATURES

Comprehensive feature list derived from:
1. **Job-Hunt-Context lessons** (what worked, what didn't)
2. **Friend group pain points** (lived experience)
3. **Competitive analysis** (Huntr, Teal, Habitica, Duolingo)
4. **User personas** (Alex, Jordan, Marcus, Priya)
5. **Gamification best practices** (points, streaks, achievements, leaderboards)

### CORE TRACKING FEATURES

1. **Quick Capture Modal** - 15-second application logging (company, role, source)
2. **Application List View** - Master list with filters (status, work mode, date)
3. **Application Detail View** - Master-detail workbench with full information
4. **Timeline Events** - Chronological history (applied, screening, interview, rejected)
5. **Status Stage Management** - Kanban-style stage progression
6. **Skills Tagging** - Required/nice-to-have skills per application
7. **Company Information** - Name, industry, rating, website
8. **Compensation Tracking** - Salary ranges, equity, bonuses
9. **Notes System** - Private notes per application (interview prep, company research)
10. **Source Attribution** - Track which job boards applications came from

### GAMIFICATION FEATURES

11. **Points System** - Score for actions (Applied +1, Interview +5, Rejection +5, Offer +50)
12. **Daily Streaks** - Consecutive days logging at least one action
13. **Streak Fire Icon** - Visual streak counter with warnings ("6 hours left today!")
14. **Achievement Badges** - Milestones (First Hunt, 10 Applications, Interview Champion, Resilience Award)
15. **Trophy Room** - Badge collection display with progress bars
16. **Confetti Celebrations** - Animations at milestones (10, 25, 50, 100 applications)
17. **Level/XP System** - Progression tiers (Novice Hunter → Expert Hunter)
18. **Combo Multipliers** - Bonus points for consecutive days of activity

### SOCIAL/COLLABORATIVE FEATURES

19. **Hunting Party Creation** - Create friend groups (2-20 members)
20. **Hunting Party Invites** - Generate invite links, send via email/Discord
21. **Leaderboard (All-Time)** - Ranked list by total points
22. **Leaderboard (Weekly)** - Resets every Monday for fresh competition
23. **Leaderboard (Monthly)** - Longer-term tracking
24. **Rivalry Panel** - "You're 15 points behind Jordan—close the gap!"
25. **Activity Feed (Personal)** - Your recent score events
26. **Activity Feed (Party)** - All hunting party member updates
27. **Weekly Challenges** - Group goals ("Everyone apply to 10 jobs")
28. **Challenge Progress Tracking** - Progress bars showing completion status
29. **Group Retrospective Dashboard** - Weekly stats, top performers, insights
30. **Member Comparison View** - Side-by-side stats with friends

### META-ANALYSIS/INSIGHTS FEATURES

31. **Sankey Diagram** - Pipeline flow visualization (Prospecting → Applied → Interview → Offer/Rejected)
32. **Source Attribution Analytics** - Conversion rates by job board
33. **Top Skills Analysis** - Most frequent skills across applications
34. **Work Mode Distribution** - Remote/Hybrid/Onsite breakdown
35. **Applications Over Time** - Cumulative line chart
36. **Interview Conversion Rate** - % of applications reaching interview
37. **Time-to-Interview Metric** - Average days from applied to interview
38. **Best Week Tracker** - Highest application count in a single week
39. **Rejection Analysis** - Common rejection patterns
40. **Skill Gap Identification** - Skills appearing in rejections frequently

### IMPORT/EXPORT FEATURES

41. **JSON Import (Job-Hunt-Context)** - Bulk import with schema mapping
42. **CSV Import** - Import from spreadsheets (Huntr, Teal exports)
43. **Retroactive Point Calculation** - Award points for imported timeline history
44. **Dry-Run Import Preview** - Review before committing
45. **Export to JSON** - Backup and data portability
46. **Export to CSV** - Spreadsheet compatibility
47. **Export to PNG** - Dashboard screenshot for sharing
48. **Export to PDF** - Printable report

### USER EXPERIENCE FEATURES

49. **Dark Mode / Light Mode** - Theme toggle with system preference detection
50. **Responsive Design** - Mobile, tablet, desktop optimization
51. **PWA Capabilities** - Install as app, offline support
52. **Keyboard Shortcuts** - Quick Capture (Ctrl+K), navigation
53. **Search & Filters** - Find applications by company, role, skills
54. **Sort Options** - By date, status, points, company name
55. **Bulk Actions** - Select multiple applications, update status
56. **Undo/Redo** - Revert accidental changes
57. **Auto-save Drafts** - Don't lose work if connection drops
58. **Toast Notifications** - Success/error feedback

### AUTOMATION/INTEGRATION FEATURES

59. **Browser Extension** - One-click capture from job boards (Indeed, LinkedIn)
60. **Email Integration** - Auto-log applications sent via email
61. **Calendar Integration** - Sync interview dates to Google Calendar
62. **LinkedIn Integration** - Pull job details from LinkedIn posts
63. **Indeed API Integration** - Auto-fill company info from job ID
64. **Glassdoor API** - Pull company ratings automatically
65. **Webhook Support** - Connect to Zapier, IFTTT for custom automations

### COMMUNICATION/REMINDER FEATURES

66. **Follow-up Reminders** - "Check on application after 2 weeks"
67. **Streak Warnings** - "Log today to keep your 7-day streak alive!"
68. **Interview Prep Reminders** - "Interview tomorrow at 2pm"
69. **Weekly Digest Email** - Summary of progress, leaderboard position
70. **Push Notifications** - Mobile alerts for leaderboard changes
71. **Discord Bot Integration** - Post activity feed to Discord channel
72. **Slack Integration** - Share milestones in Slack workspace

### PRIVACY/SECURITY FEATURES

73. **Privacy Controls** - Hide company names from leaderboard
74. **Anonymous Mode** - Participate without revealing identity
75. **Opt-out Leaderboards** - Track privately without competition
76. **Two-Factor Authentication** - Security for sensitive job search data
77. **Data Encryption** - Encrypt notes, compensation data at rest
78. **GDPR Compliance** - Data export, right to deletion
79. **Session Management** - Auto-logout after inactivity

### CONTENT/EDUCATION FEATURES

80. **Interview Question Bank** - Common questions per role type
81. **Resume Templates** - Downloadable templates
82. **Cover Letter Builder** - AI-assisted writing (Phase 4)
83. **Job Search Tips Library** - Best practices, articles
84. **Salary Negotiation Guide** - Resources for offers
85. **Application Status Decoder** - "What does 'Under Review' mean?"

### AI-POWERED FEATURES (PHASE 1 PRIORITY)

86. **AI Job Listing Parser** - Extract structured data from pasted job content (company, role, skills, salary, requirements)
87. **AI Interview Question Generator** - Generate potential interview questions based on job requirements
88. **AI Skill Extraction** - Automatically tag required and nice-to-have skills from job description
89. **AI Salary Range Detection** - Parse compensation from various formats ($120k-$150k, $60-$75/hr, etc.)
90. **AI Work Mode Detection** - Identify Remote/Hybrid/Onsite from job description
91. **AI Location Parsing** - Extract city, state, country from listing
92. **AI Company Information Enrichment** - Look up company industry, size, rating (Phase 2)
93. **AI Job Description Summarization** - Generate concise summary of role responsibilities

### ADVANCED/FUTURE AI FEATURES

94. **AI Resume Feedback** - Score resume against job description
95. **AI Job Matching** - Suggest jobs based on skills/preferences
96. **AI Cover Letter Generator** - GPT-powered drafting
97. **Application Auto-filler** - Fill common forms automatically
98. **Interview Recording Transcription** - Practice interview analysis
99. **AI Interview Prep Assistant** - Personalized prep based on job requirements
100. **AI Application Optimizer** - Suggest resume/cover letter improvements per job

### OTHER ADVANCED FEATURES

101. **Mood Tracking** - Emotional state correlation with success
102. **Habit Stacking** - Connect job search to other productivity tools
103. **Referral Network** - Connect with people who work at target companies
104. **Salary Data Aggregation** - Crowdsourced compensation insights
105. **Job Market Trends** - Hiring trends for specific roles/companies

### ADMINISTRATIVE/OPERATIONAL FEATURES

106. **User Account Management** - Profile, settings, preferences
107. **Billing/Subscription** - Free tier + premium tier
108. **Admin Dashboard** - User analytics, health metrics
109. **Customer Support Chat** - In-app help
110. **Feedback System** - In-app feature voting (Canny integration)

---

## BROAD USER STORIES

Converting brainstormed features into user stories with importance (1-10), feasibility (1-10), and story points (effort estimation).

**Format:** As a [user type], I want [action] so that I can [value].

### AUTHENTICATION & ONBOARDING

**⬜ US-001: User Registration**
As a **new user**,
I want to **create an account with email/password or OAuth (Google, GitHub)**
so that I can **securely access my job tracking data across devices**.
Importance: **10** | Feasibility: **9** | Story Points: **3**

**⬜ US-002: User Login**
As a **returning user**,
I want to **log in with my credentials**
so that I can **access my saved applications and progress**.
Importance: **10** | Feasibility: **10** | Story Points: **2**

**⬜ US-003: Onboarding Tutorial**
As a **new user**,
I want to **see a quick tutorial on Quick Capture, hunting parties, and leaderboards**
so that I can **understand core features within 60 seconds**.
Importance: **7** | Feasibility: **8** | Story Points: **5**

---

### CORE TRACKING

**⬜ US-004: AI-Powered Quick Capture (10-Second Promise)**
As a **job seeker**,
I want to **log a new application in 10 seconds by pasting the job URL and page content, with AI automatically extracting all details (company, role, skills, salary, requirements)**
so that I can **maintain momentum during high-volume application days without manual data entry**.
Importance: **10** | Feasibility: **7** | Story Points: **8**

**⬜ US-005: View Application List**
As a **job seeker**,
I want to **see all my applications in a sortable/filterable list**
so that I can **quickly find specific companies or roles**.
Importance: **10** | Feasibility: **10** | Story Points: **3**

**⬜ US-006: Application Detail View**
As a **job seeker**,
I want to **click on an application to see full details (company info, skills, timeline, notes)**
so that I can **review everything about a specific opportunity**.
Importance: **9** | Feasibility: **9** | Story Points: **8**

**⬜ US-007: Update Application Status**
As a **job seeker**,
I want to **change application status (Applied → Screening → Interview → Offer/Rejected)**
so that I can **track progression through the pipeline**.
Importance: **10** | Feasibility: **9** | Story Points: **5**

**⬜ US-008: Add Timeline Events**
As a **job seeker**,
I want to **log specific events (email received, phone screen scheduled, interview completed)**
so that I can **maintain detailed history for Sankey diagram visualization**.
Importance: **8** | Feasibility: **8** | Story Points: **5**

**⬜ US-009: Add/Edit Notes**
As a **job seeker**,
I want to **write private notes about interviews, company research, and follow-ups**
so that I can **prepare for next steps and remember key details**.
Importance: **9** | Feasibility: **10** | Story Points: **3**

**⬜ US-010: Tag Skills**
As a **job seeker**,
I want to **tag required and nice-to-have skills for each application**
so that I can **analyze which skills are most in-demand and identify gaps**.
Importance: **7** | Feasibility: **8** | Story Points: **5**

---

### GAMIFICATION

**⬜ US-011: Earn Points for Actions**
As a **job seeker**,
I want to **earn +1 for applications, +5 for interviews, +5 for rejections, +50 for offers**
so that I can **see tangible progress and feel rewarded for effort (not just outcomes)**.
Importance: **10** | Feasibility: **8** | Story Points: **5**

**⬜ US-012: View Total Score**
As a **job seeker**,
I want to **see my total score prominently displayed on the dashboard**
so that I can **track overall progress and feel accomplished**.
Importance: **9** | Feasibility: **10** | Story Points: **2**

**⬜ US-013: Daily Streak Tracking**
As a **job seeker**,
I want to **maintain a daily streak by logging at least one action per day**
so that I can **build a consistent habit and avoid breaking the chain**.
Importance: **9** | Feasibility: **7** | Story Points: **5**

**⬜ US-014: Streak Fire Icon & Warnings**
As a **job seeker**,
I want to **see a fire icon with my current streak and warnings ("6 hours left today!")**
so that I can **maintain motivation and avoid accidentally breaking my streak**.
Importance: **8** | Feasibility: **8** | Story Points: **3**

**⬜ US-015: Unlock Achievement Badges**
As a **job seeker**,
I want to **unlock achievements for milestones (First Hunt, 10 Applications, Interview Champion)**
so that I can **celebrate progress and collect trophies**.
Importance: **8** | Feasibility: **7** | Story Points: **8**

**⬜ US-016: View Trophy Room**
As a **job seeker**,
I want to **see all available achievements with progress bars for incomplete ones**
so that I can **know what to work toward next**.
Importance: **7** | Feasibility: **9** | Story Points: **5**

**⬜ US-017: Confetti Celebrations**
As a **job seeker**,
I want to **see confetti animations when I hit milestones (10, 25, 50, 100 applications)**
so that I can **feel celebrated and share the moment**.
Importance: **6** | Feasibility: **9** | Story Points: **3**

---

### SOCIAL/COLLABORATIVE (PHASE 1 PRIORITY)

**⬜ US-018: Create Hunting Party**
As a **friend group member**,
I want to **create a hunting party with a name and invite friends**
so that we can **track progress together and compete**.
Importance: **10** | Feasibility: **7** | Story Points: **8**

**⬜ US-019: Generate Invite Link**
As a **hunting party creator**,
I want to **generate a unique invite link to share via Discord/email**
so that I can **easily onboard friends without manual setup**.
Importance: **10** | Feasibility: **9** | Story Points: **3**

**⬜ US-020: Join Hunting Party via Invite**
As a **friend group member**,
I want to **click an invite link and auto-join the hunting party**
so that I can **start competing immediately**.
Importance: **10** | Feasibility: **9** | Story Points: **5**

**⬜ US-021: View Leaderboard (All-Time)**
As a **hunting party member**,
I want to **see a ranked list of all members by total points**
so that I can **know where I stand and who's leading**.
Importance: **10** | Feasibility: **8** | Story Points: **5**

**⬜ US-022: View Weekly Leaderboard**
As a **hunting party member**,
I want to **see a weekly leaderboard that resets every Monday**
so that I can **have fresh chances to win and maintain engagement**.
Importance: **9** | Feasibility: **7** | Story Points: **5**

**⬜ US-023: Rivalry Panel ("Close the Gap")**
As a **hunting party member**,
I want to **see who's directly above me with exact gap and actionable nudge**
so that I can **feel motivated to catch up (not overwhelmed by #1 leader)**.
Importance: **10** | Feasibility: **7** | Story Points: **5**

**⬜ US-024: Activity Feed (Party)**
As a **hunting party member**,
I want to **see recent wins/milestones from all party members (interviews, offers, achievements)**
so that I can **celebrate together and feel part of a team**.
Importance: **9** | Feasibility: **8** | Story Points: **5**

**⬜ US-025: Activity Feed (Personal)**
As a **job seeker**,
I want to **see my own recent score events in a chronological feed**
so that I can **review my progress and feel accomplished**.
Importance: **7** | Feasibility: **9** | Story Points: **3**

---

### IMPORT/EXPORT (PHASE 1 PRIORITY)

**⬜ US-026: Import Job-Hunt-Context JSON Files**
As a **friend group member with existing data**,
I want to **bulk import 24+ JSON files from Job-Hunt-Context**
so that I can **preserve 2+ months of tracking history without manual re-entry**.
Importance: **10** | Feasibility: **6** | Story Points: **13**

**⬜ US-027: Retroactive Point Calculation**
As a **user importing historical data**,
I want to **earn points for past applications/interviews/rejections based on timeline events**
so that I can **start with a running total that reflects my actual work**.
Importance: **10** | Feasibility: **7** | Story Points: **8**

**⬜ US-028: Dry-Run Import Preview**
As a **cautious user**,
I want to **preview imported data before committing changes**
so that I can **verify accuracy and avoid corrupting my database**.
Importance: **7** | Feasibility: **8** | Story Points: **5**

**⬜ US-029: Export to JSON**
As a **power user**,
I want to **export all my data to JSON format**
so that I can **back up my data and maintain portability**.
Importance: **8** | Feasibility: **9** | Story Points: **3**

**⬜ US-030: Export Dashboard to PNG**
As a **job seeker**,
I want to **export my dashboard as a PNG image**
so that I can **share progress snapshots with friends or mentors**.
Importance: **6** | Feasibility: **8** | Story Points: **3**

---

### META-ANALYSIS/INSIGHTS (PHASE 2 PRIORITY)

**⬜ US-031: Sankey Diagram (Pipeline Flow)**
As a **data-driven job seeker**,
I want to **visualize my application funnel from Prospecting → Applied → Interview → Offer/Rejected**
so that I can **see conversion rates and identify bottlenecks**.
Importance: **9** | Feasibility: **6** | Story Points: **13**

**⬜ US-032: Source Attribution Analytics**
As a **strategic job seeker**,
I want to **see which job boards (Indeed, LinkedIn, etc.) yield the most interviews**
so that I can **focus effort on high-conversion sources**.
Importance: **9** | Feasibility: **7** | Story Points: **8**

**⬜ US-033: Top Skills Analysis**
As a **job seeker analyzing market demand**,
I want to **see the 15 most frequent required skills across my applications**
so that I can **identify skill gaps and upskilling opportunities**.
Importance: **8** | Feasibility: **8** | Story Points: **5**

**⬜ US-034: Work Mode Distribution Chart**
As a **job seeker with location preferences**,
I want to **see a breakdown of Remote/Hybrid/Onsite applications**
so that I can **track whether I'm applying to my preferred work modes**.
Importance: **6** | Feasibility: **9** | Story Points: **3**

**⬜ US-035: Applications Over Time (Cumulative Chart)**
As a **job seeker tracking volume**,
I want to **see a cumulative line chart of applications over time**
so that I can **visualize momentum and identify slow weeks**.
Importance: **7** | Feasibility: **9** | Story Points: **5**

**⬜ US-036: Interview Conversion Rate**
As a **job seeker optimizing strategy**,
I want to **see % of applications that reach interview stage**
so that I can **benchmark my success rate and adjust approach**.
Importance: **8** | Feasibility: **8** | Story Points: **3**

---

### ADVANCED SOCIAL (PHASE 3 PRIORITY)

**⬜ US-037: Weekly Challenges**
As a **hunting party member**,
I want to **create group challenges like "Everyone apply to 10 jobs this week"**
so that we can **collaborate on goals and hold each other accountable**.
Importance: **9** | Feasibility: **6** | Story Points: **13**

**⬜ US-038: Challenge Progress Tracking**
As a **challenge participant**,
I want to **see real-time progress bars for myself and teammates**
so that I can **know if I'm on track and who needs encouragement**.
Importance: **8** | Feasibility: **7** | Story Points: **8**

**⬜ US-039: Group Retrospective Dashboard**
As a **hunting party member**,
I want to **see weekly stats (group total, top performer, most improved, source analysis)**
so that we can **have data-driven retrospectives and optimize strategy together**.
Importance: **9** | Feasibility: **6** | Story Points: **13**

**⬜ US-040: Comparative Member Analysis**
As a **curious hunting party member**,
I want to **compare my stats side-by-side with specific friends**
so that I can **learn from their strategies (which job boards, skills, approach)**.
Importance: **7** | Feasibility: **7** | Story Points: **8**

---

### USER EXPERIENCE

**⬜ US-041: Dark Mode / Light Mode Toggle**
As a **user with theme preferences**,
I want to **switch between dark and light modes**
so that I can **use the app comfortably day or night**.
Importance: **7** | Feasibility: **9** | Story Points: **3**

**⬜ US-042: Responsive Design (Mobile/Tablet/Desktop)**
As a **user on multiple devices**,
I want to **access the app seamlessly on phone, tablet, and desktop**
so that I can **log applications anywhere**.
Importance: **9** | Feasibility: **7** | Story Points: **8**

**⬜ US-043: PWA Installation**
As a **mobile user**,
I want to **install Big Job Hunter Pro as a PWA on my phone**
so that I can **access it like a native app without app store friction**.
Importance: **8** | Feasibility: **6** | Story Points: **8**

**⬜ US-044: Keyboard Shortcuts (Quick Capture Hotkey)**
As a **power user**,
I want to **press Ctrl+K to open Quick Capture from anywhere**
so that I can **log applications without mouse clicks**.
Importance: **7** | Feasibility: **8** | Story Points: **3**

**⬜ US-045: Search Applications**
As a **job seeker with many applications**,
I want to **search by company name or role title**
so that I can **quickly find specific applications**.
Importance: **8** | Feasibility: **9** | Story Points: **3**

**⬜ US-046: Filter Applications (Status, Work Mode, Date)**
As a **job seeker organizing pipeline**,
I want to **filter applications by status (Open/Closed), work mode (Remote/Hybrid), or date range**
so that I can **focus on specific subsets**.
Importance: **8** | Feasibility: **8** | Story Points: **5**

---

### AUTOMATION/INTEGRATION (PHASE 4 PRIORITY)

**⬜ US-047: Browser Extension (One-Click Capture)**
As a **job seeker browsing job boards**,
I want to **click a browser extension button to auto-fill Quick Capture from Indeed/LinkedIn**
so that I can **log applications in 5 seconds instead of 15**.
Importance: **9** | Feasibility: **5** | Story Points: **13**

**⬜ US-048: Calendar Integration (Interview Sync)**
As a **job seeker with interviews**,
I want to **auto-sync interview dates/times to Google Calendar**
so that I can **avoid double-booking and get reminders**.
Importance: **7** | Feasibility: **6** | Story Points: **8**

**⬜ US-049: Email Integration (Auto-Log Applications)**
As a **job seeker applying via email**,
I want to **automatically log applications sent from my Gmail "Sent" folder**
so that I can **capture all applications without manual entry**.
Importance: **8** | Feasibility: **4** | Story Points: **21**

---

### COMMUNICATION/REMINDERS

**⬜ US-050: Follow-up Reminders**
As a **job seeker managing follow-ups**,
I want to **get reminded to check on applications 2 weeks after applying**
so that I can **send polite follow-up emails and stay proactive**.
Importance: **7** | Feasibility: **7** | Story Points: **5**

**⬜ US-051: Streak Warnings (Push Notifications)**
As a **streak-focused user**,
I want to **receive push notifications saying "Log today to keep your 7-day streak!"**
so that I can **avoid accidentally breaking streaks**.
Importance: **8** | Feasibility: **7** | Story Points: **5**

**⬜ US-052: Weekly Digest Email**
As a **user who wants summaries**,
I want to **receive a weekly email with stats, leaderboard position, and achievements**
so that I can **stay informed without logging in daily**.
Importance: **6** | Feasibility: **8** | Story Points: **5**

---

### PRIVACY/SECURITY

**⬜ US-053: Privacy Controls (Hide Company Names)**
As a **privacy-conscious hunting party member**,
I want to **hide company names from leaderboard/activity feed**
so that I can **compete without revealing where I'm applying**.
Importance: **8** | Feasibility: **7** | Story Points: **5**

**⬜ US-054: Anonymous Mode**
As a **extremely private user**,
I want to **participate in leaderboard without showing my name/photo**
so that I can **benefit from competition without identity exposure**.
Importance: **6** | Feasibility: **8** | Story Points: **3**

**⬜ US-055: Opt-Out Leaderboards**
As a **non-competitive user**,
I want to **track privately without joining hunting parties or leaderboards**
so that I can **use the tool solo if social features stress me out**.
Importance: **7** | Feasibility: **9** | Story Points: **2**

---

### AI-POWERED PARSING (PHASE 1 PRIORITY)

**⬜ US-056: AI Job Listing Parser**
As a **job seeker**,
I want to **paste a job listing URL and page content, and have AI automatically extract all structured data (company, role, skills, salary, requirements)**
so that I can **avoid manual data entry and log applications in 10 seconds**.
Importance: **10** | Feasibility: **6** | Story Points: **13**

**⬜ US-057: AI Skill Extraction**
As a **job seeker analyzing market demand**,
I want to **have AI automatically identify and tag required and nice-to-have skills from job descriptions**
so that I can **see skill gaps without manual tagging**.
Importance: **9** | Feasibility: **7** | Story Points: **8**

**⬜ US-058: AI Interview Question Generator**
As a **job seeker preparing for interviews**,
I want to **have AI generate potential interview questions based on the job requirements and description**
so that I can **prepare effectively for each specific role**.
Importance: **8** | Feasibility: **6** | Story Points: **8**

**⬜ US-059: AI Salary Range Detection**
As a **job seeker tracking compensation**,
I want to **have AI parse salary ranges from various formats ($120k-$150k, $60-$75/hr, equity details)**
so that I can **track compensation without manual normalization**.
Importance: **7** | Feasibility: **7** | Story Points: **5**

**⬜ US-060: AI Work Mode Detection**
As a **job seeker with location preferences**,
I want to **have AI detect work mode (Remote/Hybrid/Onsite) from job descriptions**
so that I can **automatically filter by my preferred work arrangements**.
Importance: **8** | Feasibility: **8** | Story Points: **5**

**⬜ US-061: Review AI-Parsed Data**
As a **cautious user**,
I want to **review and edit AI-extracted data in the application detail view**
so that I can **correct any parsing errors or add missing information**.
Importance: **9** | Feasibility: **9** | Story Points: **5**

---

### ADVANCED/AI FEATURES (FUTURE PHASES)

**⬜ US-062: AI Resume Feedback**
As a **job seeker optimizing my resume**,
I want to **get AI-powered feedback scoring my resume against a job description**
so that I can **improve match rate and ATS compatibility**.
Importance: **8** | Feasibility: **4** | Story Points: **21**

**⬜ US-063: AI Cover Letter Generator**
As a **job seeker tired of writing cover letters**,
I want to **generate tailored cover letters via GPT-4**
so that I can **save time and maintain quality**.
Importance: **7** | Feasibility: **5** | Story Points: **13**

**⬜ US-064: AI Job Matching**
As a **job seeker discovering opportunities**,
I want to **receive AI suggestions for jobs matching my skills/preferences**
so that I can **find hidden opportunities I might miss**.
Importance: **8** | Feasibility: **3** | Story Points: **21**

---

## Total User Stories: 64 (Scoped for comprehensive coverage, +6 AI parsing features)

---

## PRIORITIZING FEATURES

Using the Importance/Feasibility Matrix to categorize user stories into:
- **DO NOW** (High Importance, High Feasibility) - Phase 1 MVP
- **PLAN FOR** (High Importance, Low Feasibility) - Phase 2-3
- **NICE TO HAVE** (Low Importance, High Feasibility) - Phase 4 or backlog
- **AVOID** (Low Importance, Low Feasibility) - Never or distant future

### Importance/Feasibility Matrix

```
Feasibility (1=Hard, 10=Easy)
      1   2   3   4   5   6   7   8   9   10
    ┌───┬───┬───┬───┬───┬───┬───┬───┬───┬───┐
10  │PLAN│PLAN│PLAN│PLAN│PLAN│DO │DO │DO │DO │DO │
    ├───┼───┼───┼───┼───┼───┼───┼───┼───┼───┤
 9  │PLAN│PLAN│PLAN│PLAN│PLAN│PLAN│DO │DO │DO │DO │
    ├───┼───┼───┼───┼───┼───┼───┼───┼───┼───┤
 8  │PLAN│PLAN│PLAN│PLAN│PLAN│PLAN│DO │DO │DO │DO │
I   ├───┼───┼───┼───┼───┼───┼───┼───┼───┼───┤
M 7 │PLAN│PLAN│PLAN│PLAN│PLAN│PLAN│NICE│NICE│NICE│NICE│
P   ├───┼───┼───┼───┼───┼───┼───┼───┼───┼───┤
O 6 │AVOID│AVOID│AVOID│AVOID│PLAN│PLAN│NICE│NICE│NICE│NICE│
R   ├───┼───┼───┼───┼───┼───┼───┼───┼───┼───┤
T 5 │AVOID│AVOID│AVOID│AVOID│AVOID│AVOID│NICE│NICE│NICE│NICE│
A   ├───┼───┼───┼───┼───┼───┼───┼───┼───┼───┤
N 4 │AVOID│AVOID│AVOID│AVOID│AVOID│AVOID│AVOID│NICE│NICE│NICE│
C   ├───┼───┼───┼───┼───┼───┼───┼───┼───┼───┤
E 3 │AVOID│AVOID│AVOID│AVOID│AVOID│AVOID│AVOID│AVOID│NICE│NICE│
    ├───┼───┼───┼───┼───┼───┼───┼───┼───┼───┤
 2  │AVOID│AVOID│AVOID│AVOID│AVOID│AVOID│AVOID│AVOID│AVOID│NICE│
    ├───┼───┼───┼───┼───┼───┼───┼───┼───┼───┤
 1  │AVOID│AVOID│AVOID│AVOID│AVOID│AVOID│AVOID│AVOID│AVOID│AVOID│
    └───┴───┴───┴───┴───┴───┴───┴───┴───┴───┘
```

### DO NOW (Phase 1 MVP) - High Importance, High Feasibility

| ID | User Story | Importance | Feasibility | Story Points | Quadrant |
|----|------------|------------|-------------|--------------|----------|
| US-001 | User Registration | 10 | 9 | 3 | DO NOW |
| US-002 | User Login | 10 | 10 | 2 | DO NOW |
| US-004 | AI-Powered Quick Capture (10-sec) | 10 | 7 | 8 | DO NOW |
| US-005 | View Application List | 10 | 10 | 3 | DO NOW |
| US-006 | Application Detail View | 9 | 9 | 8 | DO NOW |
| US-007 | Update Application Status | 10 | 9 | 5 | DO NOW |
| US-009 | Add/Edit Notes | 9 | 10 | 3 | DO NOW |
| US-011 | Earn Points for Actions | 10 | 8 | 5 | DO NOW |
| US-012 | View Total Score | 9 | 10 | 2 | DO NOW |
| US-019 | Generate Invite Link | 10 | 9 | 3 | DO NOW |
| US-020 | Join Hunting Party | 10 | 9 | 5 | DO NOW |
| US-021 | Leaderboard (All-Time) | 10 | 8 | 5 | DO NOW |
| US-024 | Activity Feed (Party) | 9 | 8 | 5 | DO NOW |
| US-029 | Export to JSON | 8 | 9 | 3 | DO NOW |
| US-060 | AI Work Mode Detection | 8 | 8 | 5 | DO NOW |
| US-061 | Review AI-Parsed Data | 9 | 9 | 5 | DO NOW |

**Phase 1 Total Story Points: ~73** (Includes AI parsing - aggressive 4-week timeline)

---

### PLAN FOR (Phase 2-3) - High Importance, Lower Feasibility

| ID | User Story | Importance | Feasibility | Story Points | Quadrant |
|----|------------|------------|-------------|--------------|----------|
| US-008 | Add Timeline Events | 8 | 8 | 5 | PLAN FOR |
| US-013 | Daily Streak Tracking | 9 | 7 | 5 | PLAN FOR |
| US-015 | Unlock Achievement Badges | 8 | 7 | 8 | PLAN FOR |
| US-018 | Create Hunting Party | 10 | 7 | 8 | PLAN FOR (MVP) |
| US-022 | Weekly Leaderboard | 9 | 7 | 5 | PLAN FOR |
| US-023 | Rivalry Panel | 10 | 7 | 5 | PLAN FOR (MVP) |
| US-026 | Import Job-Hunt-Context | 10 | 6 | 13 | PLAN FOR (MVP) |
| US-027 | Retroactive Points | 10 | 7 | 8 | PLAN FOR (MVP) |
| US-031 | Sankey Diagram | 9 | 6 | 13 | PLAN FOR |
| US-032 | Source Attribution | 9 | 7 | 8 | PLAN FOR |
| US-037 | Weekly Challenges | 9 | 6 | 13 | PLAN FOR |
| US-039 | Group Retrospective | 9 | 6 | 13 | PLAN FOR |
| US-042 | Responsive Design | 9 | 7 | 8 | PLAN FOR |
| US-043 | PWA Installation | 8 | 6 | 8 | PLAN FOR |
| US-056 | AI Job Listing Parser | 10 | 6 | 13 | PLAN FOR (MVP) |
| US-057 | AI Skill Extraction | 9 | 7 | 8 | PLAN FOR (MVP) |
| US-058 | AI Interview Questions | 8 | 6 | 8 | PLAN FOR |
| US-059 | AI Salary Detection | 7 | 7 | 5 | PLAN FOR |

**Note:** US-018, US-023, US-026, US-027, US-056, US-057 moved to Phase 1 MVP despite lower feasibility due to core AI-powered input strategy and friend group needs.

---

### NICE TO HAVE (Phase 4 or Backlog) - Lower Importance or High Effort

| ID | User Story | Importance | Feasibility | Story Points |
|----|------------|------------|-------------|--------------|
| US-003 | Onboarding Tutorial | 7 | 8 | 5 |
| US-010 | Tag Skills | 7 | 8 | 5 |
| US-014 | Streak Fire Icon | 8 | 8 | 3 |
| US-016 | View Trophy Room | 7 | 9 | 5 |
| US-017 | Confetti Celebrations | 6 | 9 | 3 |
| US-025 | Activity Feed (Personal) | 7 | 9 | 3 |
| US-028 | Dry-Run Import Preview | 7 | 8 | 5 |
| US-030 | Export to PNG | 6 | 8 | 3 |
| US-033 | Top Skills Analysis | 8 | 8 | 5 |
| US-034 | Work Mode Distribution | 6 | 9 | 3 |
| US-035 | Applications Over Time | 7 | 9 | 5 |
| US-036 | Interview Conversion Rate | 8 | 8 | 3 |
| US-040 | Comparative Analysis | 7 | 7 | 8 |
| US-041 | Dark/Light Mode | 7 | 9 | 3 |
| US-044 | Keyboard Shortcuts | 7 | 8 | 3 |
| US-045 | Search Applications | 8 | 9 | 3 |
| US-046 | Filter Applications | 8 | 8 | 5 |
| US-050 | Follow-up Reminders | 7 | 7 | 5 |
| US-051 | Streak Warnings | 8 | 7 | 5 |
| US-052 | Weekly Digest Email | 6 | 8 | 5 |
| US-053 | Privacy Controls | 8 | 7 | 5 |
| US-054 | Anonymous Mode | 6 | 8 | 3 |
| US-055 | Opt-Out Leaderboards | 7 | 9 | 2 |

---

### AVOID (Distant Future or Never) - Low Value/High Effort

| ID | User Story | Importance | Feasibility | Story Points |
|----|------------|------------|-------------|--------------|
| US-047 | Browser Extension | 9 | 5 | 13 |
| US-048 | Calendar Integration | 7 | 6 | 8 |
| US-049 | Email Integration | 8 | 4 | 21 |
| US-056 | AI Resume Feedback | 8 | 4 | 21 |
| US-057 | AI Cover Letter Gen | 7 | 5 | 13 |
| US-058 | AI Job Matching | 8 | 3 | 21 |

**Note:** Browser extension (US-047) is high importance but deferred to Phase 4 due to complexity.

---

## DEFINE THE MINIMAL VIABLE PROJECT (MVP)

### 1. Prioritized Goals (20% that solves 80% of the problem)

**Core Problem:** Friend group experiencing job search burnout due to isolation, lack of motivation, and tracking friction.

**20% of Goals:**
1. **Enable ultra-fast application logging** (AI-Powered Quick Capture in 10 seconds - just paste URL + content)
2. **Eliminate manual data entry** (AI extracts company, role, skills, salary, requirements automatically)
3. **Create competitive social accountability** (Hunting parties + leaderboards)
4. **Preserve existing work** (Import Job-Hunt-Context data with retroactive points)
5. **Gamify effort** (Points for applications, interviews, AND rejections)
6. **Show progress visibly** (Dashboard with score, rank, activity feed)

These 6 goals solve:
- ✅ **Isolation** → Hunting parties + leaderboards create visibility
- ✅ **Motivation decay** → Points + rivalry panel drive daily action
- ✅ **Tracking friction** → AI-powered 10-second Quick Capture vs. 5-10 minute JSON editing
- ✅ **Manual data entry** → AI parses everything automatically (company, skills, salary, etc.)
- ✅ **Data migration** → Import 24 existing applications without loss
- ✅ **Burnout** → Rejections earn +5 points, reframing failure as progress
- ✅ **Interview prep** → AI generates potential interview questions per job

---

### 2. Prioritized Features (DO NOW Quadrant)

**Phase 1 MVP Features (22 User Stories):**

**Authentication (2):**
- US-001: User Registration
- US-002: User Login

**Core Tracking (7):**
- US-004: AI-Powered Quick Capture (10-second promise)
- US-005: View Application List
- US-006: Application Detail View
- US-007: Update Application Status
- US-009: Add/Edit Notes
- US-011: Earn Points for Actions
- US-012: View Total Score
- US-061: Review AI-Parsed Data

**AI-Powered Parsing (4):**
- US-056: AI Job Listing Parser (extracts company, role, skills, salary, requirements)
- US-057: AI Skill Extraction (auto-tags required/nice-to-have skills)
- US-059: AI Salary Range Detection
- US-060: AI Work Mode Detection (Remote/Hybrid/Onsite)

**Social/Collaborative (6):**
- US-018: Create Hunting Party
- US-019: Generate Invite Link
- US-020: Join Hunting Party
- US-021: Leaderboard (All-Time)
- US-023: Rivalry Panel
- US-024: Activity Feed (Party)

**Import/Export (2):**
- US-026: Import Job-Hunt-Context JSON
- US-027: Retroactive Point Calculation

**Total: 22 User Stories, ~105 Story Points** (Aggressive 4-week timeline with AI features)

---

### 3. Check Goals - Missing Features?

**Goal 1: Fast Logging** → ✅ US-004 (Quick Capture)
**Goal 2: Social Accountability** → ✅ US-018, 019, 020, 021, 023, 024 (Hunting parties + leaderboards)
**Goal 3: Preserve Existing Work** → ✅ US-026, 027 (Import with retroactive points)
**Goal 4: Gamify Effort** → ✅ US-011, 012 (Points system + score display)
**Goal 5: Show Progress** → ✅ US-012, 021, 024 (Score, leaderboard, activity feed)

**All prioritized goals covered.** ✅

---

### 4. Remove Unnecessary Features from MVP

**Removed from Phase 1 (Defer to Phase 2-3):**
- ❌ US-008: Timeline Events → Complex, not essential for MVP
- ❌ US-013: Daily Streaks → Gamification enhancement, defer to Phase 2
- ❌ US-015: Achievement Badges → Defer to Phase 2
- ❌ US-022: Weekly Leaderboard → Start with all-time, add weekly in Phase 3
- ❌ US-031: Sankey Diagram → Valuable but complex; Phase 2 priority
- ❌ US-032: Source Attribution → Meta-analysis for Phase 2

**Reasoning:** These features are important long-term but not critical for solving the core problem (isolation + motivation + tracking friction). MVP must ship in 4 weeks; these can follow in Phases 2-3.

---

### 5. MVP Scope Statement

**Big Job Hunter Pro MVP (Phase 1) Scope:**

Big Job Hunter Pro Phase 1 delivers an **AI-powered** collaborative gamified job tracker that enables friend groups to:
1. **Log applications in 10 seconds** via AI-Powered Quick Capture (paste URL + page content, AI extracts all data)
2. **Eliminate manual data entry** - AI automatically parses company, role, required skills, nice-to-have skills, salary range, work mode, location
3. **Get AI-generated interview prep** - AI creates potential interview questions based on job requirements
4. **Import existing data** from Job-Hunt-Context (24+ applications with retroactive points)
5. **Create hunting parties** and invite friends via shareable links
6. **Compete on leaderboards** (all-time rankings by total points)
7. **View rival progress** via rivalry panel showing who's directly ahead and how to close the gap
8. **Celebrate together** via activity feed broadcasting wins (interviews, offers, achievements)
9. **Earn points for effort** (Applied +1, Interview +5, Rejection +5, Offer +50)
10. **Track applications** in a master list with detail view and status updates
11. **Review and edit AI-parsed data** in detail view to correct any errors

**What's IN MVP (New AI Features):**
- ✅ AI job listing parser (company, role, skills, salary extraction)
- ✅ AI skill extraction and tagging
- ✅ AI salary range detection
- ✅ AI work mode detection (Remote/Hybrid/Onsite)
- ✅ AI interview question generator (Phase 1.5 - may defer to Phase 2)

**What's NOT in MVP:**
- Daily streaks (Phase 2)
- Achievement badges (Phase 2)
- Sankey diagrams (Phase 2)
- Weekly leaderboards (Phase 3)
- Browser extension (Phase 4)
- Advanced AI features: Resume feedback, Cover letter generation, Job matching (Future)

**Success Criteria:**
- Friend group (4-6 people) imports existing 24+ applications
- First hunting party created and active
- 30+ new applications logged in Week 4 of launch
- Leaderboard drives competition (confirmed via friend group feedback)

---

## CREATE A PROJECT ROADMAP

### MVP (Phase 1) - Foundation + Social Core
**Timeline:** 4 weeks (Feb 2026)
**Goal:** Solve core problem (isolation + motivation + tracking friction) for friend group

**Features:**
- ✅ User authentication (email/password, OAuth)
- ✅ **AI-Powered Quick Capture** (10-second application logging - paste URL + content)
- ✅ **AI Job Listing Parser** (extracts company, role, skills, salary, requirements automatically)
- ✅ **AI Skill Extraction** (auto-tags required/nice-to-have skills)
- ✅ **AI Salary Detection** (parses various salary formats)
- ✅ **AI Work Mode Detection** (Remote/Hybrid/Onsite identification)
- ✅ Application list + detail view with AI-parsed data
- ✅ Review/edit AI-parsed data interface
- ✅ Status updates (Applied → Screening → Interview → Offer/Rejected)
- ✅ Points system (Applied +1, Interview +5, Rejection +5, Offer +50)
- ✅ Total score dashboard
- ✅ Hunting party creation + invite links
- ✅ Leaderboard (all-time rankings)
- ✅ Rivalry panel ("Close the gap")
- ✅ Activity feed (party member wins)
- ✅ **Import Job-Hunt-Context JSON** (bulk import, retroactive points)
- ✅ Export to JSON (data portability)
- ✅ Notes system (private notes per application)

**Story Points:** 105
**Team:** Solo developer (Christian) with friend group testing
**AI Integration:** Cheap AI API (Claude Haiku, GPT-3.5-turbo, or similar) for parsing
**Deployment:** Azure App Service (API) + Azure Static Web Apps (React)

---

### Version 1.5 (Phase 2) - Gamification Core + Meta-Analysis
**Timeline:** 4 weeks (Mar 2026)
**Goal:** Add motivational hooks (streaks, achievements) + analytical insights (Sankey, source attribution)

**New Features:**
- ✅ Daily streak tracking + fire icon
- ✅ Streak warnings ("6 hours left today!")
- ✅ Achievement badges (First Hunt, 10 Applications, Interview Champion, Resilience Award)
- ✅ Trophy Room (badge display with progress bars)
- ✅ Confetti celebrations (10, 25, 50, 100 application milestones)
- ✅ **Sankey Diagram** (pipeline flow: Prospecting → Applied → Interview → Offer/Rejected)
- ✅ **Source Attribution Analytics** (conversion rates by job board)
- ✅ **Top Skills Analysis** (15 most frequent skills)
- ✅ Work Mode Distribution (Remote/Hybrid/Onsite chart)
- ✅ Applications Over Time (cumulative line chart)
- ✅ Interview Conversion Rate (% reaching interview)
- ✅ Timeline events (detailed progression history)
- ✅ Mobile responsive design (phone, tablet optimization)

**Story Points:** 71
**Focus:** Retention (streaks) + Strategic insights (meta-analysis)

---

### Version 2.0 (Phase 3) - Advanced Social Features + Challenges
**Timeline:** 4 weeks (Apr 2026)
**Goal:** Deepen collaboration (challenges, retrospectives) + competitive layers (weekly leaderboards)

**New Features:**
- ✅ Weekly leaderboards (reset every Monday)
- ✅ Weekly challenges ("Everyone apply to 10 jobs this week")
- ✅ Challenge progress tracking (real-time progress bars)
- ✅ Group retrospective dashboard (weekly stats, top performers, insights)
- ✅ Comparative member analysis (side-by-side stats)
- ✅ Historical rivalry tracking (who's been #1 most weeks)
- ✅ Advanced rivalry panel (show exact gap: "They have 3 more interviews")
- ✅ Privacy controls (hide company names from leaderboard)
- ✅ Anonymous mode (participate without revealing identity)
- ✅ Opt-out leaderboards (solo tracking option)
- ✅ Follow-up reminders ("Check on application after 2 weeks")
- ✅ Streak warnings (push notifications)

**Story Points:** 66
**Focus:** Social depth (challenges, retrospectives) + Privacy options

---

### Version 2.5+ (Phase 4) - Polish, Automation, AI
**Timeline:** 4+ weeks (May 2026+)
**Goal:** Reduce friction (browser extension) + AI enhancements (resume feedback)

**New Features:**
- ✅ PWA installation (install as mobile app)
- ✅ Browser extension (one-click capture from Indeed, LinkedIn)
- ✅ Keyboard shortcuts (Ctrl+K for Quick Capture)
- ✅ Search & filters (by company, role, status, work mode)
- ✅ Bulk actions (update multiple applications)
- ✅ Onboarding tutorial (60-second walkthrough)
- ✅ Dark/Light mode toggle
- ✅ Calendar integration (sync interview dates to Google Calendar)
- ✅ Weekly digest email (stats, leaderboard position)
- ✅ Export to PNG (dashboard screenshot)
- ✅ AI resume feedback (score resume vs. job description)
- ✅ AI cover letter generator (GPT-4 powered)
- ✅ Skill tagging (required/nice-to-have per application)

**Story Points:** 89
**Focus:** Automation (browser extension) + AI assistance + Polish

---

### NEVER WORKING ON (Too Complex or Low Value)

**Deferred Indefinitely:**
- ❌ Email integration (auto-log from Gmail) - Too complex (Story Points: 21)
- ❌ AI job matching (suggest roles) - Requires massive dataset (Story Points: 21)
- ❌ Referral network (connect with employees) - Social networking scope creep
- ❌ Salary data aggregation - Privacy/legal concerns
- ❌ Interview recording transcription - Audio processing complexity
- ❌ Mood tracking correlation - Niche use case, adds clutter

**Reasoning:** These features either:
1. Require months of development (email integration, AI matching)
2. Introduce legal/privacy risks (salary data, interview recordings)
3. Don't align with core mission (social accountability for friend groups)
4. Can be achieved via external tools (Levels.fyi for salary, Otter.ai for transcription)

---

## ROADMAP SUMMARY (Gantt Chart)

```
Phase 1 (MVP)           ████████████████ (4 weeks) Feb 2-Feb 29
  └─ Story Points: 68

Phase 2 (Gamification)          ████████████████ (4 weeks) Mar 2-Mar 29
  └─ Story Points: 71

Phase 3 (Social++)                      ████████████████ (4 weeks) Mar 30-Apr 26
  └─ Story Points: 66

Phase 4 (Polish/AI)                             ████████████████ (4 weeks) Apr 27-May 24
  └─ Story Points: 89
```

**Total Roadmap:** 16 weeks to "feature-complete" v2.5
**Critical Path:** Phase 1 (friend group needs this NOW)

---

## IMPLEMENTATION NOTES

### Phase 1 Risk Mitigation

**Risk:** Import tool breaks with schema mismatches
**Mitigation:** Extensive testing with friend group's 24 files; dry-run preview mode

**Risk:** Leaderboard queries are too slow (LINQ performance)
**Mitigation:** Database indexing on `UserId`, `HuntingPartyId`, `Points`; caching for 5 minutes

**Risk:** Friend group abandons tool if Phase 1 takes >4 weeks
**Mitigation:** Ship hunting party + leaderboard by Week 3 even if import isn't ready

**Risk:** Rivalry panel demotivates instead of motivates
**Mitigation:** Show "next person up" not "#1 leader"; opt-out option; positive framing

### Technical Debt Allowances

**Acceptable Shortcuts in Phase 1:**
- ✅ All-time leaderboard only (no weekly reset) - Add in Phase 3
- ✅ Simple rivalry panel (just points gap) - Enhance in Phase 3
- ✅ Basic activity feed (no filters) - Polish in Phase 2
- ✅ Mobile-responsive but not PWA - PWA in Phase 4

**NOT Acceptable:**
- ❌ Skipping import tool (friend group blocker)
- ❌ Skipping hunting parties (core differentiator)
- ❌ Slow Quick Capture (breaks 15-second promise)

---

## SUCCESS METRICS BY PHASE

### Phase 1 (MVP) Success Criteria

**User Adoption:**
- [ ] Friend group (4-6 people) onboarded
- [ ] 24+ applications imported successfully
- [ ] First hunting party created and active

**Engagement:**
- [ ] 30+ new applications logged collectively in Week 4
- [ ] Daily logins from 4+ friend group members
- [ ] Leaderboard checked 5+ times/day (aggregate)

**Technical:**
- [ ] Quick Capture avg. time <20 seconds (goal: 15s)
- [ ] Leaderboard query <500ms
- [ ] Zero critical bugs blocking core workflow

**Qualitative:**
- [ ] At least 1 friend says "This is helping me apply more"
- [ ] Leaderboard drives competition (confirmed via feedback)

---

### Phase 2 Success Criteria

**Retention:**
- [ ] 30-day retention >15% (industry benchmark)
- [ ] 5+ friend group members maintain 7-day streak
- [ ] 50+ achievements unlocked (aggregate)

**Insights:**
- [ ] Sankey diagram reveals insights (e.g., "50% drop-off at screening")
- [ ] Source attribution identifies best job board (e.g., "Indeed 2x better than LinkedIn")
- [ ] Friend group uses meta-analysis to optimize strategy

**Qualitative:**
- [ ] "The Sankey diagram is motivating" (user feedback)
- [ ] Meta-analysis drives behavioral change (e.g., stop using low-conversion sources)

---

### Phase 3 Success Criteria

**Collaboration:**
- [ ] Friend group completes 4 weekly challenges
- [ ] Weekly retrospective used in 3+ Sunday calls
- [ ] Group analytics identifies actionable patterns

**Viral Growth:**
- [ ] Friend group invites 2+ additional friends (8-10 total users)
- [ ] Second hunting party created (non-friend-group)

**Retention:**
- [ ] Day 7 retention >30%
- [ ] Weekly challenge completion rate >50%

---

### Phase 4 Success Criteria

**Automation:**
- [ ] Browser extension reduces Quick Capture to <10 seconds
- [ ] 20+ users install PWA on mobile
- [ ] AI resume feedback used by 10+ users

**Portfolio Impact:**
- [ ] 3+ technical interviews reference this project
- [ ] Job offer received with this project cited

**Business:**
- [ ] 100+ total signups (friend-of-friends network)
- [ ] Product Hunt launch completed (featured = bonus)

---

## CONCLUSION

This Project Scoping document defines a clear path from concept to shipped product:

1. **Task Flow Analysis** identified 5 critical workflows (logging apps, checking leaderboards, updating status, importing data, retrospectives)
2. **Brainstorming** generated 100 potential features across tracking, gamification, social, analytics, automation
3. **User Stories** converted 58 priority features into actionable stories with importance/feasibility ratings
4. **Prioritization Matrix** categorized features into DO NOW (16 stories, Phase 1), PLAN FOR (14 stories, Phases 2-3), NICE TO HAVE (23 stories, Phase 4), and AVOID (11 stories, distant future)
5. **MVP Definition** scoped Phase 1 to 16 user stories (68 story points) solving 80% of the problem: isolation + motivation + tracking friction
6. **Roadmap** mapped 4 phases over 16 weeks with clear success criteria per phase

**Next Steps:**
1. Review scoping document with friend group (validate priorities)
2. Begin Phase 1 development (Week 1: Backend + Import Command)
3. Ship MVP by Feb 29, 2026
4. Dogfood with friend group, iterate based on feedback
5. Execute Phases 2-4 per roadmap

**We're not just building this. We're using it. We need it to work. Let's hunt together. 🎯**
