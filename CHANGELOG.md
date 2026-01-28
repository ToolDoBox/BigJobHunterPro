# BigJobHunterPro Changelog

All notable changes and planned features for this project are documented here.

---

## Claude Session Workflow

This section documents how Claude Code sessions work with this project, enabling seamless handoffs between sessions.

### How to Pick Up Where We Left Off

1. **Read this CHANGELOG first** - Check the [Unreleased] section for planned features and their status
2. **Check CLAUDE.md** - Contains project structure, commands, and conventions
3. **Review Meta/Sprints/** - For sprint-specific context if working on a sprint task
4. **Check git status** - See what branch you're on and any uncommitted changes

### How to Update This Changelog

When starting a new feature:
1. Find the feature in [Unreleased] section
2. Update its status from `[ ] Not Started` to `[~] In Progress`
3. Add implementation notes as you work

When completing a feature:
1. Update status to `[x] Completed`
2. Add completion date
3. Move to a versioned release section when deploying

When adding a new planned feature:
1. Add to [Unreleased] section with full specification
2. Follow the existing format (Purpose, Scope, Technical Details, Files)
3. Assign a priority number

### Implementation Workflow

1. **Create feature branch:** `git checkout -b feature/<feature-name>`
2. **Backend first:** Entity → DbContext → Migration → Repository → DTOs → Controller
3. **Frontend second:** Types → Service → Components → Page → Router → Navigation
4. **Test thoroughly:** Backend API tests, then frontend integration
5. **Update changelog:** Mark feature complete, add notes
6. **Commit with conventional commits:** `feat:`, `fix:`, `docs:`, `refactor:`, etc.

---

## [Unreleased] - Planned Features

### Feature 1: Question Range
**Status:** [x] Completed (2026-01-28)
**Priority:** 1 (Current Focus)
**Branch:** `feature/question-range`

#### Purpose
A new tab for collecting interview questions and practicing optimal answers. Helps identify frequently asked questions across interviews and build a personal Q&A knowledge base.

#### User Stories
- As a job seeker, I want to record questions I'm asked during interviews so I can prepare better answers
- As a job seeker, I want to track how often each question is asked so I can prioritize my practice
- As a job seeker, I want to categorize questions (behavioral, technical, etc.) so I can study by type
- As a job seeker, I want to write and refine my answers so I'm prepared for future interviews

#### Scope
- New navigation tab "Question Range" (hunting theme: like a shooting/practice range)
- Full CRUD for interview questions
- Question categorization (Behavioral, Technical, Situational, Company-Specific, General)
- Frequency tracking (how many times each question has been asked)
- Optional link to specific job application (to remember context)
- Tags for additional organization
- Search and filter capabilities
- (Future) Study/flashcard mode for practice

#### Technical Details

**Backend - New Files:**
- `src/Domain/Entities/InterviewQuestion.cs` - Question entity
- `src/Domain/Enums/QuestionCategory.cs` - Category enum (Behavioral=0, Technical=1, Situational=2, CompanySpecific=3, General=4, Other=5)
- `src/Infrastructure/Data/Repositories/InterviewQuestionRepository.cs` - Data access
- `src/Application/DTOs/Questions/InterviewQuestionDto.cs` - Response DTO
- `src/Application/DTOs/Questions/CreateInterviewQuestionDto.cs` - Create request
- `src/Application/DTOs/Questions/UpdateInterviewQuestionDto.cs` - Update request
- `src/WebAPI/Controllers/InterviewQuestionsController.cs` - API endpoints

**Backend - Modified Files:**
- `src/Infrastructure/Data/ApplicationDbContext.cs` - Add DbSet<InterviewQuestion> and configuration

**API Endpoints:**
- `GET /api/interview-questions` - List with optional filters (category, search, applicationId)
- `GET /api/interview-questions/{id}` - Get single question
- `POST /api/interview-questions` - Create new question
- `PUT /api/interview-questions/{id}` - Update question
- `DELETE /api/interview-questions/{id}` - Delete question
- `POST /api/interview-questions/{id}/increment` - Increment TimesAsked counter

**Frontend - New Files:**
- `bigjobhunterpro-web/src/pages/QuestionRange.tsx` - Main page
- `bigjobhunterpro-web/src/services/questions.ts` - API service
- `bigjobhunterpro-web/src/types/question.ts` - TypeScript types
- `bigjobhunterpro-web/src/components/questions/QuestionsList.tsx` - List container
- `bigjobhunterpro-web/src/components/questions/QuestionCard.tsx` - Individual question
- `bigjobhunterpro-web/src/components/questions/AddQuestionModal.tsx` - Create modal
- `bigjobhunterpro-web/src/components/questions/EditQuestionModal.tsx` - Edit modal
- `bigjobhunterpro-web/src/components/questions/CategoryBadge.tsx` - Category indicator

**Frontend - Modified Files:**
- `bigjobhunterpro-web/src/router/index.tsx` - Add route `{ path: 'question-range', element: <QuestionRange /> }`
- `bigjobhunterpro-web/src/components/layout/Header.tsx` - Add nav item

**Entity Schema:**
```csharp
public class InterviewQuestion
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public string QuestionText { get; set; } = string.Empty;
    public string? AnswerText { get; set; }
    public string? Notes { get; set; }

    public QuestionCategory Category { get; set; }
    public List<string> Tags { get; set; } = new();

    public int TimesAsked { get; set; } = 1;
    public DateTime? LastAskedDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }

    public Guid? ApplicationId { get; set; }
    public Application? Application { get; set; }
}
```

---

### Feature 2: Follow-up Email System Improvements
**Status:** [ ] Not Started
**Priority:** 2
**Branch:** `feature/followup-email-improvements`

#### Purpose
Improve the existing follow-up email generation to provide more useful, contextual emails and fix the broken mailto/export functionality.

#### Current Problems
1. Generated emails lack context - they're too generic
2. "Open in Gmail" button doesn't work properly
3. No way to copy email text to clipboard
4. User can't specify their intent/goal for the email

#### Scope
- Add a "What's your goal?" textarea before generating the email
- Fix mailto link to work with mail.google.com (Gmail web)
- Add "Copy to Clipboard" button
- Improve email templates with better context awareness

#### Technical Details

**Frontend - Modified Files:**
- `bigjobhunterpro-web/src/components/applications/FollowUpEmailModal.tsx`
  - Add intent/goal input field before "Generate" button
  - Pass intent to generation function
  - Fix Gmail link generation
  - Add copy to clipboard button

- `bigjobhunterpro-web/src/utils/followUpEmail.ts`
  - Update `generateFollowUpEmail()` to accept and use intent parameter
  - Fix `buildMailtoUrl()` for Gmail web compatibility
  - Add `copyToClipboard()` utility function

**Gmail Web URL Format:**
```
https://mail.google.com/mail/?view=cm&to={email}&su={subject}&body={body}
```
(All parameters must be URI encoded)

**New UI Flow:**
1. User clicks "Follow Up" on a timeline event
2. Modal shows intent field: "What do you want to achieve with this email?"
3. User enters: "Thank them for the interview and reiterate interest"
4. Click "Generate Email"
5. Email preview shown with: Edit, Copy to Clipboard, Open in Gmail, Open in Default Mail

---

### Feature 3: Expired Application Status
**Status:** [ ] Not Started
**Priority:** 3
**Branch:** `feature/expired-status`

#### Purpose
Add a new "Expired" status for applications that go cold - companies that never responded after initial application. This is distinct from "Rejected" (explicit no) and helps keep the active pipeline clean.

#### Rules
- Applications become "Expired" after 21 days of no activity since the last timeline event
- Expired applications award 0 points (retroactively adjusted)
- Expired applications do NOT appear in Hunting Party activity feed
- Users can manually un-expire an application if the company responds later
- Expired status is visually distinct (grayed out, different badge color)

#### Technical Details

**Backend Changes:**

1. Add to `src/Domain/Enums/ApplicationStatus.cs`:
   ```csharp
   Expired = 6
   ```

2. Add to `src/Domain/Enums/EventType.cs`:
   ```csharp
   Expired = 7
   ```

3. Modify `src/Domain/Entities/Application.cs`:
   - Update `ComputeCurrentStatus()` to check for expiration condition
   - Update `ComputeTotalPoints()` to return 0 for expired applications
   - Add helper: `IsExpired()` checks if last event > 21 days and status not terminal

4. Add background job or computed property:
   - Option A: Scheduled job that marks applications as expired daily
   - Option B: Computed on-read (check last event date when fetching)
   - Recommendation: Computed on-read for simplicity, no background jobs needed

5. Modify `src/WebAPI/Controllers/ApplicationsController.cs`:
   - Add `POST /api/applications/{id}/unexpire` endpoint to reactivate

6. Modify Activity Feed queries:
   - Exclude Expired applications from `GetActivityFeed()` results

**Frontend Changes:**

1. Update `bigjobhunterpro-web/src/components/applications/StatusBadge.tsx`:
   - Add "Expired" case with gray/muted styling

2. Update `bigjobhunterpro-web/src/pages/Applications.tsx`:
   - Add "Expired" to status filter dropdown
   - Visual distinction for expired cards (opacity, grayscale)

3. Update `bigjobhunterpro-web/src/pages/ApplicationDetail.tsx`:
   - Show "Reactivate" button if expired
   - Display warning banner for expired applications

4. Update types in `bigjobhunterpro-web/src/types/`:
   - Add 'Expired' to status union types

---

### Feature 4: Dedicated FavIcon
**Status:** [ ] Not Started
**Priority:** 4
**Branch:** `feature/favicon`

#### Purpose
Replace the default/placeholder favicon with a branded icon that fits the hunting/arcade theme.

#### Design Options
1. **Campfire Icon** - Plays on "The Lodge" hunting lodge theme, warmth, gathering place
2. **Crosshair on Envelope** - Combines job hunting (target/crosshair) with applications (envelope)

#### Technical Details

**Files to Create:**
- `bigjobhunterpro-web/public/favicon.ico` - 16x16, 32x32, 48x48 multi-size ICO
- `bigjobhunterpro-web/public/favicon-16x16.png`
- `bigjobhunterpro-web/public/favicon-32x32.png`
- `bigjobhunterpro-web/public/apple-touch-icon.png` - 180x180
- `bigjobhunterpro-web/public/android-chrome-192x192.png`
- `bigjobhunterpro-web/public/android-chrome-512x512.png`

**Files to Modify:**
- `bigjobhunterpro-web/index.html` - Update favicon links
- `bigjobhunterpro-web/public/manifest.json` - Update icons array (if exists)

**Color Palette (from theme):**
- Forest Green: #2E4600
- Blaze Orange: #FF6700
- CRT Amber: #FFB000
- Terminal Green: #00FF00

**Implementation Notes:**
- Use SVG source for scalability, then export to PNG/ICO
- Consider using a tool like RealFaviconGenerator for all sizes
- Test on multiple browsers and devices

---

### Feature 5: Better Filtering System for Job Listings
**Status:** [ ] Not Started (Research Phase)
**Priority:** 5
**Branch:** `feature/job-filtering-research`

#### Purpose
Help users quickly filter through daily job listings to find the best matches for their resume, experience level, location preferences, and salary requirements.

#### Problem Statement
Currently, filtering through a day's worth of new job listings takes too long. Users need a way to:
- Match listings against their resume/skills
- Filter by years of experience required
- Filter by location (remote, specific cities)
- Filter by salary/pay range
- Avoid listings that are clearly not a fit

#### Constraints
- Cannot scrape job board websites (ToS violations, blocks)
- Need local processing or privacy-respecting solution
- Should work with multiple job boards

#### Research Areas

1. **Browser Extension Approach**
   - Parse job listings as user browses
   - Highlight/filter in-page
   - Pros: Works with any site, real-time
   - Cons: Requires extension development, browser-specific

2. **Manual Import/Paste Approach**
   - User copies job listing text into app
   - AI analyzes fit against stored resume
   - Pros: Simple, no scraping needed
   - Cons: Manual effort per listing

3. **Job Board API Integrations**
   - LinkedIn API (limited for job searching)
   - Indeed API (publisher program)
   - Greenhouse, Lever (ATS APIs - usually employer-side)
   - Pros: Structured data, reliable
   - Cons: Limited availability, may require partnerships

4. **RSS/Email Digests**
   - Parse job alert emails or RSS feeds
   - Many job boards offer email alerts
   - Pros: User controls data, no scraping
   - Cons: Parsing email formats is fragile

5. **AI-Powered Analysis**
   - Store user's resume/preferences in app
   - When user pastes job description, AI scores fit
   - Shows: match %, missing skills, red flags
   - Pros: Intelligent, personalized
   - Cons: Requires AI API costs per analysis

#### Recommended Approach (Tentative)
Combination of Manual Import + AI Analysis:
1. User stores resume and preferences in profile
2. User pastes job listing text or URL
3. App extracts key requirements
4. AI compares against resume, gives fit score
5. Highlights matching skills, missing skills, concerns
6. User can save promising listings to apply

#### Next Steps
- [ ] Research job board ToS and API availability
- [ ] Prototype manual import + AI analysis flow
- [ ] Test with real job listings
- [ ] Evaluate accuracy and usefulness
- [ ] Consider browser extension if manual import too tedious

---

## Version History

### [0.1.0] - 2026-01-XX (Initial Release)
*First production-ready release*

#### Added
- Quick Capture for logging applications in <15 seconds
- Application tracking with timeline events
- Contact management per application
- Cover letter generation
- Hunting Party leaderboards
- Points and scoring system
- User authentication

---

## Notes for Future Sessions

### Common Commands
```bash
# Backend
cd src/WebAPI && dotnet run                    # Run API
dotnet build                                    # Build solution
dotnet test                                     # Run tests
dotnet ef migrations add <Name> -p ../Infrastructure -s .  # New migration

# Frontend
cd bigjobhunterpro-web && npm run dev          # Dev server
npm run build                                   # Production build
npm run lint                                    # Lint check

# Database (PostgreSQL via Docker)
docker-compose up -d postgres                   # Start DB
docker exec -it bigjobhunterpro-postgres psql -U postgres -d bigjobhunterpro_dev
```

### Key File Locations
- **Entities:** `src/Domain/Entities/`
- **Enums:** `src/Domain/Enums/`
- **DbContext:** `src/Infrastructure/Data/ApplicationDbContext.cs`
- **Controllers:** `src/WebAPI/Controllers/`
- **React Pages:** `bigjobhunterpro-web/src/pages/`
- **React Components:** `bigjobhunterpro-web/src/components/`
- **Router:** `bigjobhunterpro-web/src/router/index.tsx`
- **Navigation:** `bigjobhunterpro-web/src/components/layout/Header.tsx`
- **API Services:** `bigjobhunterpro-web/src/services/`
- **Types:** `bigjobhunterpro-web/src/types/`

### Conventions to Follow
- **Commits:** Conventional Commits (feat:, fix:, docs:, refactor:)
- **C# Naming:** PascalCase classes/methods, _camelCase private fields
- **TS Naming:** PascalCase components, camelCase functions/variables
- **Dates:** Use `CreatedDate`/`UpdatedDate` (not `CreatedAt`/`UpdatedAt`) to match existing entities
- **Theme:** 90s arcade/hunting aesthetic, specific color palette, hunting terminology
