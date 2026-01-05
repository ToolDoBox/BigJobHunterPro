# Big Job Hunter Pro — Project Structure Document

**Project:** Big Job Hunter Pro
**Domain:** bigjobhunter.pro
**Version:** v1.0
**Date:** 2026-01-04
**Status:** Architecture & Design Phase

---

## EXECUTIVE SUMMARY

This document defines the architectural foundation, technical standards, information architecture, data models, and style guidelines for Big Job Hunter Pro. It translates the scoping document's feature requirements into concrete structural decisions that will guide implementation.

**Key Architectural Decisions:**
- **Frontend:** React SPA with TypeScript, Tailwind CSS, React Router
- **Backend:** ASP.NET Core 8 Web API with Entity Framework Core
- **Database:** Azure SQL Database (relational for ACID compliance)
- **Hosting:** Azure Static Web Apps (frontend) + Azure App Service (API)
- **Auth:** JWT tokens with OAuth 2.0 (Google, GitHub)
- **Real-time:** SignalR for leaderboard updates and activity feed

---

## PART I: PROJECT STANDARDS

### Functionality Standards

**F1. Core Workflows Must Complete Successfully**
- All primary user flows (Quick Capture, View Applications, Update Status, View Leaderboard) must complete without errors in production
- Error rate for critical paths <0.5% (99.5% success rate)
- All API endpoints must return appropriate HTTP status codes (2xx success, 4xx client error, 5xx server error)

**F2. Data Integrity**
- No application data loss during status updates or imports
- Retroactive point calculation must be mathematically consistent (re-running import yields same point total)
- Leaderboard rankings must reflect current point totals within 5 seconds of score changes

**F3. Cross-Platform Compatibility**
- Application must function on Chrome 120+, Firefox 120+, Safari 17+, Edge 120+
- Mobile browsers (iOS Safari, Chrome Mobile) must support all core features
- Progressive degradation for older browsers (fallback UI without animations)

**F4. Authentication Security**
- All authenticated endpoints require valid JWT token
- Password reset flow must complete within 15 minutes (token expiry)
- OAuth flows must redirect correctly without exposing tokens in URL fragments

---

### Usability Standards

**U1. AI-Powered Quick Capture Promise: 10 Seconds**
- From paste to confirmation, logging an application must average ≤10 seconds
- Two input fields: (1) Job listing URL, (2) Full page content (large textarea)
- Form auto-focuses on first field (URL)
- AI parsing happens asynchronously after submission (user gets instant +1 point feedback)
- Parsed data visible in detail view within 5 seconds
- Validation errors appear inline within 100ms
- Fallback to manual entry if AI parsing fails

**U2. Accessibility (WCAG 2.1 Level AA)**
- All interactive elements keyboard-navigable (tab order logical)
- Color contrast ratio ≥4.5:1 for text, ≥3:1 for large text/UI components
- Screen reader announces score changes, achievement unlocks, leaderboard updates
- Focus indicators visible on all interactive elements

**U3. Mobile-First Responsive Design**
- Touch targets ≥44x44px on mobile (Apple/Android guidelines)
- Forms use appropriate input types (type="email", type="url", inputmode="numeric")
- Navigation accessible via hamburger menu on screens <768px
- Leaderboard scrollable horizontally on narrow screens without layout break

**U4. Feedback & Confirmation**
- All destructive actions (delete application, leave party) require confirmation dialog
- Toast notifications appear for 4 seconds, dismissible via X button
- Loading states visible for operations >300ms (skeleton screens, spinners)

---

### Aesthetics Standards

**A1. Visual Consistency**
- All components follow style guide (defined in Part IV)
- Maximum 3 font weights used across application (Regular 400, Medium 500, Bold 700)
- Icon set consistent (Heroicons or Lucide React throughout, no mixing)

**A2. Gamification Delight**
- Confetti animations at milestones (10, 25, 50, 100 applications)
- Points display animates when incremented (+1, +5 transitions)
- Achievement unlock modal appears with celebratory animation (fade + scale)

**A3. Competitive Edge**
- Leaderboard design emphasizes current user (highlighted row, bold text)
- Rivalry panel uses motivational colors (green for "ahead", amber for "close gap")
- Activity feed uses avatars + timestamps for human connection

---

### Performance Standards

**P1. Initial Load Time**
- First Contentful Paint (FCP) <1.5 seconds on 4G connection
- Time to Interactive (TTI) <3 seconds on 4G connection
- Lighthouse Performance score ≥90 on desktop, ≥80 on mobile

**P2. API Response Times**
- GET /api/applications (list view) <400ms at p95
- POST /api/applications (Quick Capture) <300ms at p95
- GET /api/leaderboard/{partyId} <500ms at p95 (with caching)
- SignalR message delivery <200ms latency

**P3. Database Query Performance**
- Application list queries use indexes on UserId, Status, CreatedDate
- Leaderboard aggregation cached for 5 minutes (Redis or in-memory)
- N+1 queries eliminated via EF Core .Include() or projection

**P4. Client-Side Performance**
- React component re-renders minimized (React.memo, useMemo, useCallback)
- Virtualized lists for >50 applications (react-window or similar)
- Images lazy-loaded below fold, optimized WebP format

---

### Content Quality Standards

**C1. Microcopy Consistency**
- Tone: Motivational but not cheesy ("Close the gap" not "You're almost there, champ!")
- Voice: Active, direct ("Log your first application" not "Applications can be logged")
- Terminology: Consistent ("Hunting Party" not "Team" or "Group")

**C2. Error Messages**
- Actionable guidance ("Email already registered. Try logging in or reset your password.")
- No technical jargon exposed to users (no stack traces, error codes hidden unless dev mode)
- Validation messages appear inline near relevant field

**C3. Empty States**
- Illustrations + actionable CTA ("No applications yet. Log your first hunt!")
- Leaderboard empty state: "Invite friends to start competing"
- Activity feed empty state: "Your party's achievements will appear here"

---

### Development Standards

**D1. Code Organization**
- Frontend: Feature-based folder structure (features/applications, features/leaderboard)
- Backend: Clean Architecture (API → Application → Domain → Infrastructure layers)
- Shared types: TypeScript interfaces mirror C# DTOs (code generation via NSwag or similar)

**D2. Naming Conventions**
- **C# Backend:** PascalCase classes/methods, camelCase parameters, _camelCase private fields
- **TypeScript Frontend:** PascalCase components, camelCase functions/variables, SCREAMING_SNAKE_CASE constants
- **CSS:** BEM methodology (block__element--modifier) or Tailwind utility classes
- **Database:** PascalCase tables (Applications, Users), PascalCase columns (CreatedDate, UserId)

