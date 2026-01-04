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

**U1. Quick Capture Promise: 15 Seconds**
- From click to confirmation, logging an application must average ≤15 seconds
- Form auto-focuses on first field (company name)
- Enter key submits form (keyboard-only flow supported)
- Validation errors appear inline within 100ms

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
- **CompanyName (string, required):** Job company name
- **RoleTitle (string, required):** Job position title
- **SourceUrl (string, nullable):** Link to job posting
- **Source (enum, nullable):** Indeed, LinkedIn, Referral, CompanyWebsite, Other
- **Status (enum, required):** Prospecting, Applied, Screening, Interview, Offer, Rejected, Withdrawn
- **WorkMode (enum, nullable):** Remote, Hybrid, Onsite
- **SalaryMin (int, nullable):** Minimum salary (stored in thousands: 120 = $120k)
- **SalaryMax (int, nullable):** Maximum salary
- **Points (int, computed):** Points awarded for this application (Applied=1, Interview=5, Rejected=5, Offer=50)
- **CreatedDate (DateTime, indexed):** When logged
- **UpdatedDate (DateTime):** Last modification timestamp
- **Relationships:**
  - N:1 with User
  - 1:N with Notes
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

---

### Cardinality Summary

| Relationship | Cardinality | Notes |
|--------------|-------------|-------|
| User ↔ Application | 1:N | One user owns many applications |
| User ↔ HuntingParty | N:M | Users can join multiple parties (Phase 3+) |
| HuntingParty ↔ ActivityEvent | 1:N | Party has many activity events |
| Application ↔ Note | 1:N | Application can have multiple notes |
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

## PART IV: PROJECT STYLE GUIDE

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
- **GET /api/applications/{id}** - Get single application
- **POST /api/applications** - Create application (Quick Capture)
- **PUT /api/applications/{id}** - Update application
- **DELETE /api/applications/{id}** - Delete application
- **PATCH /api/applications/{id}/status** - Update status only

**Request/Response Format:**
```json
// POST /api/applications (Request)
{
  "companyName": "Infosys",
  "roleTitle": "Senior Software Engineer",
  "sourceUrl": "https://indeed.com/job/12345",
  "source": "Indeed"
}

// Response (201 Created)
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "companyName": "Infosys",
  "roleTitle": "Senior Software Engineer",
  "sourceUrl": "https://indeed.com/job/12345",
  "source": "Indeed",
  "status": "Applied",
  "points": 1,
  "createdDate": "2026-01-04T10:30:00Z"
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

**Week 1: Foundation**
- [ ] Backend: Database schema, migrations, seeded data (achievements)
- [ ] Backend: User authentication (email/password, JWT)
- [ ] Backend: Application CRUD endpoints
- [ ] Frontend: Project scaffolding, routing, layout shell

**Week 2: Core Tracking**
- [ ] Frontend: Quick Capture modal (15-second promise)
- [ ] Frontend: Application list view (sortable table)
- [ ] Frontend: Application detail view (master-detail)
- [ ] Backend: Points calculation service

**Week 3: Social Features**
- [ ] Backend: Hunting party creation, invite codes
- [ ] Backend: Leaderboard aggregation (cached)
- [ ] Backend: Activity feed events
- [ ] Frontend: Leaderboard UI, rivalry panel
- [ ] SignalR: Real-time leaderboard updates

**Week 4: Import & Polish**
- [ ] Backend: Import Job-Hunt-Context JSON endpoint
- [ ] Backend: Retroactive point calculation
- [ ] Frontend: Import UI (drag-and-drop, progress bar)
- [ ] Testing: E2E critical flows
- [ ] Deployment: Azure staging environment

---

## CONCLUSION

This Project Structure document establishes:

1. **Standards** - Quality benchmarks for functionality, usability, aesthetics, performance, content, and development
2. **Information Architecture** - Page hierarchy, navigation design, user flows
3. **Data Models** - 10 core entities with relationships, cardinality, and indexes
4. **Style Guide** - Colors, typography, layout, components, animations aligned with gamified brand
5. **Technical Architecture** - React + TypeScript frontend, ASP.NET Core backend, Azure hosting
6. **Implementation Roadmap** - 4-week Phase 1 MVP broken into weekly milestones

**Next Steps:**
1. Initialize frontend React project (Vite + TypeScript + Tailwind)
2. Initialize backend ASP.NET Core API project (Web API template)
3. Create Azure SQL Database, apply EF Core migrations
4. Begin Week 1 development (authentication + database foundation)

**We're building this to solve our own problem. Let's make it work. 🎯**
