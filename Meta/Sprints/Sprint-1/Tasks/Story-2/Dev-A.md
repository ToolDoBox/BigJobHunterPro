# cadleta Task List - Backend (Dev A)

**Story:** Quick Capture + Points (5 SP)
**Sprint:** 1 | Jan 4-17, 2026
**Tech Stack:** ASP.NET Core 8, EF Core, Clean Architecture
**Branch:** `feature/story2-backend-applications`

---

## PHASE 0: API CONTRACT (SYNC POINT - DO FIRST)

Before coding, confirm these contracts with realemmetts:

### Applications Endpoint (You Build This)
| Method | Endpoint | Request Body | Response (201) |
|--------|----------|--------------|----------------|
| POST | /api/applications | `{ companyName, roleTitle, sourceName, sourceUrl? }` | `{ id, companyName, roleTitle, sourceName, sourceUrl, status, points, totalPoints, createdDate }` |

### Request DTO: CreateApplicationRequest
```json
{
  "companyName": "string (required, max 200 chars)",
  "roleTitle": "string (required, max 200 chars)",
  "sourceName": "string (required, max 100 chars)",
  "sourceUrl": "string (optional, max 500 chars, valid URL format)"
}
```

### Response DTO: CreateApplicationResponse
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

### Error Response Format (400 - Validation)
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Validation Failed",
  "status": 400,
  "errors": {
    "companyName": ["Company name is required"],
    "roleTitle": ["Role title is required"]
  }
}
```

### Points System
| Status | Points |
|--------|--------|
| Applied | +1 |
| Screening | +2 |
| Interview | +5 |
| Rejected | +5 |
| Offer | +50 |

---

## YOUR TASKS

### A1. Create Application Domain Entity
**Depends on:** Story 1 backend complete
- [ ] Create Application entity in Domain layer
- [ ] Define all required properties with data annotations
- [ ] Add navigation property to User (many-to-one)
- [ ] Create ApplicationStatus enum

**Files to create:**
- `src/Domain/Entities/Application.cs`
- `src/Domain/Enums/ApplicationStatus.cs`

**Application Entity:**
```csharp
// src/Domain/Entities/Application.cs
namespace Domain.Entities;

public class Application
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string RoleTitle { get; set; } = string.Empty;
    public string SourceName { get; set; } = string.Empty;
    public string? SourceUrl { get; set; }
    public ApplicationStatus Status { get; set; }
    public int Points { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }

    // Navigation property
    public ApplicationUser User { get; set; } = null!;
}
```

**ApplicationStatus Enum:**
```csharp
// src/Domain/Enums/ApplicationStatus.cs
namespace Domain.Enums;

public enum ApplicationStatus
{
    Applied = 0,
    Screening = 1,
    Interview = 2,
    Offer = 3,
    Rejected = 4,
    Withdrawn = 5
}
```

---

### A2. Update ApplicationDbContext with Applications DbSet
**Depends on:** A1
- [ ] Add DbSet<Application> to ApplicationDbContext
- [ ] Configure entity with fluent API (indexes, constraints)
- [ ] Add Applications collection to ApplicationUser

**Files to modify:**
- `src/Infrastructure/Data/ApplicationDbContext.cs`
- `src/Domain/Entities/ApplicationUser.cs`

**DbContext Configuration:**
```csharp
// Add to ApplicationDbContext
public DbSet<Application> Applications => Set<Application>();