**D3. Version Control**
- Git flow: main (production), develop (integration), feature/* branches
- Commit messages: Conventional Commits format (`feat:`, `fix:`, `refactor:`, `docs:`)
- Pull requests require 1 approval (friend group peer review)
- No direct commits to main branch

**D4. Testing Requirements**
- Unit tests for business logic (services, utilities) ≥70% coverage
- Integration tests for critical API endpoints (Quick Capture, Import, Leaderboard)
- E2E tests for core user flows (Playwright or Cypress: register → log application → view leaderboard)

---

## PART II: INFORMATION ARCHITECTURE

### Content Audit: Application Sections

**Primary Sections (Authenticated Users):**
1. **Dashboard** - Personal stats, score, streak, quick actions
2. **Applications** - Master list, filters, search
3. **Hunting Party** - Leaderboard, activity feed, rivalry panel
4. **Analytics** - Sankey diagram, source attribution, skill analysis (Phase 2+)
5. **Profile** - Settings, import/export, privacy controls

**Public Sections (Unauthenticated):**
1. **Landing Page** - Hero, features, friend group testimonials, CTA
2. **Login/Register** - OAuth options, email/password forms
3. **About** - Mission, friend group story, tech stack

---

### User's Perspective: Navigation Expectations

**User Type 1: Friend Group Member (Primary)**
- Expectations: Fast access to Quick Capture, leaderboard always visible
- Mental model: "Dashboard shows my stats, Hunting Party shows competition"
- Navigation preference: Sidebar on desktop, bottom nav on mobile

**User Type 2: Solo User (Secondary)**
- Expectations: Focus on personal analytics, opt-out of social features
- Mental model: "Applications page is my workspace, Analytics shows insights"
- Navigation preference: Minimal social distractions, prominent analytics link

**User Type 3: New User (Onboarding)**
- Expectations: Guided setup (import data or start fresh, create/join party)
- Mental model: "I need to import my existing applications first"
- Navigation preference: Step-by-step wizard, skip option available

---

### Site Structure: Page Hierarchy

```
Big Job Hunter Pro (bigjobhunter.pro)
│
├── / (Landing Page) [Public]
│   ├── Hero + CTA
│   ├── Features Overview
│   ├── Friend Group Testimonials
│   └── Pricing (Free during beta)
│
├── /login [Public]
│   ├── Email/Password Form
│   └── OAuth Buttons (Google, GitHub)
│
├── /register [Public]
│   ├── Email/Password Form
│   └── OAuth Buttons
│
├── /app (Authenticated Shell)
│   │
│   ├── /app/dashboard [Default Landing]
│   │   ├── Personal Stats Widget (Score, Streak, Applications Count)
│   │   ├── Quick Capture Button (Prominent)
│   │   ├── Recent Activity (Personal)
│   │   └── Hunting Party Snapshot (Leaderboard Top 3)
│   │
│   ├── /app/applications
│   │   ├── List View (Sortable, Filterable)
│   │   ├── Search Bar
│   │   ├── /app/applications/new (Quick Capture Modal)
│   │   └── /app/applications/{id} (Detail View - Master/Detail Layout)
│   │       ├── Application Info
│   │       ├── Timeline Events (Phase 2)
│   │       ├── Notes
│   │       └── Edit/Delete Actions
│   │
│   ├── /app/party (Hunting Party Hub)
│   │   ├── Leaderboard (All-Time)
│   │   ├── Weekly Leaderboard (Phase 3)
│   │   ├── Rivalry Panel
│   │   ├── Activity Feed
│   │   ├── /app/party/create (Create New Party)
│   │   ├── /app/party/join/{inviteCode} (Join via Invite Link)
│   │   └── /app/party/{id}/settings (Manage Party - Creator Only)
│   │
│   ├── /app/analytics (Phase 2+)
│   │   ├── Sankey Diagram
│   │   ├── Source Attribution Chart
│   │   ├── Top Skills Table
│   │   ├── Work Mode Distribution
│   │   └── Applications Over Time
│   │
│   ├── /app/achievements (Phase 2+)
│   │   ├── Trophy Room
│   │   ├── Badge Grid (Unlocked + Locked with Progress)
│   │   └── Rarity Indicators
│   │
│   ├── /app/profile
│   │   ├── Account Settings (Email, Password, OAuth Connections)
│   │   ├── Privacy Settings (Hide Companies, Anonymous Mode, Opt-Out)
│   │   ├── Import Data (Job-Hunt-Context JSON, CSV)
│   │   ├── Export Data (JSON, CSV, PNG)
│   │   └── Delete Account
│   │
│   └── /app/onboarding (First-Time User Flow)
│       ├── Step 1: Import or Start Fresh?
│       ├── Step 2: Create or Join Hunting Party?
│       └── Step 3: Log First Application (Tutorial)
│
└── /invite/{inviteCode} [Public → Auth Required]
    └── Redirect to /app/party/join/{inviteCode}
```

---

### Navigation Design

**Desktop (≥1024px):**
- **Left Sidebar (Fixed):**
  - Logo + App Name
  - Dashboard (icon + label)
  - Applications (icon + label)
  - Hunting Party (icon + label + notification badge)
  - Analytics (icon + label, Phase 2+)
  - Achievements (icon + label, Phase 2+)
  - Profile (icon + label)
  - Quick Capture Button (Floating Action Button)

**Tablet (768px - 1023px):**
- **Left Sidebar (Collapsible):**
  - Icons only (hover shows tooltip)
  - Expand button reveals labels

**Mobile (<768px):**
- **Top App Bar:**
  - Hamburger menu (opens slide-out nav)
  - Page title (centered)
  - Notification bell (right)
- **Bottom Navigation (Fixed):**
  - Dashboard, Applications, Party, Profile icons
  - Quick Capture (center, elevated button)

**Common Elements (All Breakpoints):**
- **Header:** User avatar (top-right), score display, streak fire icon
- **Quick Capture Modal:** Accessible via Ctrl+K shortcut or FAB button
- **Toast Notifications:** Top-right corner, auto-dismiss 4s

---

## PART III: HIGH-LEVEL DATA MODELING

### Entity-Relationship Diagram (ERD)

```
┌─────────────────┐         ┌──────────────────┐
│  User           │────────<│  Application     │
├─────────────────┤  1:N    ├──────────────────┤
│ UserId (PK)     │         │ ApplicationId(PK)│
│ Email           │         │ UserId (FK)      │
│ PasswordHash    │         │ CompanyName      │
│ DisplayName     │         │ RoleTitle        │
│ AvatarUrl       │         │ SourceUrl        │
│ TotalPoints     │         │ Status           │
│ CurrentStreak   │         │ WorkMode         │
│ CreatedDate     │         │ Points           │
└─────────────────┘         │ CreatedDate      │
        │                   │ UpdatedDate      │
        │                   └──────────────────┘
        │                            │
        │ N:M                        │ 1:N
        │                            │
        ▼                            ▼
┌─────────────────┐         ┌──────────────────┐
│ HuntingPartyMbr │         │  Note            │
├─────────────────┤         ├──────────────────┤
│ UserId (FK)     │         │ NoteId (PK)      │
│ PartyId (FK)    │         │ ApplicationId(FK)│
│ Role            │         │ Content          │
│ JoinedDate      │         │ CreatedDate      │
└─────────────────┘         └──────────────────┘
        │
        │ N:1
        │
        ▼
┌─────────────────┐         ┌──────────────────┐
│ HuntingParty    │────────<│  ActivityEvent   │
├─────────────────┤  1:N    ├──────────────────┤
│ PartyId (PK)    │         │ EventId (PK)     │
│ PartyName       │         │ PartyId (FK)     │
│ InviteCode      │         │ UserId (FK)      │
│ CreatorId (FK)  │         │ EventType        │
│ CreatedDate     │         │ PointsAwarded    │
└─────────────────┘         │ Timestamp        │
        │                   └──────────────────┘
        │ 1:N
        │
        ▼
┌─────────────────┐
│ LeaderboardEntry│
├─────────────────┤
│ EntryId (PK)    │
│ PartyId (FK)    │
│ UserId (FK)     │
│ Rank            │
│ TotalPoints     │
│ WeeklyPoints    │ (Phase 3)
│ LastUpdated     │
└─────────────────┘

┌─────────────────┐         ┌──────────────────┐
│ User            │────────<│  Achievement     │
├─────────────────┤  N:M    ├──────────────────┤
│ UserId (PK)     │         │ AchievementId(PK)│
└─────────────────┘         │ Name             │
        │                   │ Description      │
        │                   │ IconUrl          │
        │ N:M               │ PointThreshold   │
        │                   └──────────────────┘
        ▼                            │
┌─────────────────┐                 │ 1:N
│ UserAchievement │                 │
├─────────────────┤                 ▼
│ UserId (FK)     │         ┌──────────────────┐
│ AchievementId(FK)│        │  Skill           │
│ UnlockedDate    │         ├──────────────────┤
└─────────────────┘         │ SkillId (PK)     │
                            │ Name             │
                            └──────────────────┘
                                     │
                                     │ N:M
                                     ▼
                            ┌──────────────────┐
                            │ ApplicationSkill │
                            ├──────────────────┤
                            │ ApplicationId(FK)│
                            │ SkillId (FK)     │
                            │ IsRequired       │
                            └──────────────────┘
```

---

### Entity Definitions

#### **User**
- **UserId (Guid, PK):** Unique identifier
- **Email (string, unique, indexed):** Primary login credential
- **PasswordHash (string, nullable):** Null if OAuth-only
- **DisplayName (string):** Public name shown in leaderboard/activity feed
- **AvatarUrl (string, nullable):** Profile picture URL
- **TotalPoints (int, computed):** Sum of all ApplicationPoints earned
- **CurrentStreak (int):** Consecutive days with ≥1 action
- **LastActivityDate (DateTime):** For streak calculation
- **CreatedDate (DateTime):** Account creation timestamp
- **Relationships:**
  - 1:N with Applications
  - N:M with HuntingParties (via HuntingPartyMembership)
  - N:M with Achievements (via UserAchievement)

#### **Application**
- **ApplicationId (Guid, PK):** Unique identifier
- **UserId (Guid, FK, indexed):** Owner of application
- **CompanyName (string, required):** Job company name (AI-extracted or manual)
- **RoleTitle (string, required):** Job position title (AI-extracted or manual)
- **SourceUrl (string, nullable):** Link to job posting
- **Source (enum, nullable):** Indeed, LinkedIn, Referral, CompanyWebsite, Other
- **Status (enum, required):** Prospecting, Applied, Screening, Interview, Offer, Rejected, Withdrawn
- **WorkMode (enum, nullable):** Remote, Hybrid, Onsite (AI-extracted)
- **Location (string, nullable):** City, State, Country (AI-extracted)
- **SalaryMin (int, nullable):** Minimum salary (stored in thousands: 120 = $120k) (AI-extracted)
- **SalaryMax (int, nullable):** Maximum salary (AI-extracted)
- **JobDescription (string, nullable, max 10000 chars):** AI-generated summary of role
- **RawPageContent (string, nullable, max 50000 chars):** Original pasted content for re-parsing
- **ParsedByAI (bool):** True if data was AI-extracted, false if manual entry
- **AIParsingStatus (enum):** Pending, Success, Failed, ManualOverride
- **AIParsingError (string, nullable):** Error message if parsing failed
- **Points (int, computed):** Points awarded for this application (Applied=1, Interview=5, Rejected=5, Offer=50)
- **CreatedDate (DateTime, indexed):** When logged
- **UpdatedDate (DateTime):** Last modification timestamp
- **LastAIParsedDate (DateTime, nullable):** When AI last processed this application
- **Relationships:**
  - N:1 with User
  - 1:N with Notes
  - 1:N with InterviewQuestions (AI-generated)
  - N:M with Skills (via ApplicationSkill)

#### **HuntingParty**
- **PartyId (Guid, PK):** Unique identifier
- **PartyName (string, required):** Display name ("The Job Hunters", "Squad Goals")
- **InviteCode (string, unique, indexed):** Short code for invite links (e.g., "JH42X9")
- **CreatorId (Guid, FK):** User who created party
- **IsPublic (bool):** Allow anyone to join (Phase 3+)
- **CreatedDate (DateTime):** Party creation timestamp
- **Relationships:**
  - N:M with Users (via HuntingPartyMembership)
  - 1:N with ActivityEvents
  - 1:N with LeaderboardEntries

#### **HuntingPartyMembership** (Join Table)
- **UserId (Guid, FK, composite PK):** Member user
- **PartyId (Guid, FK, composite PK):** Hunting party
- **Role (enum):** Creator, Member
- **JoinedDate (DateTime):** When user joined
- **IsActive (bool):** True unless user leaves party

#### **ActivityEvent**
- **EventId (Guid, PK):** Unique identifier
- **PartyId (Guid, FK, indexed):** Which party sees this event
- **UserId (Guid, FK):** User who performed action
- **EventType (enum):** ApplicationLogged, InterviewCompleted, OfferReceived, AchievementUnlocked, RejectionLogged
- **ApplicationId (Guid, FK, nullable):** Related application (if applicable)
- **AchievementId (Guid, FK, nullable):** Related achievement (if applicable)
- **PointsAwarded (int):** Points earned from this event
- **Timestamp (DateTime, indexed):** When event occurred
- **Relationships:**
  - N:1 with HuntingParty
  - N:1 with User

#### **LeaderboardEntry** (Computed/Cached Table)
- **EntryId (Guid, PK):** Unique identifier
- **PartyId (Guid, FK, indexed):** Leaderboard scope
- **UserId (Guid, FK, indexed):** Ranked user
- **Rank (int):** Position in leaderboard (1-based)
- **TotalPoints (int):** All-time points total
- **WeeklyPoints (int, Phase 3):** Points earned this week (Monday reset)
- **ApplicationCount (int):** Total applications logged
- **InterviewCount (int):** Total interviews completed
- **LastUpdated (DateTime):** Cache timestamp (invalidate after 5 min)

#### **Note**
- **NoteId (Guid, PK):** Unique identifier
- **ApplicationId (Guid, FK, indexed):** Related application
- **Content (string, max 5000 chars):** Markdown-formatted note
- **CreatedDate (DateTime):** When note was created
- **UpdatedDate (DateTime):** Last edit timestamp

#### **Achievement** (Seeded Master Data)
- **AchievementId (Guid, PK):** Unique identifier
- **Name (string):** "First Hunt", "Century Club", "Interview Champion"
- **Description (string):** "Log your first application"
- **IconUrl (string):** URL to badge icon
- **Category (enum):** Milestone, Streak, Social, Resilience
- **PointThreshold (int, nullable):** Trigger (100 applications = 100)
- **Relationships:**
  - N:M with Users (via UserAchievement)

#### **UserAchievement** (Join Table)
- **UserId (Guid, FK, composite PK):** User who unlocked
- **AchievementId (Guid, FK, composite PK):** Achievement unlocked
- **UnlockedDate (DateTime):** When earned

#### **Skill** (Master Data + User-Created)
- **SkillId (Guid, PK):** Unique identifier
- **Name (string, unique):** "React", "TypeScript", "Python"
- **Category (enum, nullable):** Frontend, Backend, DevOps, Soft Skills
- **IsVerified (bool):** True for system-seeded skills, false for user-created

#### **ApplicationSkill** (Join Table)
- **ApplicationId (Guid, FK, composite PK):** Related application
- **SkillId (Guid, FK, composite PK):** Required skill
- **IsRequired (bool):** True = required, False = nice-to-have

#### **InterviewQuestion** (AI-Generated)
- **QuestionId (Guid, PK):** Unique identifier
- **ApplicationId (Guid, FK, indexed):** Related application
- **Question (string, max 500 chars):** AI-generated interview question
- **Category (enum):** Technical, Behavioral, ProjectExperience, CompanyCulture, Situational
- **Difficulty (enum):** Easy, Medium, Hard
- **GeneratedDate (DateTime):** When AI created this question
- **UserNotes (string, nullable, max 2000 chars):** User's prep notes for this question
- **Relationships:**
  - N:1 with Application

---

### Cardinality Summary

| Relationship | Cardinality | Notes |
|--------------|-------------|-------|
| User ↔ Application | 1:N | One user owns many applications |
| User ↔ HuntingParty | N:M | Users can join multiple parties (Phase 3+) |
| HuntingParty ↔ ActivityEvent | 1:N | Party has many activity events |
| Application ↔ Note | 1:N | Application can have multiple notes |
| Application ↔ InterviewQuestion | 1:N | Application can have multiple AI-generated interview questions |
| Application ↔ Skill | N:M | Applications require multiple skills, skills appear in many apps |
| User ↔ Achievement | N:M | Users unlock many achievements, achievements unlocked by many users |

---

### Database Indexes (Performance Critical)

```sql
-- User indexes
CREATE INDEX IX_User_Email ON Users(Email);
CREATE INDEX IX_User_TotalPoints ON Users(TotalPoints DESC); -- Leaderboard sorting

-- Application indexes
CREATE INDEX IX_Application_UserId ON Applications(UserId);
CREATE INDEX IX_Application_CreatedDate ON Applications(CreatedDate DESC); -- Chronological sorting
CREATE INDEX IX_Application_Status ON Applications(Status); -- Filtering

-- HuntingParty indexes
CREATE UNIQUE INDEX IX_HuntingParty_InviteCode ON HuntingParties(InviteCode);

-- LeaderboardEntry indexes
CREATE INDEX IX_LeaderboardEntry_PartyId_Rank ON LeaderboardEntries(PartyId, Rank);
CREATE INDEX IX_LeaderboardEntry_UserId ON LeaderboardEntries(UserId);

-- ActivityEvent indexes
CREATE INDEX IX_ActivityEvent_PartyId_Timestamp ON ActivityEvents(PartyId, Timestamp DESC);
```

---

## PART IV: JOB APPLICATION JSON SCHEMA

### Overview

Big Job Hunter Pro uses a standardized JSON format (Schema v1) for representing job applications with rich metadata. This format supports both manual data entry and bulk imports from external tracking systems (e.g., Job-Hunt-Context JSON files).

**Schema Version:** `v1`
**Purpose:** Complete application lifecycle tracking with timeline events, contacts, skills, and interview preparation
**Primary Use Cases:**
1. **Bulk Import:** Import historical applications with retroactive point calculation
2. **Data Export:** Export user applications for backup or external analysis
3. **API Interchange:** Optional detailed format for advanced API consumers (standard API uses simpler DTOs)

---

### Core Schema Structure

#### Top-Level Metadata
```json
{
  "schema_version": "v1",
  "id": "2025-11-23-american-auto-auction-group-junior-devops-engineer",
  "created_at": "2025-12-14T00:00:00Z",
  "updated_at": "2025-12-17T21:30:26Z"
}
```

**Fields:**
- **schema_version** (string, required): Always `"v1"` for current format
- **id** (string, required): Unique identifier format: `YYYY-MM-DD-company-slug-role-slug`
- **created_at** (ISO-8601 timestamp, required): When application was first logged
- **updated_at** (ISO-8601 timestamp, required): Last modification timestamp

#### Company Information
```json
{
  "company": {
    "name": "American Auto Auction Group LLC",
    "industry": "Vehicle auction and remarketing",
    "location": "Dallas, TX",
    "rating": {
      "value": 4.2,
      "scale": 5,
      "count": 128,
      "source": "Glassdoor"
    },
    "website": "https://americasaa.com"
  }
}
```

**Legacy Format Support (Import Only):**
```json
{
  "company_name": "American Auto Auction Group LLC",
  "company_location": "Dallas, TX",
  "company_industry": "Vehicle auction and remarketing"
}
```

**Normalization:** Import parser converts legacy `company_*` fields to nested `company` object.

#### Role Information
```json
{
  "role": {
    "title": "Mid-Level .NET DevOps Engineer",
    "level": "mid",
    "function": "devops",
    "department": "Engineering",
    "employment_type": "full_time",
    "summary": "DevOps engineer focused on .NET 8 modernization and legacy system migration",
    "alternate_titles": ["Application Developer-.NET", "Junior DevOps Engineer"]
  }
}
```

**Controlled Vocabularies:**
- **level:** `entry`, `junior`, `mid`, `senior`, `lead`, `principal`, `staff`, `executive`
- **employment_type:** `full_time`, `part_time`, `contract`, `temporary`, `internship`

**Legacy Format Support:**
- `role_title` → `role.title`
- `role_level` → `role.level`

#### Location & Work Mode
```json
{
  "location": {
    "raw": "Dallas, TX",
    "city": "Dallas",
    "state": "TX",
    "postal_code": null,
    "country": "USA",
    "work_mode": "remote"
  }
}
```

**Controlled Vocabularies:**
- **work_mode:** `remote`, `hybrid`, `onsite`, `null`

**Legacy Format Support:**
- `work_mode` (top-level) → `location.work_mode`
- `company_location` → `location.raw`

#### Source Attribution
```json
{
  "source": {
    "type": "job_board",
    "name": "ZipRecruiter",
    "url": "https://www.ziprecruiter.com/c/Americas-Auto-Auction/Job/...",
    "external_id": "64eac5bfc311dc4f"
  }
}
```

**Controlled Vocabularies:**
- **type:** `job_board`, `referral`, `recruiter`, `company_website`, `linkedin`, `other`

**Legacy Format Support:**
- `source_channel` → `source.type`
- `source_platform` → `source.name`
- `job_post_url` → `source.url`

#### Compensation
```json
{
  "compensation": {
    "currency": "USD",
    "salary_min": 80000,
    "salary_max": 90000,
    "salary_unit": "yearly",
    "bonus": null,
    "equity": null,
    "notes": "Recruiter stated pay range 80–90K. Candidate indicated 80–83K is acceptable."
  }
}
```

**Controlled Vocabularies:**
- **salary_unit:** `hourly`, `yearly`, `null`

**Legacy Format Support:**
- `salary_currency` → `compensation.currency`
- `salary_min` → `compensation.salary_min`
- `salary_max` → `compensation.salary_max`
- `salary_type` → `compensation.salary_unit`
- `comp_notes` → `compensation.notes`

#### Status Tracking
```json
{
  "status": {
    "stage": "interview",
    "state": "closed"
  },
  "final_outcome": "rejected",
  "final_outcome_date": "2025-12-30"
}
```

**Controlled Vocabularies:**
- **stage:** `applied`, `screening`, `interview`, `offer`, `accepted`, `rejected`, `withdrawn`
- **state:** `open`, `closed`
- **final_outcome:** `pending`, `accepted`, `rejected`, `withdrawn`, `ghosted`

**Legacy Format Support:**
- `current_stage` → `status.stage`
- `current_status` → derived from `status.stage` + context

#### Skills
```json
{
  "skills": {
    "required": [
      ".NET / C#",
      ".NET 8",
      "ASP.NET (API controllers)",
      "Queue services and queue processing",
      "Legacy system modernization (strangler pattern)",
      "Troubleshooting (application + environment)",
      "Basic DevOps concepts (deployments, environment reliability)",
      "CI/CD fundamentals"
    ],
    "nice_to_have": [
      "SQL Server",
      "PowerShell",
      "GitHub",
      "Python scripting",
      "Monitoring and alerting tools",
      "Infrastructure automation (Terraform/Ansible)",
      "Cloud exposure (Azure/AWS)",
      "Docker/Kubernetes"
    ]
  },
  "skill_tags": [
    "devops",
    "windows",
    "dotnet",
    "net8",
    "aspnet",
    "iis",
    "ci_cd",
    "automation"
  ]
}
```

**Import Behavior:**
- `required` skills → `ApplicationSkill.IsRequired = true`
- `nice_to_have` skills → `ApplicationSkill.IsRequired = false`
- `skill_tags` → Searchable metadata, not stored in ApplicationSkill (stored in JSON column or separate Tags table)

**Legacy Format Support:**
- `required_skills` → `skills.required`
- `nice_to_have_skills` → `skills.nice_to_have`

#### Contacts
```json
{
  "contacts": [
    {
      "id": "contact_1",
      "name": "Adalia Moran",
      "role": "Corporate Recruiter",
      "relationship": "company_recruiter",
      "email": "adalia.moran@americasaa.com",
      "phone": "Office: 317/689-7509; Mobile: 463/336-3547",
      "linkedin": null,
      "notes": "Conducted initial virtual screen on Monday, Dec 15, 2025."
    },
    {
      "id": "contact_2",
      "name": "Tim Ochs",
      "role": "Team Lead",
      "relationship": "hiring_manager",
      "email": "Tim.Ochs@americasaa.com",
      "phone": null,
      "linkedin": null,
      "notes": "Team lead for the role; direct manager."
    }
  ]
}
```

**Controlled Vocabularies:**
- **relationship:** `recruiter`, `company_recruiter`, `hiring_manager`, `team_member`, `referral`, `other`

**Import Behavior:**
- Contacts are imported but not exposed in MVP UI (Phase 2+ feature)
- Stored in JSON column on Application table or separate Contact table

#### Timeline Events
```json
{
  "timeline_events": [
    {
      "event_type": "application_submitted",
      "stage": "applied",
      "direction": "outgoing",
      "channel": "job_board",
      "at": "2025-11-23T00:00:00-06:00",
      "actor": "candidate",
      "summary": "Applied on Indeed",
      "notes": "Applied on Nov 23, 2025 via Indeed for Junior DevOps Engineer listing."
    },
    {
      "event_type": "interview",
      "stage": "interview",
      "direction": "other",
      "channel": "video",
      "at": "2025-12-15T09:00:00-06:00",
      "actor": "company",
      "summary": "Recruiter screen completed (Adalia Moran)",
      "notes": "Key details captured: company expanded to ~50 locations..."
    },
    {
      "event_type": "email_received",
      "stage": "closed",
      "direction": "incoming",
      "channel": "email",
      "at": "2025-12-30T07:10:00-06:00",
      "actor": "recruiter",
      "summary": "Recruiter notified: offer extended to another candidate",
      "notes": "Email from Adalia Moran: thanked candidate for interviewing..."
    }
  ]
}
```

**Controlled Vocabularies:**
- **event_type:** `sourced`, `application_submitted`, `email_received`, `email_sent`, `phone_call`, `voicemail_received`, `interview`, `screening`, `offer`, `follow_up`, `rejected`, `withdrawn`, `accepted`
- **stage:** `applied`, `screening`, `interview`, `offer`, `accepted`, `rejected`, `withdrawn`, `closed`
- **direction:** `inbound`, `outbound`, `incoming`, `outgoing`, `other`
- **channel:** `job_board`, `email`, `phone`, `video`, `video_call`, `in_person`, `other`
- **actor:** `candidate`, `user`, `recruiter`, `hiring_manager`, `company`, `system`, string (freeform)

**Import Behavior:**
- Timeline events drive **retroactive point calculation** during import
- Point awards calculated based on event_type and stage:
  - `application_submitted` → +1 point
  - First `screening` or `interview` event with `stage: "screening"` → +2 points
  - Each `interview` event with `stage: "interview"` → +5 points
  - `rejected` or final rejection event → +5 points (resilience bonus)
  - `offer` event → +50 points
- Events stored chronologically for timeline visualization (Phase 2+)

#### Requirements
```json
{
  "requirements": {
    "education": {
      "level": "bachelor",
      "majors": ["Computer Science", "Software Engineering"],
      "experience_in_lieu_acceptable": true
    },
    "experience": {
      "years_min": 2,
      "years_max": 5,
      "notes": ["2+ years professional .NET development", "DevOps exposure preferred"]
    },
    "certifications": {
      "required": [],
      "nice_to_have": ["AWS Certified Developer", "Microsoft Azure Administrator"]
    }
  }
}
```

**Legacy Format Support:**
- `years_experience_required` → `requirements.experience.years_min`
- `education_required` → `requirements.education.level`

#### Benefits
```json
{
  "benefits": [
    "401(k) matching",
    "Health insurance",
    "Dental insurance",
    "Vision insurance",
    "Paid time off",
    "Parental leave"
  ]
}
```

#### Interview Preparation (Custom Extension)
```json
{
  "interviewPrep": {
    "preparedDate": "2025-12-15",
    "experienceGaps": {
      "critical": [
        {
          "skill": "Windows Server & IIS Administration",
          "gap": "Limited direct IIS/Windows Server hosting experience",
          "specifics": "App pools, bindings, deployment troubleshooting, logging"
        }
      ],
      "secondary": [
        {
          "skill": "Monitoring and alerting tools",
          "gap": "Not deeply used in prior role",
          "specifics": "Dashboards, alert thresholds, incident response"
        }
      ]
    },
    "strengths": [
      {
        "area": "C# & .NET",
        "experience": "Multi-year professional experience",
        "details": "Built end-to-end generators, strong troubleshooting and automation mindset."
      }
    ],
    "questionsToAsk": [
      "Is the environment Windows-only, or is there Linux as well?",
      "How often do you deploy/releases happen?",
      "What CI/CD tooling is currently in place?"
    ]
  }
}
```

**Import Behavior:**
- `interviewPrep` object is preserved during import but not mapped to database entities in MVP
- Stored in JSON column on Application table for future display (Phase 3+)

#### Interview Questions (AI-Inferred)
```json
{
  "interview_questions_asked_inferred": [
    {
      "at": "2025-12-29T11:00:00-06:00",
      "stage": "interview",
      "question": "Can you introduce yourself and summarize your background?",
      "evidence": "Candidate gives a structured intro (All Kids Network role, generators, handwriting project)."
    },
    {
      "at": "2025-12-29T11:00:00-06:00",
      "stage": "interview",
      "question": "Why are you interested in this role / why are you looking to change jobs now?",
      "evidence": "Candidate answers with growth (full stack) and necessity (hours reduced due to ad revenue changes)."
    }
  ]
}
```

**Import Behavior:**
- Imported questions populate the `InterviewQuestion` entity
- `evidence` field stored in `InterviewQuestion.UserNotes`
- Questions are linked to the application via `ApplicationId` foreign key

#### General Notes & Next Actions
```json
{
  "general_notes": "Initial application was to an Indeed 'Junior DevOps Engineer' posting. During recruiter screen on Dec 15, 2025, recruiter stated the junior role is already filled...",
  "next_action": {
    "action": "Follow up with recruiter to confirm and schedule the next technical interview",
    "due_date": "2025-12-29",
    "notes": "Prepare by reviewing C#/.NET 8 fundamentals, ASP.NET API patterns..."
  },
  "risk_flags": [
    "Public posting for a 'Mid-Level .NET DevOps Engineer' could not be located",
    "Significant legacy modernization effort: ~20-year-old C codebase"
  ]
}
```

**Legacy Format Support:**
- `general_notes` (string) → `general_notes` (array of strings in v1)
- `next_action` (string) → `next_action` (object with action, due_date, notes)

---

### Schema Validation Rules

**Required Fields:**
- `schema_version` (must be `"v1"`)
- `id` (format: `YYYY-MM-DD-company-slug-role-slug`)
- `created_at` (ISO-8601 timestamp)
- `updated_at` (ISO-8601 timestamp)
- `company.name` (fallback to "Unknown" if missing during import)
- `role.title` (fallback to "Unknown" if missing during import)
- `status.stage` (default: `"applied"` if missing)
- `status.state` (default: `"open"` if missing)

**Optional Fields:**
- All other fields are nullable or can be empty arrays
- Unknown/missing values → `null`
- Empty collections → `[]`

**Date/Timestamp Formats:**
- **Dates Only:** `YYYY-MM-DD` (e.g., `2025-11-23`)
- **Timestamps:** ISO-8601 with timezone (e.g., `2025-12-14T00:00:00Z`, `2025-11-23T00:00:00-06:00`)

**ID Format:**
- Pattern: `YYYY-MM-DD-company-slug-role-slug`
- Example: `2025-11-23-american-auto-auction-group-junior-devops-engineer`
- Slug Rules: Lowercase, hyphens instead of spaces, alphanumeric only

---

### Import Workflow

**Step 1: File Upload**
- User uploads JSON file(s) via `/app/profile` → Import Data
- Frontend validates JSON structure (schema_version, required fields)
- Backend receives file via `POST /api/import/applications`

**Step 2: Parsing & Normalization**
- Backend parses JSON and normalizes legacy field names to schema v1
- Validates required fields, controlled vocabularies, date formats
- Generates internal `ApplicationId` (Guid), maps `id` field to `ExternalId` column

**Step 3: Duplicate Detection**
- Check for existing applications by `ExternalId` or (`CompanyName`, `RoleTitle`, `CreatedDate`)
- Skip duplicates or prompt user for merge/overwrite behavior

**Step 4: Entity Mapping**
- Map JSON to database entities:
  - `Application` (core fields)
  - `ApplicationSkill` (required + nice-to-have skills)
  - `InterviewQuestion` (AI-inferred questions)
  - `Note` (general_notes → individual Note records, Phase 2+)
  - `Contact` (Phase 2+)
  - Timeline events stored in JSON column or separate `TimelineEvent` table (Phase 2+)

**Step 5: Retroactive Point Calculation**
- Parse `timeline_events` chronologically
- Award points based on event types:
  - `application_submitted` → +1
  - First `screening` event → +2
  - Each `interview` event → +5
  - `rejected` event → +5
  - `offer` event → +50
- Sum total points, update `Application.Points` and `User.TotalPoints`

**Step 6: Leaderboard Update**
- Recalculate user's rank in all joined hunting parties
- Broadcast `LeaderboardUpdated` event via SignalR to all party members

**Step 7: Activity Feed**
- Generate single `ActivityEvent`: `BulkImportCompleted` with total applications count and points earned
- Broadcast to hunting party activity feed

**Step 8: Response**
```json
{
  "importId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "status": "completed",
  "summary": {
    "totalApplications": 42,
    "imported": 38,
    "skipped": 4,
    "totalPointsAwarded": 287
  },
  "errors": [
    {
      "applicationId": "2025-10-15-company-x-role-y",
      "error": "Invalid date format for created_at"
    }
  ]
}
```

---

### Export Workflow

**Endpoint:** `GET /api/export/applications?format=json`

**Output:**
- Array of Application objects in schema v1 format
- Includes all fields from database (contacts, timeline_events, interview_questions)
- Filename: `bigjobhunter-export-{userId}-{timestamp}.json`

**Example:**
```json
{
  "export_version": "v1",
  "exported_at": "2026-01-04T15:30:00Z",
  "user": {
    "id": "user-guid-123",
    "displayName": "Jordan Smith",
    "totalPoints": 287
  },
  "applications": [
    { /* Full schema v1 application object */ },
    { /* ... */ }
  ]
}
```

---

### Integration with Database Schema

**Application Entity Mapping:**

| JSON Field | Database Column | Notes |
|------------|----------------|-------|
| `id` | `ExternalId` (string) | Preserved for re-import matching |
| `company.name` | `CompanyName` | |
| `role.title` | `RoleTitle` | |
| `location.work_mode` | `WorkMode` (enum) | |
| `location.raw` | `Location` | |
| `compensation.salary_min` | `SalaryMin` (int) | Stored in thousands (120 = $120k) |
| `compensation.salary_max` | `SalaryMax` (int) | |
| `source.type` | `Source` (enum) | |
| `source.url` | `SourceUrl` | |
| `status.stage` | `Status` (enum) | |
| `created_at` | `CreatedDate` (DateTime) | |
| `updated_at` | `UpdatedDate` (DateTime) | |
| `skills.required` | `ApplicationSkill` (IsRequired=true) | Many-to-many join |
| `skills.nice_to_have` | `ApplicationSkill` (IsRequired=false) | Many-to-many join |
| `interview_questions_asked_inferred` | `InterviewQuestion` | One-to-many relationship |
| `timeline_events` | JSON column or `TimelineEvent` table | Phase 2+ |
| `contacts` | JSON column or `Contact` table | Phase 2+ |
| `interviewPrep` | JSON column on Application | Phase 3+ |

**JSON Column Storage (MVP Approach):**
- Store complex nested objects (`timeline_events`, `contacts`, `interviewPrep`, `requirements`, `benefits`) in single `ExtendedMetadata` JSON column
- Query support via EF Core JSON column features (SQL Server 2016+)
- Avoid premature normalization for Phase 2+ features

**Future Normalization (Phase 2+):**
- Separate `Contact`, `TimelineEvent`, `Requirement`, `Benefit` tables
- Maintain JSON export format for backwards compatibility

---

### Tools & Technologies Schema

**Optional Nested Structure (Extended Import Format):**
```json
{
  "tools_and_tech": {
    "languages_and_platforms": [".NET 8", "C#", "ASP.NET"],
    "databases": ["SQL Server", "MongoDB"],
    "cloud_and_infrastructure": ["Azure", "AWS", "Docker", "Kubernetes"],
    "testing": ["xUnit", "Playwright"],
    "devops": ["GitHub Actions", "Jenkins", "Terraform"],
    "other": ["PowerShell", "Python scripting"]
  },
  "responsibilities": {
    "architecture_and_design": [
      "Design microservices architecture for cloud migration",
      "Implement strangler pattern for legacy system modernization"
    ],
    "development": [
      "Build ASP.NET Core API controllers",
      "Develop queue processing services"
    ],
    "devops_and_deployment": [
      "Configure CI/CD pipelines",
      "Containerize applications for cloud deployment"
    ],
    "collaboration": [
      "Coordinate releases with team",
      "Document deployment procedures"
    ]
  }
}
```

**Import Behavior:**
- `tools_and_tech` flattened into `skills.required` or `skills.nice_to_have` based on context
- `responsibilities` stored in JSON column for future display (Phase 3+)

---

### Legacy Schema Migration

**Deprecated Field Names (Auto-Converted on Import):**

| Legacy Field (Import) | Schema v1 Field (Database) |
|----------------------|---------------------------|
| `company_name` | `company.name` |
| `company_location` | `location.raw` |
| `company_industry` | `company.industry` |
| `role_title` | `role.title` |
| `role_level` | `role.level` |
| `employment_type` | `role.employment_type` |
| `work_mode` (top-level) | `location.work_mode` |
| `source_channel` | `source.type` |
| `source_platform` | `source.name` |
| `job_post_url` | `source.url` |
| `salary_currency` | `compensation.currency` |
| `salary_min` | `compensation.salary_min` |
| `salary_max` | `compensation.salary_max` |
| `salary_type` | `compensation.salary_unit` |
| `comp_notes` | `compensation.notes` |
| `required_skills` | `skills.required` |
| `nice_to_have_skills` | `skills.nice_to_have` |
| `current_stage` | `status.stage` |
| `current_status` | Derived from context |
| `years_experience_required` | `requirements.experience.years_min` |
| `education_required` | `requirements.education.level` |

**Normalization Logic:**
- Import parser detects legacy field names and maps to schema v1 structure
- Both formats accepted during import (Phase 1-3)
- Export always uses schema v1 format
- Internal API exclusively uses schema v1 DTOs

---

### JSON Schema Documentation Files

**Location:** `Meta/JSON-application-persona.md`
**Purpose:** GPT prompt for converting messy job search notes into schema v1 JSON

**Sample File:** `Meta/2025-11-23-american-auto-auction-group-junior-devops-engineer.json`
**Purpose:** Reference implementation showing all fields populated with real-world data

**Validation Schema (Future):** `Meta/application-schema-v1.json`
**Purpose:** JSON Schema definition for automated validation (JSON Schema Draft 7)

---

### Performance Considerations

**Import Performance:**
- Batch insert applications (100 records per transaction)
- Use `AddRange()` for skills, interview questions, timeline events
- Index on `ExternalId` for duplicate detection
- Background job for large imports (>500 applications) with progress notifications

**Query Performance:**
- JSON columns indexed via computed columns for frequently queried paths:
  - `timeline_events[0].at AS FirstEventDate` (indexed)
  - `status.state AS CurrentState` (indexed)
- Avoid `SELECT *` from JSON columns in list views (project only needed fields)

**Export Performance:**
- Paginated export for users with >1000 applications
- Lazy-load `timeline_events` and `contacts` (not included in default export, opt-in via query param)

---

## PART V: PROJECT STYLE GUIDE

### Personality & Tone

**Brand Personality:**
- **Competitive:** Leaderboards, rivalry panel, "Close the gap" nudges
- **Motivational:** Rejections = +5 points, confetti celebrations, achievement unlocks
- **Collaborative:** Hunting parties, activity feed, "hunt together" messaging
- **Gamified:** Points, streaks, badges, levels (future)
- **Data-Driven:** Sankey diagrams, analytics, conversion rates

**Tone of Voice:**
- **Active & Direct:** "Log your first application" (not "Applications can be logged")
- **Supportive but Not Cheesy:** "Keep hunting" (not "You got this, superstar!")
- **Competitive but Friendly:** "Jordan is 12 points ahead—close the gap" (not "You're losing badly")
- **Celebratory:** "Interview unlocked! +5 points 🎯" (emoji acceptable in toasts)

**Writing Guidelines:**
- Use second person ("You have 3 applications") not third person
- Action verbs for CTAs: "Log", "Join", "Invite", "Compete"
- Avoid corporate jargon: "hunting party" not "collaboration workspace"
- Numbers are concrete: "15 seconds" not "quickly", "12 points ahead" not "slightly ahead"

---

### Color Palette

**Primary Colors:**
- **Primary (Hunter Green):** `#10B981` (Tailwind emerald-500)
  - Use: CTAs, Quick Capture button, success states, positive points (+1, +5)
  - Accessibility: Passes WCAG AA on white background (4.52:1)

