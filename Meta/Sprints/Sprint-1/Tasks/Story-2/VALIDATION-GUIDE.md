# Story 2 Backend - Validation Guide

**Story:** Quick Capture + Points (5 SP)
**Sprint:** 1 | Jan 4-17, 2026
**Developer:** cadleta (Dev A - Backend)
**Date:** 2026-01-05

---

## Purpose

This document provides a systematic checklist to validate that all Story 2 backend tasks (A1-A9) were completed correctly and meet the acceptance criteria defined in `Dev-A.md`.

---

## Validation Checklist

### A1: Create Application Domain Entity ✓

**Files to Verify:**
- [ ] `src/Domain/Entities/Application.cs` exists
- [ ] `src/Domain/Enums/ApplicationStatus.cs` exists

**Application Entity Requirements:**
- [ ] Has `Guid Id` property
- [ ] Has `string UserId` property (string for Identity compatibility)
- [ ] Has `string CompanyName` property with default empty string
- [ ] Has `string RoleTitle` property with default empty string
- [ ] Has `string SourceName` property with default empty string
- [ ] Has `string? SourceUrl` nullable property
- [ ] Has `ApplicationStatus Status` property
- [ ] Has `int Points` property
- [ ] Has `DateTime CreatedDate` property
- [ ] Has `DateTime UpdatedDate` property
- [ ] Has `ApplicationUser User` navigation property

**ApplicationStatus Enum Requirements:**
- [ ] Has `Applied = 0`
- [ ] Has `Screening = 1`
- [ ] Has `Interview = 2`
- [ ] Has `Offer = 3`
- [ ] Has `Rejected = 4`
- [ ] Has `Withdrawn = 5`

---

### A2: Update ApplicationDbContext with Applications DbSet ✓

**Files to Verify:**
- [ ] `src/Infrastructure/Data/ApplicationDbContext.cs` modified
- [ ] `src/Domain/Entities/ApplicationUser.cs` modified

**DbContext Requirements:**
- [ ] Has `DbSet<Domain.Entities.Application> Applications` property
- [ ] OnModelCreating configures Application entity with:
  - [ ] Primary key on Id
  - [ ] CompanyName: Required, MaxLength 200
  - [ ] RoleTitle: Required, MaxLength 200
  - [ ] SourceName: Required, MaxLength 100
  - [ ] SourceUrl: MaxLength 500 (nullable)
  - [ ] Index on UserId
  - [ ] Index on CreatedDate
  - [ ] Foreign key relationship to User with Cascade delete

**ApplicationUser Requirements:**
- [ ] Has `ICollection<Application> Applications` property initialized to empty list
- [ ] Has `int TotalPoints` property

---

### A3: Create and Apply Migration for Applications Table ✓

**Migration Verification:**
- [ ] Migration file exists in `src/Infrastructure/Data/Migrations/`
- [ ] Migration name contains "AddApplicationsTable"
- [ ] Up method creates Applications table
- [ ] Up method adds TotalPoints column to AspNetUsers
- [ ] Down method drops Applications table
- [ ] Down method removes TotalPoints column

**Database Verification:**
- [ ] Migration was applied successfully (no errors)
- [ ] Applications table exists in database
- [ ] Applications table has all required columns
- [ ] AspNetUsers table has TotalPoints column
- [ ] Indexes are created (IX_Applications_UserId, IX_Applications_CreatedDate)
- [ ] Foreign key constraint exists

---

### A4: Create Application DTOs ✓

**Files to Verify:**
- [ ] `src/Application/DTOs/Applications/CreateApplicationRequest.cs` exists
- [ ] `src/Application/DTOs/Applications/CreateApplicationResponse.cs` exists
- [ ] `src/Application/DTOs/Applications/ApplicationDto.cs` exists

**CreateApplicationRequest Requirements:**
- [ ] CompanyName: [Required], [MaxLength(200)]
- [ ] RoleTitle: [Required], [MaxLength(200)]
- [ ] SourceName: [Required], [MaxLength(100)]
- [ ] SourceUrl: [MaxLength(500)], [Url] (nullable)
- [ ] All validation messages are user-friendly