// In OnModelCreating
modelBuilder.Entity<Application>(entity =>
{
    entity.HasKey(e => e.Id);
    entity.Property(e => e.CompanyName).IsRequired().HasMaxLength(200);
    entity.Property(e => e.RoleTitle).IsRequired().HasMaxLength(200);
    entity.Property(e => e.SourceName).IsRequired().HasMaxLength(100);
    entity.Property(e => e.SourceUrl).HasMaxLength(500);
    entity.HasIndex(e => e.UserId);
    entity.HasIndex(e => e.CreatedDate);
    entity.HasOne(e => e.User)
          .WithMany(u => u.Applications)
          .HasForeignKey(e => e.UserId)
          .OnDelete(DeleteBehavior.Cascade);
});
```

**Add to ApplicationUser:**
```csharp
// Add to ApplicationUser.cs
public ICollection<Application> Applications { get; set; } = new List<Application>();
public int TotalPoints { get; set; }
```

---

### A3. Create and Apply Migration for Applications Table
**Depends on:** A2
- [ ] Generate EF Core migration
- [ ] Review migration SQL
- [ ] Apply migration to development database

**Commands:**
```bash
cd src/WebAPI
dotnet ef migrations add AddApplicationsTable -p ../Infrastructure -s .
dotnet ef database update -p ../Infrastructure -s .
```

---

### A4. Create Application DTOs
**Depends on:** A1
- [ ] Create CreateApplicationRequest with validation attributes
- [ ] Create CreateApplicationResponse
- [ ] Create ApplicationDto for list responses (Story 3 prep)

**Files to create:**
- `src/Application/DTOs/Applications/CreateApplicationRequest.cs`
- `src/Application/DTOs/Applications/CreateApplicationResponse.cs`
- `src/Application/DTOs/Applications/ApplicationDto.cs`

**CreateApplicationRequest:**
```csharp
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Applications;

public class CreateApplicationRequest
{
    [Required(ErrorMessage = "Company name is required")]
    [MaxLength(200, ErrorMessage = "Company name cannot exceed 200 characters")]
    public string CompanyName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Role title is required")]
    [MaxLength(200, ErrorMessage = "Role title cannot exceed 200 characters")]
    public string RoleTitle { get; set; } = string.Empty;

    [Required(ErrorMessage = "Source name is required")]
    [MaxLength(100, ErrorMessage = "Source name cannot exceed 100 characters")]
    public string SourceName { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Source URL cannot exceed 500 characters")]
    [Url(ErrorMessage = "Source URL must be a valid URL")]
    public string? SourceUrl { get; set; }
}
```

**CreateApplicationResponse:**
```csharp
namespace Application.DTOs.Applications;

public class CreateApplicationResponse
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string RoleTitle { get; set; } = string.Empty;
    public string SourceName { get; set; } = string.Empty;
    public string? SourceUrl { get; set; }
    public string Status { get; set; } = "Applied";
    public int Points { get; set; }
    public int TotalPoints { get; set; }
    public DateTime CreatedDate { get; set; }
}
```

---

### A5. Create Points Calculation Service
**Depends on:** A1
- [ ] Create IPointsService interface
- [ ] Implement PointsService with status-based calculations
- [ ] Add method to update user total points

**Files to create:**
- `src/Application/Interfaces/IPointsService.cs`
- `src/Infrastructure/Services/PointsService.cs`

**IPointsService:**
```csharp
using Domain.Enums;

namespace Application.Interfaces;

public interface IPointsService
{
    int CalculatePoints(ApplicationStatus status);
    Task<int> UpdateUserTotalPointsAsync(Guid userId, int pointsToAdd);
}
```

**PointsService:**
```csharp
using Application.Interfaces;
using Domain.Enums;
using Infrastructure.Data;

namespace Infrastructure.Services;

public class PointsService : IPointsService
{
    private readonly ApplicationDbContext _context;

    public PointsService(ApplicationDbContext context)
    {
        _context = context;
    }

    public int CalculatePoints(ApplicationStatus status)
    {
        return status switch
        {
            ApplicationStatus.Applied => 1,
            ApplicationStatus.Screening => 2,
            ApplicationStatus.Interview => 5,
            ApplicationStatus.Rejected => 5,
            ApplicationStatus.Offer => 50,
            _ => 0
        };
    }

