# Sprint-10 Implementation Plan

## Overview

This sprint adds job description analysis features to the dashboard and reorganizes the layout for better UX.

## Requirements Summary

1. **Job Description Analysis Section** - Analyze ALL applied positions to show:
   - Keyword analysis of Roles
   - Most frequent skills from job descriptions
   - Placement: Under "What's Working", above "Application Breakdown"

2. **Quick Actions Relocation** - Move to top of dashboard, directly under "Welcome to the Lodge" banner

---

## Implementation Tasks

### Phase 1: Backend - New Analytics Endpoint

#### Task 1.1: Add New Analytics Service Method
**File:** `src/Infrastructure/Services/AnalyticsService.cs`

Create new method `GetAllApplicationsAnalysis()` that:
- Queries ALL user applications (not filtered by success status)
- Extracts keywords from:
  - `RoleTitle` field
  - `JobDescription` field (if present)
- Aggregates skills from:
  - `RequiredSkills` list
  - `NiceToHaveSkills` list
- Returns separate lists for role keywords and skill frequencies

**Data Structure:**
```csharp
public class ApplicationsAnalysisDto
{
    public List<KeywordFrequencyDto> RoleKeywords { get; set; }
    public List<SkillFrequencyDto> TopSkills { get; set; }
    public int TotalApplicationsAnalyzed { get; set; }
}

public class SkillFrequencyDto
{
    public string Skill { get; set; }
    public int Count { get; set; }
    public decimal Percentage { get; set; }
}
```

#### Task 1.2: Add Controller Endpoint
**File:** `src/WebAPI/Controllers/AnalyticsController.cs`

Add endpoint:
```
GET /api/analytics/applications-analysis?topRoleKeywords=10&topSkills=15
```

#### Task 1.3: Update Interface
**File:** `src/Application/Interfaces/IAnalyticsService.cs`

Add method signature for the new analysis method.

---

### Phase 2: Frontend - Analytics Service

#### Task 2.1: Add Type Definitions
**File:** `bigjobhunterpro-web/src/types/analytics.ts`

Add types:
```typescript
export interface ApplicationsAnalysis {
  roleKeywords: KeywordFrequency[];
  topSkills: SkillFrequency[];
  totalApplicationsAnalyzed: number;
}

export interface SkillFrequency {
  skill: string;
  count: number;
  percentage: number;
}
```

#### Task 2.2: Add Service Function
**File:** `bigjobhunterpro-web/src/services/analytics.ts`

Add function:
```typescript
export const getApplicationsAnalysis = async (
  topRoleKeywords: number = 10,
  topSkills: number = 15
): Promise<ApplicationsAnalysis>
```

---

### Phase 3: Frontend - Dashboard Modifications

#### Task 3.1: Create JobDescriptionAnalysis Component
**File:** `bigjobhunterpro-web/src/components/dashboard/JobDescriptionAnalysis.tsx`

New component displaying:
- Section header: "Job Description Insights" (or similar retro-themed name)
- Two columns:
  - Left: "Role Keywords" - list of top keywords from role titles
  - Right: "In-Demand Skills" - list of most frequent skills
- Progress bars or badges showing frequency/percentage
- Total applications analyzed counter
- Loading state
- Empty state (when no applications or no data to analyze)

#### Task 3.2: Modify Dashboard Layout
**File:** `bigjobhunterpro-web/src/pages/Dashboard.tsx`

Changes:
1. **Move Quick Actions** (currently lines 363-373) to after the welcome banner (after line 114)
2. **Add JobDescriptionAnalysis component** between "What's Working" section (ends ~line 293) and "Application Breakdown" section (starts ~line 295)
3. Add state and data fetching for the new analysis data

---

## File Change Summary

| File | Type | Changes |
|------|------|---------|
| `src/Application/Interfaces/IAnalyticsService.cs` | Modify | Add method signature |
| `src/Infrastructure/Services/AnalyticsService.cs` | Modify | Add `GetAllApplicationsAnalysis()` method |
| `src/WebAPI/Controllers/AnalyticsController.cs` | Modify | Add new endpoint |
| `bigjobhunterpro-web/src/types/analytics.ts` | Create | Add new types |
| `bigjobhunterpro-web/src/services/analytics.ts` | Modify | Add service function |
| `bigjobhunterpro-web/src/components/dashboard/JobDescriptionAnalysis.tsx` | Create | New component |
| `bigjobhunterpro-web/src/pages/Dashboard.tsx` | Modify | Reorder layout, add new section |

---

## UI Design Notes

### Job Description Insights Section

```
┌─────────────────────────────────────────────────────────────────┐
│  🎯 JOB DESCRIPTION INSIGHTS                                    │
│  Based on X applications analyzed                               │
├─────────────────────────────────┬───────────────────────────────┤
│  ROLE KEYWORDS                  │  IN-DEMAND SKILLS             │
│  ─────────────────────          │  ─────────────────            │
│  ▓▓▓▓▓▓▓▓░░ Engineer (45%)     │  ▓▓▓▓▓▓▓▓▓░ React (52%)      │
│  ▓▓▓▓▓▓░░░░ Senior (38%)       │  ▓▓▓▓▓▓▓▓░░ TypeScript (48%) │
│  ▓▓▓▓▓░░░░░ Software (32%)     │  ▓▓▓▓▓▓░░░░ Python (35%)     │
│  ▓▓▓▓░░░░░░ Full-Stack (28%)   │  ▓▓▓▓▓░░░░░ AWS (30%)        │
│  ▓▓▓░░░░░░░ Developer (22%)    │  ▓▓▓▓░░░░░░ Node.js (25%)    │
│  ...                            │  ...                          │
└─────────────────────────────────┴───────────────────────────────┘
```

### Color Palette (from CLAUDE.md)
- Forest Green: #2E4600
- Blaze Orange: #FF6700
- CRT Amber: #FFB000
- Terminal Green: #00FF00

### Dashboard Layout Order (After Changes)

1. Welcome Banner - "Welcome to the Lodge, {user}!"
2. **Quick Actions** (MOVED from bottom)
3. Stats Grid (Points, Applications, Streak)
4. Weekly Progress
5. What's Working (Top Keywords, Best Sources)
6. **Job Description Insights** (NEW)
7. Application Breakdown (Pie Charts)
8. Average Time to Milestone

---

## Testing Considerations

1. **Backend:**
   - Test with user having no applications
   - Test with applications missing job descriptions
   - Test with applications missing skills
   - Test keyword extraction and stopword filtering

2. **Frontend:**
   - Test loading states
   - Test empty states
   - Test with large datasets
   - Verify responsive layout

---

## Implementation Order

1. Backend: Interface update
2. Backend: Service implementation
3. Backend: Controller endpoint
4. Frontend: Type definitions
5. Frontend: Service function
6. Frontend: JobDescriptionAnalysis component
7. Frontend: Dashboard layout changes
8. Testing and polish