**CreateApplicationResponse Requirements:**
- [ ] Has Guid Id
- [ ] Has string CompanyName
- [ ] Has string RoleTitle
- [ ] Has string SourceName
- [ ] Has string? SourceUrl
- [ ] Has string Status (default "Applied")
- [ ] Has int Points
- [ ] Has int TotalPoints
- [ ] Has DateTime CreatedDate

**ApplicationDto Requirements:**
- [ ] Has all required properties for list views
- [ ] Includes Status, Points, CreatedDate, UpdatedDate

---

### A5: Create Points Calculation Service ✓

**Files to Verify:**
- [ ] `src/Application/Interfaces/IPointsService.cs` exists
- [ ] `src/Infrastructure/Services/PointsService.cs` exists

**IPointsService Requirements:**
- [ ] Has `int CalculatePoints(ApplicationStatus status)` method
- [ ] Has `Task<int> UpdateUserTotalPointsAsync(string userId, int pointsToAdd)` method

**PointsService Requirements:**
- [ ] Implements IPointsService
- [ ] CalculatePoints returns:
  - [ ] 1 for Applied
  - [ ] 2 for Screening
  - [ ] 5 for Interview
  - [ ] 5 for Rejected
  - [ ] 50 for Offer
  - [ ] 0 for others
- [ ] UpdateUserTotalPointsAsync:
  - [ ] Finds user by userId
  - [ ] Throws exception if user not found
  - [ ] Adds points to user.TotalPoints
  - [ ] Saves changes to database
  - [ ] Returns updated total points

---

### A6: Create Application Service ✓

**Files to Verify:**
- [ ] `src/Application/Interfaces/IApplicationService.cs` exists
- [ ] `src/Infrastructure/Services/ApplicationService.cs` exists

**IApplicationService Requirements:**
- [ ] Has `Task<CreateApplicationResponse> CreateApplicationAsync(CreateApplicationRequest request)` method

**ApplicationService Requirements:**
- [ ] Implements IApplicationService
- [ ] Constructor injects: ApplicationDbContext, ICurrentUserService, IPointsService
- [ ] CreateApplicationAsync:
  - [ ] Gets userId from ICurrentUserService.GetUserId()
  - [ ] Throws UnauthorizedAccessException if userId is null
  - [ ] Calculates points using PointsService.CalculatePoints(ApplicationStatus.Applied)
  - [ ] Creates Application entity with:
    - [ ] New Guid for Id
    - [ ] UserId from current user
    - [ ] Properties from request
    - [ ] Status = ApplicationStatus.Applied
    - [ ] Points from calculation
    - [ ] CreatedDate = DateTime.UtcNow
    - [ ] UpdatedDate = DateTime.UtcNow
  - [ ] Adds application to context
  - [ ] Updates user total points via PointsService
  - [ ] Saves changes
  - [ ] Returns CreateApplicationResponse with all fields populated

---

### A7: Create Applications Controller ✓

**Files to Verify:**
- [ ] `src/WebAPI/Controllers/ApplicationsController.cs` exists

**Controller Requirements:**
- [ ] Has [Authorize] attribute on class
- [ ] Has [ApiController] attribute
- [ ] Has [Route("api/[controller]")] attribute
- [ ] Constructor injects IApplicationService

**POST /api/applications Endpoint:**
- [ ] Has [HttpPost] attribute
- [ ] Has [ProducesResponseType(typeof(CreateApplicationResponse), 201)] attribute
- [ ] Has [ProducesResponseType(400)] attribute
- [ ] Has [ProducesResponseType(401)] attribute
- [ ] Accepts [FromBody] CreateApplicationRequest
- [ ] Calls _applicationService.CreateApplicationAsync(request)
- [ ] Returns CreatedAtAction with:
  - [ ] Action name: nameof(GetApplication)
  - [ ] Route values: { id = result.Id }
  - [ ] Value: result

**GET /api/applications/{id} Endpoint:**
- [ ] Exists as placeholder for Story 3
- [ ] Returns NotFound()

---

### A8: Register Services in DI Container ✓

**Files to Verify:**
- [ ] `src/WebAPI/Program.cs` modified

**Service Registration Requirements:**
- [ ] IPointsService registered as Scoped → PointsService
- [ ] IApplicationService registered as Scoped → ApplicationService
- [ ] Registrations appear in correct location (after existing services)