    public async Task<int> UpdateUserTotalPointsAsync(Guid userId, int pointsToAdd)
    {
        var user = await _context.Users.FindAsync(userId.ToString());
        if (user == null) throw new InvalidOperationException("User not found");

        user.TotalPoints += pointsToAdd;
        await _context.SaveChangesAsync();

        return user.TotalPoints;
    }
}
```

---

### A6. Create Application Service
**Depends on:** A4, A5
- [ ] Create IApplicationService interface
- [ ] Implement ApplicationService with create method
- [ ] Inject IPointsService and ICurrentUserService
- [ ] Handle transaction for application + points update

**Files to create:**
- `src/Application/Interfaces/IApplicationService.cs`
- `src/Infrastructure/Services/ApplicationService.cs`

**IApplicationService:**
```csharp
using Application.DTOs.Applications;

namespace Application.Interfaces;

public interface IApplicationService
{
    Task<CreateApplicationResponse> CreateApplicationAsync(CreateApplicationRequest request);
}
```

**ApplicationService:**
```csharp
using Application.DTOs.Applications;
using Application.Interfaces;
using Domain.Enums;
using Infrastructure.Data;

namespace Infrastructure.Services;

public class ApplicationService : IApplicationService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IPointsService _pointsService;

    public ApplicationService(
        ApplicationDbContext context,
        ICurrentUserService currentUser,
        IPointsService pointsService)
    {
        _context = context;
        _currentUser = currentUser;
        _pointsService = pointsService;
    }

    public async Task<CreateApplicationResponse> CreateApplicationAsync(CreateApplicationRequest request)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var points = _pointsService.CalculatePoints(ApplicationStatus.Applied);

        var application = new Domain.Entities.Application
        {
            Id = Guid.NewGuid(),
            UserId = Guid.Parse(userId),
            CompanyName = request.CompanyName,
            RoleTitle = request.RoleTitle,
            SourceName = request.SourceName,
            SourceUrl = request.SourceUrl,
            Status = ApplicationStatus.Applied,
            Points = points,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        };

        _context.Applications.Add(application);

        var totalPoints = await _pointsService.UpdateUserTotalPointsAsync(
            Guid.Parse(userId), points);

        await _context.SaveChangesAsync();

        return new CreateApplicationResponse
        {
            Id = application.Id,
            CompanyName = application.CompanyName,
            RoleTitle = application.RoleTitle,
            SourceName = application.SourceName,
            SourceUrl = application.SourceUrl,
            Status = application.Status.ToString(),
            Points = application.Points,
            TotalPoints = totalPoints,
            CreatedDate = application.CreatedDate
        };
    }
}
```

---

### A7. Create Applications Controller
**Depends on:** A6
- [ ] Create ApplicationsController with [Authorize] attribute
- [ ] Implement POST /api/applications endpoint
- [ ] Return 201 Created with location header
- [ ] Handle validation errors (400)

**Files to create:**
- `src/WebAPI/Controllers/ApplicationsController.cs`

**ApplicationsController:**
```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Applications;
using Application.Interfaces;

namespace WebAPI.Controllers;

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

    /// <summary>
    /// Creates a new job application (Quick Capture)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreateApplicationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateApplication([FromBody] CreateApplicationRequest request)
    {
        var result = await _applicationService.CreateApplicationAsync(request);

        return CreatedAtAction(
            nameof(GetApplication),
            new { id = result.Id },
            result
        );
    }

    /// <summary>
    /// Gets a single application by ID (placeholder for Story 3)
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApplicationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetApplication(Guid id)
    {
        // Placeholder for Story 3
        return NotFound();
    }
}
```

---

### A8. Register Services in DI Container
**Depends on:** A5, A6, A7
- [ ] Register IPointsService
- [ ] Register IApplicationService
- [ ] Verify all dependencies resolve correctly

**Files to modify:**
- `src/WebAPI/Program.cs`

**Add to Program.cs:**
```csharp
// Add after other service registrations
builder.Services.AddScoped<IPointsService, PointsService>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();
```

---

### A9. Write Integration Tests
**Depends on:** A8
- [ ] Test POST /api/applications returns 201 with valid data
- [ ] Test POST /api/applications returns 400 with missing required fields
- [ ] Test POST /api/applications returns 401 without token
- [ ] Test points are correctly added to user
- [ ] Test application is stored in database

**Files to create:**
- `tests/WebAPI.IntegrationTests/ApplicationsControllerTests.cs`

**Test Cases:**
```csharp
[Fact]
public async Task CreateApplication_WithValidData_Returns201AndApplication()
{
    // Arrange
    var request = new CreateApplicationRequest
    {
        CompanyName = "Test Company",
        RoleTitle = "Software Engineer",
        SourceName = "LinkedIn"
    };

    // Act
    var response = await _client.PostAsJsonAsync("/api/applications", request);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.Created);
    var result = await response.Content.ReadFromJsonAsync<CreateApplicationResponse>();
    result.Should().NotBeNull();
    result!.CompanyName.Should().Be("Test Company");
    result.Status.Should().Be("Applied");
    result.Points.Should().Be(1);
}

