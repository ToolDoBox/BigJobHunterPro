# Big Job Hunter Pro — Concept Document

Domain: **bigjobhunter.pro**
Project Name: **Big Job Hunter Pro**
Version: v0.1 (concept draft)
Last Updated: 2026-01-03

---

## 1) One-line concept

According to the **Big Job Hunter Pro concept brief (user)**: _“Gamify your search at BigJobHunter.Pro. A retro-arcade job tracker styled after 90s hunting classics. Log applications in 15 seconds, earn points for interviews, maintain daily streaks, and climb the social leaderboards. Turn the ‘Corporate Wilderness’ into your high-score hunting ground.”_

---

## 2) Product promise

Big Job Hunter Pro turns job hunting into an arcade-style progression loop that rewards effort and consistency, not just outcomes. Players “hunt” opportunities, log actions fast, rack up points for real milestones (applications, interviews, offers), keep streaks alive, and compete with friends on leaderboards—wrapped in a nostalgic 90s hunting-arcade aesthetic that feels like a playable experience rather than a spreadsheet.

Assumption: The primary emotional target is “momentum” (reduce dread and increase daily action) and the secondary target is “friendly competition” (make job searching social and trackable in a fun way).

---

## 3) Target users

- **Solo job seekers** who need structure and motivation to apply consistently.
- **Friends / cohorts** (e.g., “Q1 job hunt crew”) who want shared accountability and rivalry.
- **Career-switchers** who benefit from clear progress visibility across stages.

Assumption: Teams/cohorts will typically be small (2–20 players), which influences how “social leaderboard” should feel (more intimate and rivalry-driven than global).

---

## 4) The “Corporate Wilderness” fantasy

Big Job Hunter Pro frames the job search as a stylized hunt through a corporate jungle:

- Job applications become **“Targets”**.
- Your pipeline becomes a **mission map**.
- Each milestone is a **score event**.
- Leaderboards become the **arcade high-score wall**.
- Streaks become **“keeping the fire lit”**—a daily campfire you protect.

Assumption: The fantasy should be consistent in naming and UI metaphors, but the underlying workflows must remain clean and modern so the theme never blocks usability.

---

## 5) Core gameplay loop

### 5.1 The daily loop (what players do every day)

1. Log at least one meaningful job-hunt action in **~15 seconds**.
2. Earn points immediately (feedback + celebration).
3. Keep or extend a daily streak.
4. See position change on the leaderboard (or a “closing the gap” rivalry panel).
5. Decide the next “hunt action” (what to do next).

According to the **Big Job Hunter Pro concept brief (user)**: the loop explicitly includes _quick logging_, _points for interviews_, _daily streaks_, and _social leaderboards_.

### 5.2 The weekly loop (what keeps it from feeling repetitive)

Assumption: Add a “season” or weekly cadence:

- Weekly leaderboard snapshots
- Weekly goals (“missions”)
- New badges or challenges that push consistent behavior

### 5.3 The long-term loop (why it stays valuable)

- A job-hunt funnel that shows conversion and time-to-stage
- A history of outcomes across time
- A “trophy room” of achievements and milestones
- A personal performance trend line (pace, streak bests, consistency)

Assumption: Long-term value depends on insight, not only competition—players should still benefit even if they ignore the leaderboard.

---

## 6) Scoring philosophy and point economy

### 6.1 Scoring principles

- **Reward effort and resilience** (including rejection) to prevent demotivation.
- **Make progress legible**: every score change should be explainable.
- **Avoid perverse incentives**: points shouldn’t encourage spam or low-quality behavior.

### 6.2 Baseline scoring rules

According to the **Big Job Hunter Pro concept brief (user)** (from existing implementation notes):

- **+1** point for Application
- **+2** points for Screening call
- **+5** points for First interview completed
- **+10** points for Follow up interview
- **+5** points for Rejection
- **+50** points for Offer

Assumption: “Follow up interview” will be clarified into (a) a generic “Interview 2+” rule or (b) a specific stage mapping so players don’t get confused when they have Interview 3/4.

### 6.3 Why rejection earns points

Rejection points communicate: “You took a shot.” In the Corporate Wilderness, missed shots still count as reps. This reduces shame spirals and keeps streaks alive.

Assumption: The UI should explicitly teach this, because it’s counterintuitive in traditional trackers.

---

## 7) Application lifecycle and stages