- **Secondary (Rivalry Amber):** `#F59E0B` (Tailwind amber-500)
  - Use: Warnings, "close the gap" rivalry panel, streak warnings
  - Accessibility: Passes WCAG AA on white background (4.54:1)

- **Accent (Victory Blue):** `#3B82F6` (Tailwind blue-500)
  - Use: Links, info states, leaderboard highlights
  - Accessibility: Passes WCAG AA on white background (4.56:1)

**Neutral Colors:**
- **Background (Light Mode):** `#F9FAFB` (Tailwind gray-50)
- **Surface (Light Mode):** `#FFFFFF` (White)
- **Text Primary:** `#111827` (Tailwind gray-900)
- **Text Secondary:** `#6B7280` (Tailwind gray-500)
- **Border:** `#E5E7EB` (Tailwind gray-200)

**Dark Mode Palette (Phase 4):**
- **Background:** `#111827` (Tailwind gray-900)
- **Surface:** `#1F2937` (Tailwind gray-800)
- **Text Primary:** `#F9FAFB` (Tailwind gray-50)
- **Text Secondary:** `#9CA3AF` (Tailwind gray-400)

**Semantic Colors:**
- **Success:** `#10B981` (Emerald-500) - Application logged, offer received
- **Warning:** `#F59E0B` (Amber-500) - Streak expiring soon
- **Error:** `#EF4444` (Red-500) - Validation errors, destructive actions
- **Info:** `#3B82F6` (Blue-500) - Onboarding tips, info tooltips