[Fact]
public async Task CreateApplication_WithMissingCompanyName_Returns400()

[Fact]
public async Task CreateApplication_WithMissingRoleTitle_Returns400()

[Fact]
public async Task CreateApplication_WithoutToken_Returns401()

[Fact]
public async Task CreateApplication_AddsPointsToUser()

[Fact]
public async Task CreateApplication_StoresApplicationInDatabase()
```

---

## INTEGRATION CHECKLIST (with realemmetts)

When realemmetts is ready to connect:
- [ ] Ensure API is running on known port (https://localhost:5001)
- [ ] Verify CORS allows frontend origin
- [ ] Test: POST /api/applications creates application in database
- [ ] Test: Response includes correct points (+1)
- [ ] Test: User's TotalPoints is updated
- [ ] Test: GET /api/auth/me returns updated points
- [ ] Test: Validation errors return 400 with field-specific messages
- [ ] Test: Missing token returns 401

---

## COMPLETION CRITERIA

- [ ] POST /api/applications creates application with status "Applied"
- [ ] Application is stored with UserId from JWT claims
- [ ] +1 point is added to user's TotalPoints
- [ ] Response includes application details and updated totalPoints
- [ ] Validation errors return 400 with field-specific messages
- [ ] Unauthenticated requests return 401
- [ ] Integration tests pass

---

## GIT WORKFLOW

**Your Branch:** `feature/story2-backend-applications`

```bash
git checkout main
git pull origin main
git checkout -b feature/story2-backend-applications
# ... do your work ...
git add .
git commit -m "feat: add POST /api/applications endpoint with points calculation"
git push -u origin feature/story2-backend-applications
```

**Commit Convention:** `feat:`, `fix:`, `refactor:`, `docs:`

**Merge Order:** You merge backend first, then realemmetts merges frontend.

---

## FILE STRUCTURE (New Files)

```
src/
├── Domain/
│   ├── Entities/
│   │   ├── ApplicationUser.cs (modified - add Applications, TotalPoints)
│   │   └── Application.cs (new)
│   └── Enums/
│       └── ApplicationStatus.cs (new)
├── Application/
│   ├── DTOs/
│   │   └── Applications/
│   │       ├── CreateApplicationRequest.cs (new)
│   │       ├── CreateApplicationResponse.cs (new)
│   │       └── ApplicationDto.cs (new)
│   └── Interfaces/
│       ├── IPointsService.cs (new)
│       └── IApplicationService.cs (new)
├── Infrastructure/
│   ├── Data/
│   │   ├── ApplicationDbContext.cs (modified)
│   │   └── Migrations/
│   │       └── YYYYMMDD_AddApplicationsTable.cs (generated)
│   └── Services/
│       ├── PointsService.cs (new)
│       └── ApplicationService.cs (new)
└── WebAPI/
    ├── Controllers/
    │   └── ApplicationsController.cs (new)
    └── Program.cs (modified - register services)

tests/
└── WebAPI.IntegrationTests/
    └── ApplicationsControllerTests.cs (new)
```
