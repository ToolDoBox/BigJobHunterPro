# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Big Job Hunter Pro is a gamified job tracking application with a retro-arcade 90s hunting aesthetic. The core promise is "log applications in 15 seconds, earn points for milestones, maintain daily streaks, and compete on social leaderboards."

**Current State:** Phase 1 development (Sprint 1). The repo currently contains a static landing page (`index.html`). The full-stack application is planned but not yet scaffolded.

## Target Architecture

### Backend (Not Yet Implemented)
- **Framework:** ASP.NET Core 8 Web API
- **Architecture:** Clean Architecture (Domain → Application → Infrastructure → API)
- **ORM:** Entity Framework Core with MS SQL Server
- **Auth:** ASP.NET Core Identity + JWT tokens
- **Real-time:** SignalR for leaderboard/activity feed updates

### Frontend (Not Yet Implemented)
- **Framework:** React 18 + TypeScript + Vite
- **Styling:** Tailwind CSS with retro-arcade theme
- **State:** Redux Toolkit (or TanStack Query for server state)
- **Routing:** React Router v6

### Database Schema (Planned)
Key entities: `User`, `Application`, `HuntingParty`, `HuntingPartyMembership`, `ActivityEvent`, `LeaderboardEntry`, `Achievement`, `UserAchievement`, `Note`, `Skill`, `ApplicationSkill`

## Commands (Once Full Stack is Scaffolded)

### Backend (.NET)
```bash
dotnet build                          # Build solution
dotnet test                           # Run all tests
dotnet run --project src/WebAPI       # Run API locally

# EF Core migrations
dotnet ef migrations add <Name> -p src/Infrastructure -s src/WebAPI
dotnet ef database update -p src/Infrastructure -s src/WebAPI
```

### Frontend (React)
```bash
cd bigjobhunterpro-web
npm install                           # Install dependencies
npm run dev                           # Start dev server
npm test                              # Run tests (Vitest)
npm run lint                          # Run ESLint
npm run build                         # Production build
```

### Docker
```bash
docker-compose up -d db               # Start SQL Server only
docker-compose up -d                  # Start all services
docker-compose down -v                # Stop and remove volumes
```

## Key Concepts

### Scoring System
- **+1 point:** Application logged
- **+2 points:** Screening call
- **+5 points:** Interview completed (any round)
- **+5 points:** Rejection logged (rewards resilience)
- **+50 points:** Offer received

### Core Features (MVP Scope)
1. **Quick Capture:** Log applications in ≤15 seconds (company, role, source)
2. **Hunting Parties:** Private friend groups with leaderboards
3. **Rivalry Panel:** Shows who's directly ahead and the exact gap
4. **Activity Feed:** Real-time updates of party members' actions
5. **Import Tool:** Bulk import from Job-Hunt-Context JSON files with retroactive points

### UI Theme (90s Arcade)
- **Colors:** Forest Green (#2E4600), Blaze Orange (#FF6700), CRT Amber (#FFB000), Terminal Green (#00FF00)
- **Typography:** "Press Start 2P" for headers/scores, Inter for body text
- **Terminology:** "The Lodge" (dashboard), "The Armory" (tracker), "Hunting Party" (groups), "Quick Capture" (fast logging)

## Development Standards

### Naming Conventions
- **C#:** PascalCase classes/methods, camelCase parameters, _camelCase private fields
- **TypeScript:** PascalCase components, camelCase functions/variables
- **Database:** PascalCase tables and columns

### Git Workflow
- Branch from `main` with `feature/*` prefixes
- Commit messages: Conventional Commits (`feat:`, `fix:`, `refactor:`, `docs:`)
- PRs require review before merging

### Testing Strategy
- Backend: xUnit for unit/integration tests
- Frontend: Vitest + React Testing Library
- E2E: Playwright (planned)

## Project Documentation

- `Docs/GETTING_STARTED.md` - Setup instructions and project scaffolding commands
- `Docs/Project-Structure.md` - Architecture, data models, style guide, technical decisions
- `Docs/Project-Scoping.md` - User stories, feature prioritization, roadmap
- `Docs/concept.md` - Product vision and UX concepts
- `Meta/Sprint 1.md` - Current sprint backlog and completion criteria
- If you need a new Markdown file that does not fit an existing category, place it in `Meta/` (misc bucket, but do not create junk files).