**Gamification Colors:**
- **Points Gained:** `#10B981` (Green) - "+1", "+5" animations
- **Leaderboard Rank 1:** `#F59E0B` (Gold)
- **Leaderboard Rank 2:** `#9CA3AF` (Silver)
- **Leaderboard Rank 3:** `#CD7F32` (Bronze)
- **Current User Highlight:** `#DBEAFE` (Blue-100) background

---

### Typography

**Font Family:**
- **Primary:** Inter (Google Fonts) - Clean, readable, modern sans-serif
  - Fallback: system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif

**Font Weights:**
- **Regular (400):** Body text, labels
- **Medium (500):** Buttons, navigation items
- **Bold (700):** Headings, emphasis

**Type Scale (Tailwind Classes):**
- **Heading 1:** `text-4xl font-bold` (36px) - Page titles
- **Heading 2:** `text-3xl font-bold` (30px) - Section headers
- **Heading 3:** `text-2xl font-bold` (24px) - Card headers
- **Heading 4:** `text-xl font-bold` (20px) - Widget titles
- **Body Large:** `text-lg font-normal` (18px) - Introductory paragraphs
- **Body:** `text-base font-normal` (16px) - Default text
- **Body Small:** `text-sm font-normal` (14px) - Secondary text, captions
- **Caption:** `text-xs font-normal` (12px) - Timestamps, helper text