According to the **Big Job Hunter Pro concept brief (user)** (existing stage list):
**Applied, Screening, Interview 1, Interview 2, Interview 3, Interview 4, Offer, Rejection, Withdraw, Acceptance**

### 7.1 Stage presentation

- Stages are shown as an arcade “level path” (progress map).
- Current stage is highlighted in **Blaze Orange**.
- Completed stages glow **green**.
- Future stages are dimmed.

Assumption: The stage path should be clickable, but guarded (e.g., confirmation, audit trail, optional “edit window”) to maintain trust in scoring.

---

## 8) Social competition

### 8.1 What the leaderboard represents

According to the **Big Job Hunter Pro concept brief (user)**: players climb **social leaderboards** and compete with one another.

Assumption: The primary leaderboard scope is “groups/cohorts” (friend parties), with optional global discovery later.

### 8.2 “Hunting Party” groups

A group is a shared space with:

- A name (e.g., “Q1 COHORT”)
- A roster
- A leaderboard
- Optional “season” context (weekly/monthly)
- Invite flow (“Invite friend to hunt”)

### 8.3 Rivalry and “Target Acquisition” panel

According to the **Big Job Hunter Pro concept brief (user)** (dashboard concept): show a rivalry comparison that tells you:

- Who is right above you
- The exact gap (points or key metric)
- A concrete nudge (“They have 3 more interviews this week than you”)

Assumption: Rivalry panels should focus on controllable behaviors (applications, screenings, interviews logged) more than outcomes.

---

## 9) Streaks and engagement mechanics

According to the **Big Job Hunter Pro concept brief (user)**: maintain **daily streaks** and display a **streak fire icon**.

### 9.1 Streak definition

Assumption: A streak is maintained if the player logs at least one “qualifying action” per day (configurable: any score event, or a subset).

### 9.2 Streak UX

- A prominent fire icon
- “Log today to keep the fire lit”
- Streak warnings (e.g., “You have 6 hours left today”)

Assumption: Streak reminders will exist, but must be opt-in to respect user preferences and reduce stress.

### 9.3 Achievements and badges

Assumption: Add achievements that celebrate:

- Milestones (10 applications, first interview, first offer)
- Consistency (7-day streak, 30-day streak)
- Resilience (10 rejections logged)
- Momentum (3 screenings in 7 days)

---

## 10) Product surfaces (what players see)

### 10.1 Landing page (bigjobhunter.pro)

- Immediate concept pitch: “Turn job hunting into a high-score run.”
- Screenshots / concept art
- Call-to-action to sign in / start
- Social proof (later)

Assumption: Landing page copy should emphasize “fast logging” + “fun motivation” + “privacy-respecting competition.”

### 10.2 “The Lodge” (Dashboard + Leaderboard)

According to the **Big Job Hunter Pro concept brief (user)**: the main dashboard is “The Lodge,” with a personal stats area and a leaderboard sidebar.

Key elements:

- Total score (hero number)
- Streak (fire icon)
- Latest achievement
- Hunt funnel (pipeline map)
- Recent activity (arcade “feed”)
- Leaderboard + rivalry panel + invite button

### 10.3 “The Armory” (Application Tracker)

According to the **Big Job Hunter Pro concept brief (user)**: the workhorse screen is “The Armory,” a master-detail tracker:

- Left: Mission Log (application list)
- Right: Workbench (details, stage path, timeline, notes)
- Overlay: Quick Capture modal (“NEW TARGET ACQUISITION”)

### 10.4 Leaderboards (focused views)

- Top 10 list
- Focused stats
- Filters (time window, metric)

Assumption: Leaderboard filters should be simple and game-like: “All-Time,” “This Week,” “This Season.”

---

## 11) The 90s hunting-arcade aesthetic (visual system)

### 11.1 Overall vibe

According to the **Big Job Hunter Pro concept brief (user)**: “Modern Retro” that balances clean web usability with 90s arcade hunting nostalgia.

### 11.2 Palette (canonical)

According to the **Big Job Hunter Pro concept brief (user)**:

- Background: dark charcoal + deep “Forest Green” **#2E4600**
- Primary CTA / highlight: “Blaze Orange” **#FF6700**
- Score glow: “CRT Amber” **#FFB000**
- Terminal text: “Terminal Green” **#00FF00**

### 11.3 UI motifs