**Additional Updates:**
- [ ] GetMeResponse has TotalPoints property
- [ ] AuthController /me endpoint returns TotalPoints

---

### A9: Write Integration Tests ✓

**Files to Verify:**
- [ ] `tests/WebAPI.IntegrationTests/Controllers/ApplicationsControllerTests.cs` exists

**Test Infrastructure:**
- [ ] Uses CustomWebApplicationFactory
- [ ] Uses FluentAssertions
- [ ] Each test is independent

**Required Test Cases:**
- [ ] CreateApplication_WithValidData_Returns201AndApplication
- [ ] CreateApplication_WithMissingCompanyName_Returns400
- [ ] CreateApplication_WithMissingRoleTitle_Returns400
- [ ] CreateApplication_WithMissingSourceName_Returns400
- [ ] CreateApplication_WithoutToken_Returns401
- [ ] CreateApplication_AddsPointsToUser
- [ ] CreateApplication_StoresApplicationInDatabase

**Bonus Test Cases:**
- [ ] CreateApplication_WithSourceUrl_Returns201AndIncludesUrl
- [ ] CreateApplication_WithInvalidUrl_Returns400
- [ ] CreateApplication_WithTooLongCompanyName_Returns400
- [ ] CreateApplication_WithInvalidToken_Returns401
- [ ] CreateApplication_MultipleUsers_IsolatesPoints

---

## Build & Compile Verification

- [ ] `dotnet build` succeeds with 0 errors
- [ ] No compiler warnings related to new code
- [ ] All project references resolve correctly
- [ ] Solution builds from clean state

---

## API Contract Verification

**Request Format:**
```json
POST /api/applications
Authorization: Bearer {token}
Content-Type: application/json

{
  "companyName": "string (required, max 200)",
  "roleTitle": "string (required, max 200)",
  "sourceName": "string (required, max 100)",
  "sourceUrl": "string (optional, max 500, valid URL)"
}
```

**Success Response (201 Created):**
```json
{
  "id": "guid",
  "companyName": "string",
  "roleTitle": "string",
  "sourceName": "string",
  "sourceUrl": "string | null",
  "status": "Applied",
  "points": 1,
  "totalPoints": 42,
  "createdDate": "2026-01-05T10:30:00Z"
}
```

**Validation Error (400):**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Validation Failed",
  "status": 400,
  "errors": {
    "companyName": ["Company name is required"]
  }
}
```

**Unauthorized (401):**
- [ ] Returns 401 when no token provided
- [ ] Returns 401 when invalid token provided

---

## Code Quality Standards

**Naming Conventions:**
- [ ] All C# classes use PascalCase
- [ ] All properties use PascalCase
- [ ] All parameters use camelCase
- [ ] All private fields use _camelCase

**Architecture:**
- [ ] Domain layer has no dependencies on other layers
- [ ] Application layer depends only on Domain
- [ ] Infrastructure implements Application interfaces
- [ ] WebAPI depends on all layers

**Clean Code:**
- [ ] No unused using statements
- [ ] No commented-out code
- [ ] Proper XML documentation on public APIs
- [ ] Consistent formatting

---

## Acceptance Criteria (from Dev-A.md)

**Completion Criteria:**
- [ ] POST /api/applications creates application with status "Applied"
- [ ] Application is stored with UserId from JWT claims
- [ ] +1 point is added to user's TotalPoints
- [ ] Response includes application details and updated totalPoints
- [ ] Validation errors return 400 with field-specific messages
- [ ] Unauthenticated requests return 401
- [ ] Integration tests pass

**Integration Readiness:**
- [ ] API runs on known port (https://localhost:5001)
- [ ] CORS allows frontend origin
- [ ] POST /api/applications creates application in database
- [ ] Response includes correct points (+1)
- [ ] User's TotalPoints is updated
- [ ] GET /api/auth/me returns updated points
- [ ] Validation errors return 400 with field-specific messages
- [ ] Missing token returns 401

---

## Final Sign-Off

**Verified By:** _________________
**Date:** _________________
**Status:** [ ] PASS / [ ] FAIL

**Issues Found:**
1.
2.
3.

**Recommendations:**
1.
2.
3.

---

## Notes

This validation guide serves as a comprehensive checklist to ensure Story 2 backend implementation meets all requirements and is ready for frontend integration.