**Special Typography:**
- **Points Display:** `text-3xl font-bold text-emerald-600` - Prominent score
- **Leaderboard Rank:** `text-5xl font-bold` - Large rank numbers (#1, #2)
- **Quick Capture Button:** `text-lg font-medium` - Readable CTA

**Line Height:**
- Headings: `leading-tight` (1.25)
- Body text: `leading-relaxed` (1.625)
- Captions: `leading-normal` (1.5)

**Text Alignment:**
- Left-aligned by default (LTR languages)
- Center-aligned for empty states, modals
- Right-aligned for numeric values (leaderboard points, dates in tables)

---

### Layout & Spacing

**Grid System:**
- **Desktop (≥1024px):** 12-column grid, 24px gutters
- **Tablet (768-1023px):** 8-column grid, 16px gutters
- **Mobile (<768px):** 4-column grid, 16px gutters

**Container Widths:**
- **Desktop:** max-width: 1280px (Tailwind `max-w-7xl`)
- **Tablet:** max-width: 768px (Tailwind `max-w-3xl`)
- **Mobile:** max-width: 100% with 16px padding

**Spacing Scale (Tailwind):**
- **xs:** `4px` (p-1, m-1) - Icon gaps, tight spacing
- **sm:** `8px` (p-2, m-2) - Button padding, form field gaps
- **md:** `16px` (p-4, m-4) - Card padding, section spacing
- **lg:** `24px` (p-6, m-6) - Page padding, widget spacing
- **xl:** `32px` (p-8, m-8) - Section headers, large gaps
- **2xl:** `48px` (p-12, m-12) - Page top/bottom margins

**Common Patterns:**
- **Card Padding:** `p-6` (24px all sides)
- **Page Padding:** `px-4 py-8` (16px horizontal, 32px vertical on mobile)
- **Button Padding:** `px-4 py-2` (16px horizontal, 8px vertical)
- **Input Padding:** `px-3 py-2` (12px horizontal, 8px vertical)

---

### UI Components

#### **Buttons**

**Primary Button (Quick Capture, Login):**
```css
bg-emerald-600 hover:bg-emerald-700 active:bg-emerald-800
text-white font-medium
px-4 py-2 rounded-lg
transition-colors duration-150
focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:ring-offset-2
```

**Secondary Button (Cancel, Back):**
```css
bg-white hover:bg-gray-50 active:bg-gray-100
text-gray-700 font-medium border border-gray-300
px-4 py-2 rounded-lg
transition-colors duration-150
```

**Danger Button (Delete, Leave Party):**
```css
bg-red-600 hover:bg-red-700 active:bg-red-800
text-white font-medium
px-4 py-2 rounded-lg
```

**Icon Button (Hamburger, Close):**
```css
p-2 rounded-full hover:bg-gray-100 active:bg-gray-200
text-gray-600
```

#### **Forms**

**Text Input:**
```css
border border-gray-300 rounded-lg
px-3 py-2
focus:border-emerald-500 focus:ring-2 focus:ring-emerald-500
placeholder:text-gray-400
```

**Select Dropdown:**
```css
border border-gray-300 rounded-lg
px-3 py-2
bg-white
appearance-none (custom arrow icon)
```

**Checkbox/Radio:**
```css
accent-color: emerald-600 (modern browsers)
w-4 h-4 text-emerald-600 border-gray-300 rounded
```

#### **Cards**

**Application Card (List View):**
```css
bg-white border border-gray-200 rounded-lg
p-4 hover:shadow-md transition-shadow
cursor-pointer
```

**Leaderboard Card:**
```css
bg-white border border-gray-200 rounded-lg p-6
Current user: bg-blue-50 border-blue-300
Rank 1: border-amber-400 border-2
```

**Activity Event Card:**
```css
flex items-center gap-3 p-3 hover:bg-gray-50 rounded-lg
Avatar (left) + Text (center) + Timestamp (right, text-gray-500)
```

#### **Modals**

**Quick Capture Modal:**
```css
max-w-md w-full bg-white rounded-lg shadow-xl
p-6
Fixed center: top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2
Backdrop: bg-black/50 backdrop-blur-sm
```

**Confirmation Dialog:**
```css
max-w-sm w-full bg-white rounded-lg shadow-xl p-6
Heading: text-xl font-bold text-gray-900
Body: text-gray-600
Actions: flex gap-3 justify-end
```

#### **Toasts**

**Success Toast:**
```css
bg-emerald-50 border-l-4 border-emerald-500
text-emerald-900 p-4 rounded-lg shadow-lg
Icon: Check circle (emerald-500)
Position: top-right, fixed
```

**Error Toast:**
```css
bg-red-50 border-l-4 border-red-500
text-red-900 p-4 rounded-lg shadow-lg
Icon: X circle (red-500)
```

---

### Animations & Timing

**Transition Durations:**
- **Instant:** `duration-75` (75ms) - Hover states
- **Fast:** `duration-150` (150ms) - Button clicks, color changes
- **Normal:** `duration-300` (300ms) - Modals, dropdowns
- **Slow:** `duration-500` (500ms) - Page transitions

**Easing Functions:**
- **Default:** `ease-in-out` - Most transitions
- **Enter:** `ease-out` - Modals appearing, toasts sliding in
- **Exit:** `ease-in` - Modals disappearing, toasts sliding out

**Animations:**

**Confetti (Milestones):**
- Library: canvas-confetti (lightweight, customizable)
- Trigger: 10, 25, 50, 100 applications
- Duration: 3 seconds
- Colors: Emerald, Amber, Blue

**Points Increment (+1, +5):**
- CSS: `animate-bounce` on point change
- Text color: Green (`text-emerald-600`)
- Duration: 300ms

**Achievement Unlock:**
- Modal: Fade in + scale (0.95 → 1.0)
- Badge icon: Rotate + bounce
- Duration: 500ms

**Streak Fire Icon:**
- Flame flicker: CSS keyframe animation (subtle)
- Hover: Scale 1.1
- Warning state: Pulse animation (amber color)

**Loading States:**
- Spinner: Tailwind `animate-spin` (360° rotation)
- Skeleton screens: `animate-pulse` (gray-200 shimmer)
- Progress bars: Smooth width transition (duration-500)

---

### Icons

**Icon Library:** Heroicons (MIT license, designed by Tailwind team)
- **Style:** Outline for most UI (24x24px)
- **Style:** Solid for filled states (badges, active nav, 20x20px)

**Common Icons:**
- Quick Capture: `PlusCircleIcon`
- Applications: `BriefcaseIcon`
- Hunting Party: `UserGroupIcon`
- Analytics: `ChartBarIcon`
- Achievements: `TrophyIcon`
- Profile: `UserCircleIcon`
- Search: `MagnifyingGlassIcon`
- Filter: `FunnelIcon`
- Settings: `Cog6ToothIcon`
- Logout: `ArrowRightOnRectangleIcon`
- Success: `CheckCircleIcon`
- Error: `XCircleIcon`
- Info: `InformationCircleIcon`
- Warning: `ExclamationTriangleIcon`

**Icon Colors:**
- Default: `text-gray-600`
- Active/Hover: `text-gray-900`
- Primary: `text-emerald-600`
- Success: `text-emerald-500`
- Error: `text-red-500`

---

### Responsive Breakpoints

**Tailwind Breakpoints:**
- **sm:** 640px (Large phones, portrait tablets)
- **md:** 768px (Tablets, small laptops)
- **lg:** 1024px (Laptops, desktops)
- **xl:** 1280px (Large desktops)
- **2xl:** 1536px (Ultra-wide monitors)

**Design Approach:**
- Mobile-first (default styles for <640px)
- Add complexity at larger breakpoints (progressive enhancement)

**Component Adaptations:**

**Sidebar Navigation:**
- Mobile (<768px): Hidden, accessible via hamburger menu
- Tablet (768-1023px): Icon-only sidebar, collapsible
- Desktop (≥1024px): Full sidebar with labels

**Application List:**
- Mobile: Stacked cards, 1 column
- Tablet: 2 columns
- Desktop: Table view with sortable columns

**Leaderboard:**
- Mobile: Compact cards (rank + name + points)
- Desktop: Full table (rank, avatar, name, apps, interviews, points)

**Quick Capture Modal:**
- Mobile: Full-screen overlay
- Desktop: Centered modal (max-w-md)

---

### Code Standards

**Frontend (React + TypeScript):**

**File Structure:**
```
src/
├── features/
│   ├── applications/
│   │   ├── components/
│   │   │   ├── ApplicationCard.tsx
│   │   │   ├── ApplicationDetail.tsx
│   │   │   └── QuickCaptureModal.tsx
│   │   ├── hooks/
│   │   │   └── useApplications.ts
│   │   ├── services/
│   │   │   └── applicationService.ts
│   │   └── types/
│   │       └── Application.ts
│   ├── leaderboard/
│   ├── auth/
│   └── analytics/
├── shared/
│   ├── components/
│   │   ├── Button.tsx
│   │   ├── Modal.tsx
│   │   └── Toast.tsx
│   ├── hooks/
│   └── utils/
├── App.tsx
└── main.tsx
```

**Component Template:**
```typescript
// ApplicationCard.tsx
import React from 'react';
import { Application } from '../types/Application';

interface ApplicationCardProps {
  application: Application;
  onClick: (id: string) => void;
}

export const ApplicationCard: React.FC<ApplicationCardProps> = ({
  application,
  onClick
}) => {
  return (
    <div
      onClick={() => onClick(application.id)}
      className="bg-white border border-gray-200 rounded-lg p-4 hover:shadow-md transition-shadow cursor-pointer"
    >
      <h3 className="text-xl font-bold text-gray-900">{application.roleTitle}</h3>
      <p className="text-gray-600">{application.companyName}</p>
      <span className="text-sm text-gray-500">{application.createdDate}</span>
    </div>
  );
};
```

**Backend (C# ASP.NET Core):**

**Project Structure:**
```
BigJobHunterPro.Api/
├── Controllers/
│   ├── ApplicationsController.cs
│   ├── LeaderboardController.cs
│   └── AuthController.cs
├── Application/
│   ├── Services/
│   │   ├── ApplicationService.cs
│   │   └── LeaderboardService.cs
│   └── DTOs/
│       ├── ApplicationDto.cs
│       └── LeaderboardEntryDto.cs
├── Domain/
│   ├── Entities/
│   │   ├── User.cs
│   │   ├── Application.cs
│   │   └── HuntingParty.cs
│   └── Enums/
│       ├── ApplicationStatus.cs
│       └── WorkMode.cs
├── Infrastructure/
│   ├── Data/
│   │   ├── ApplicationDbContext.cs
│   │   └── Migrations/
│   └── Repositories/
│       └── ApplicationRepository.cs
└── Program.cs
```

**Controller Template:**
```csharp
// ApplicationsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BigJobHunterPro.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ApplicationsController : ControllerBase
{
    private readonly IApplicationService _applicationService;

    public ApplicationsController(IApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ApplicationDto>>> GetApplications()
    {
        var userId = User.GetUserId(); // Extension method
        var applications = await _applicationService.GetUserApplicationsAsync(userId);
        return Ok(applications);
    }

    [HttpPost]
    public async Task<ActionResult<ApplicationDto>> CreateApplication(
        [FromBody] CreateApplicationRequest request)
    {
        var userId = User.GetUserId();
        var application = await _applicationService.CreateApplicationAsync(userId, request);
        return CreatedAtAction(nameof(GetApplication), new { id = application.Id }, application);
    }
}
```

---

## PART V: TECHNICAL ARCHITECTURE

### Frontend Stack

**Core:**
- **Framework:** React 18 (functional components, hooks)
- **Language:** TypeScript 5.x (strict mode enabled)
- **Build Tool:** Vite (fast HMR, optimized production builds)
- **Routing:** React Router v6 (declarative routing, nested routes)

**Styling:**
- **CSS Framework:** Tailwind CSS 3.x (utility-first)
- **Component Library:** Headless UI (accessible, unstyled primitives)
- **Icons:** Heroicons (React components)

**State Management:**
- **Server State:** TanStack Query (React Query 5.x) - API caching, optimistic updates
- **Client State:** Zustand or Jotai (lightweight, minimal boilerplate)
- **Forms:** React Hook Form (performant, validation via Zod)

**Real-Time:**
- **SignalR Client:** @microsoft/signalr (leaderboard updates, activity feed)

**Utilities:**
- **Date Formatting:** date-fns (lightweight alternative to Moment.js)
- **Animations:** Framer Motion (confetti, achievement unlocks)
- **Charts:** Recharts (Sankey diagram, line charts) or D3.js (custom visualizations)

**Development:**
- **Linting:** ESLint (Airbnb config + TypeScript rules)
- **Formatting:** Prettier (consistent code style)
- **Testing:** Vitest (unit tests) + Playwright (E2E tests)

---

### Backend Stack

**Core:**
- **Framework:** ASP.NET Core 8 Web API
- **Language:** C# 12
- **ORM:** Entity Framework Core 8 (Code-First migrations)
- **Database:** Azure SQL Database (S1 tier: 20 DTUs, 250GB storage)

**Authentication:**
- **JWT:** System.IdentityModel.Tokens.Jwt (bearer tokens)
- **OAuth:** Microsoft.AspNetCore.Authentication.Google, GitHub
- **Identity:** ASP.NET Core Identity (user management, password hashing)

**Real-Time:**
- **SignalR:** Microsoft.AspNetCore.SignalR (leaderboard hub, activity hub)

**Utilities:**
- **Validation:** FluentValidation (declarative validation rules)
- **Mapping:** Mapster or AutoMapper (DTO ↔ Entity mapping)
- **Logging:** Serilog (structured logging to Azure App Insights)
- **Caching:** Microsoft.Extensions.Caching.Memory (in-memory) or Azure Redis (distributed)

**AI Integration:**
- **AI Service:** Custom service wrapping cheap AI API (Claude Haiku, GPT-3.5-turbo, or Gemini Flash)
- **HTTP Client:** IHttpClientFactory for AI API requests
- **Prompt Engineering:** Structured prompts for job listing parsing
- **Response Parsing:** JSON deserialization from AI responses
- **Error Handling:** Retry logic with exponential backoff, fallback to manual entry
- **Background Processing:** Background queue for async AI parsing (IHostedService or Hangfire)
- **Cost Optimization:** Token counting, response caching for duplicate listings

**Development:**
- **API Documentation:** Swagger/OpenAPI (Swashbuckle)
- **Testing:** xUnit (unit tests) + Testcontainers (integration tests with SQL Server container)

---

### Infrastructure & Deployment

**Hosting:**
- **Frontend:** Azure Static Web Apps (free tier, global CDN, auto HTTPS)
- **Backend API:** Azure App Service (B1 tier: 1 vCPU, 1.75GB RAM)
- **Database:** Azure SQL Database (S1 tier)
- **Caching (Phase 3+):** Azure Redis Cache (C0 Basic: 250MB)

**CI/CD:**
- **Version Control:** GitHub (private repo during development, public Phase 4+)
- **CI Pipeline:** GitHub Actions
  - Frontend: Build → Test → Deploy to Azure Static Web Apps
  - Backend: Build → Test → Publish to Azure App Service
- **Environments:** Development (local), Staging (Azure dev slot), Production

**Monitoring:**
- **Application Insights:** Azure Monitor (API performance, exceptions, custom metrics)
- **Logging:** Serilog → Application Insights (structured logs)
- **Uptime Monitoring:** Azure Monitor alerts (API health check endpoint)

**Security:**
- **HTTPS:** Enforced (Azure App Service auto-certificate)
- **CORS:** Restricted to frontend domain (bigjobhunter.pro)
- **Rate Limiting:** AspNetCoreRateLimit middleware (100 requests/minute per user)
- **Secrets:** Azure Key Vault (connection strings, OAuth secrets)

---

### API Design Patterns

**RESTful Conventions:**
- **GET /api/applications** - List applications (paginated, filterable)
- **GET /api/applications/{id}** - Get single application with AI-parsed data
- **POST /api/applications** - Create application (AI-Powered Quick Capture)
- **POST /api/applications/manual** - Create application with manual entry (fallback)
- **POST /api/applications/{id}/reparse** - Re-trigger AI parsing for existing application
- **PUT /api/applications/{id}** - Update application (manual edits override AI data)
- **DELETE /api/applications/{id}** - Delete application
- **PATCH /api/applications/{id}/status** - Update status only
- **GET /api/applications/{id}/interview-questions** - Get AI-generated interview questions
- **GET /api/applications/{id}/parsing-status** - Get current AI parsing status

**Request/Response Format (AI-Powered):**
```json
// POST /api/applications (Request - AI-Powered Quick Capture)
{
  "sourceUrl": "https://indeed.com/job/12345",
  "rawPageContent": "[Full copied page content from Ctrl+A, Ctrl+C...]"
}

// Response (201 Created - Instant, AI parsing queued)
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "sourceUrl": "https://indeed.com/job/12345",
  "status": "Applied",
  "points": 1,
  "aiParsingStatus": "Pending",
  "createdDate": "2026-01-04T10:30:00Z"
}

// GET /api/applications/{id} (After AI parsing completes - ~5 seconds)
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "companyName": "Infosys",
  "roleTitle": "Senior Software Engineer",
  "sourceUrl": "https://indeed.com/job/12345",
  "source": "Indeed",
  "status": "Applied",
  "workMode": "Hybrid",
  "location": "Dallas, TX, USA",
  "salaryMin": 120,
  "salaryMax": 150,
  "jobDescription": "Looking for an experienced full-stack engineer to build cloud solutions...",
  "requiredSkills": ["C#", "Azure", "React", "SQL Server"],
  "niceToHaveSkills": ["TypeScript", "Docker", "Kubernetes"],
  "parsedByAI": true,
  "aiParsingStatus": "Success",
  "points": 1,
  "createdDate": "2026-01-04T10:30:00Z",
  "lastAIParsedDate": "2026-01-04T10:30:05Z"
}

// GET /api/applications/{id}/interview-questions
{
  "applicationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "questions": [
    {
      "id": "q1",
      "question": "Describe your experience with C# and .NET Core in cloud environments",
      "category": "Technical",
      "difficulty": "Medium"
    },
    {
      "id": "q2",
      "question": "How do you approach migrating a monolithic application to microservices on Azure?",
      "category": "ProjectExperience",
      "difficulty": "Hard"
    },
    {
      "id": "q3",
      "question": "Tell me about a time you had to optimize database performance",
      "category": "Behavioral",
      "difficulty": "Medium"
    }
  ]
}
```

**Error Format (RFC 7807 Problem Details):**
```json
{
  "type": "https://bigjobhunter.pro/errors/validation",
  "title": "Validation Failed",
  "status": 400,
  "errors": {
    "companyName": ["Company name is required"],
    "roleTitle": ["Role title is required"]
  }
}
```

**Pagination (Cursor-Based):**
```json
// GET /api/applications?cursor=xyz&limit=20
{
  "data": [ /* 20 applications */ ],
  "pagination": {
    "nextCursor": "abc123",
    "hasMore": true
  }
}
```

---

### SignalR Hubs

**LeaderboardHub:**
- **Event:** `LeaderboardUpdated` (broadcast when user earns points)
- **Payload:** `{ partyId, userId, newRank, totalPoints }`
- **Clients Subscribe:** Frontend connects on `/app/party` page mount

**ActivityHub:**
- **Event:** `ActivityEventCreated` (broadcast to hunting party)
- **Payload:** `{ eventType, userId, displayName, avatarUrl, pointsAwarded, timestamp }`
- **Clients Subscribe:** Frontend connects on `/app/party` page mount

---

## PART VI: IMPLEMENTATION PRIORITIES

### Phase 1 MVP (Weeks 1-4)

**Week 1: Foundation + AI Integration**
- [ ] Backend: Database schema with AI parsing fields, migrations, seeded data (achievements, skills)
- [ ] Backend: User authentication (email/password, JWT)
- [ ] Backend: Application CRUD endpoints with AI parsing fields
- [ ] Backend: AI parsing service setup (API client, prompt engineering)
- [ ] Backend: Background job queue for async AI parsing
- [ ] Frontend: Project scaffolding, routing, layout shell

**Week 2: AI-Powered Core Tracking**
- [ ] Backend: AI job listing parser implementation (company, role, skills, salary extraction)
- [ ] Backend: AI interview question generator
- [ ] Backend: Error handling and fallback logic for AI failures
- [ ] Frontend: AI-Powered Quick Capture modal (URL + content paste, 10-second promise)
- [ ] Frontend: Loading states for AI parsing (skeleton screens)
- [ ] Frontend: Application list view (sortable table) with AI parsing status indicators
- [ ] Frontend: Application detail view with AI-parsed data display
- [ ] Backend: Points calculation service

**Week 3: Social Features + AI Polish**
- [ ] Backend: Hunting party creation, invite codes
- [ ] Backend: Leaderboard aggregation (cached)
- [ ] Backend: Activity feed events
- [ ] Frontend: Leaderboard UI, rivalry panel
- [ ] Frontend: Review/edit AI-parsed data interface
- [ ] Frontend: Interview questions display in detail view
- [ ] SignalR: Real-time leaderboard updates
- [ ] Testing: AI parsing accuracy and error handling

**Week 4: Import & Polish**
- [ ] Backend: Import Job-Hunt-Context JSON endpoint
- [ ] Backend: Retroactive point calculation
- [ ] Frontend: Import UI (drag-and-drop, progress bar)
- [ ] AI: Optimize prompts and token usage for cost reduction
- [ ] Testing: E2E critical flows (AI-powered Quick Capture, manual fallback)
- [ ] Deployment: Azure staging environment with AI API configuration

---

## CONCLUSION

This Project Structure document establishes:

1. **Standards** - Quality benchmarks for functionality, usability, aesthetics, performance, content, and development
2. **Information Architecture** - Page hierarchy, navigation design, user flows
3. **Data Models** - 11 core entities (including InterviewQuestion) with relationships, cardinality, and indexes
4. **Style Guide** - Colors, typography, layout, components, animations aligned with gamified brand
5. **Technical Architecture** - React + TypeScript frontend, ASP.NET Core backend with AI integration, Azure hosting
6. **AI-Powered Features** - Job listing parser, skill extraction, salary detection, work mode detection, interview question generation
7. **Implementation Roadmap** - 4-week Phase 1 MVP with AI parsing capabilities broken into weekly milestones

**Key AI Integration Decisions:**
- **Cheap AI API:** Claude Haiku, GPT-3.5-turbo, or Gemini Flash for cost-effective parsing
- **Async Processing:** Background queue prevents UI blocking, instant +1 point feedback
- **Error Handling:** Automatic fallback to manual entry if AI parsing fails
- **Cost Optimization:** Token counting, response caching, prompt engineering for minimal token usage
- **User Control:** Review/edit interface allows users to correct AI parsing errors

**Next Steps:**
1. Initialize frontend React project (Vite + TypeScript + Tailwind)
2. Initialize backend ASP.NET Core API project (Web API template)
3. Set up AI API credentials (Claude/OpenAI/Google AI)
4. Create Azure SQL Database with AI parsing fields, apply EF Core migrations
5. Implement AI parsing service with prompt engineering
6. Begin Week 1 development (authentication + database foundation + AI integration)

**We're building an AI-powered solution to solve our own problem. Let's make it work. 🎯**