- Thick pixel/bolted borders
- Chunky “arcade button” CTAs
- Score counters like VCR displays
- Optional CRT scanline overlay
- Pixel/16-bit icons for stage states and events

Assumption: The theme should be optional in intensity (e.g., scanlines toggle) so long sessions stay comfortable.

### 11.4 Typography

According to the **Big Job Hunter Pro concept brief (user)**:

- Pixel font for headers/scores
- Legible modern sans-serif for body copy

Assumption: Pixel font usage is constrained to avoid accessibility issues (scores, headers, labels), while form fields and paragraphs remain modern and readable.

---

## 12) Copy voice and naming conventions

The copy should read like an arcade cabinet in an office jungle:

- “Quick Capture”
- “New Target Acquisition”
- “Tracked Targets”
- “Mission Log”
- “Workbench”
- “Hunt Path”
- “Hunting Party”
- “Trophy Room”
- “Target Acquisition” (rivalry)

Assumption: Microcopy should be motivating but not corny; it should feel like a confident arcade narrator, not a meme generator.

---

## 13) Trust, privacy, and fairness

Assumption: Because job hunting is sensitive, the product must make it easy to compete without oversharing:

- Hide company/role names in leaderboards if desired
- Share only aggregate counts (applications, interviews) to the group
- Keep private notes always private by default

Assumption: To keep competition credible:

- Scoring changes are traceable (score breakdown)
- Edits create history entries (“why score changed”)
- Rate limits or anomaly detection deter spam

---

## 14) The three key image concepts (art direction)

You currently have three visual concept briefs intended for UI/UX designers or concept artists.

### 14.1 Title screen concept — `Title.png`

According to the **Big Job Hunter Pro concept brief (user)**:

- A retro-arcade “screen capture” of **Job Hunter Pro**
- “Big Buck Hunter meets The Office”
- Player holds a **bright orange peripheral** to “shoot” resumes
- Target is a **fleeing briefcase** in an overgrown office jungle
- UI highlights: **high score**, **leaderboard on the side**, **streak fire icon**
- Vibe: frantic, nostalgic, gamified

**Usage:** Marketing hero image (landing page), share cards, splash screen.

Assumption: This image should prioritize instant comprehension of the gimmick (job hunting as arcade hunt) over UI accuracy.

---

### 14.2 Main dashboard concept — `dashboard.png`

According to the **Big Job Hunter Pro concept brief (user)**:
**Concept Title:** “The Lodge” Dashboard & Leaderboard

**Composition:**

- Top HUD: logo + global counters (Total Apps, Active Hunts, Season Ends) + “+ QUICK CAPTURE” CTA
- Main left: Trophy Room banner (Total Score, Streak, Latest Achievement)
- Funnel map: stages as a side-scrolling level
- Activity feed: arcade-style event feed (including rejections as positive)
- Right sidebar: “HUNTING PARTY: Q1 COHORT” leaderboard with Top 10 + rivalry panel + invite CTA

**Usage:** Primary logged-in home screen concept; the “fantasy” anchor of the app.

Assumption: The dashboard should be a “motivational control center” that makes the next action obvious within 5 seconds.

---

### 14.3 Application tracker concept — `Jts.png`

According to the **Big Job Hunter Pro concept brief (user)**:
**Concept Title:** “The Armory” (Application Tracker)

**Composition:**

- Left pane: “TRACKED TARGETS” list as “data cartridges” with status icons and timestamps
- Right pane: selected target details with “Hunt Path” stage progression bar
- Timeline + notes split view (mission history + private notes)
- Quick Capture modal overlay: “// NEW TARGET ACQUISITION //” with minimal fields and a huge “LOCK & LOAD (+1 PT)” CTA

**Usage:** The core work screen where players spend time managing pipeline details.

Assumption: The Armory must remain highly usable and scannable; the theme supports the workflow, never replaces it.

---

## 15) What makes Big Job Hunter Pro feel “complete”

A polished version of this concept feels like:

- Logging is genuinely fast and satisfying (15-second promise is real).
- The dashboard makes progress emotionally rewarding (score, streak, funnel, feed).
- Leaderboards create friendly rivalry without pressuring oversharing.
- The theme is consistent and delightful, but the UX is modern and clean.
- Players can always understand “what happened” and “what to do next.”

Assumption: The north-star experience is: “I open the app for 30 seconds, log one action, feel rewarded, and leave with a clear next step.”

---
