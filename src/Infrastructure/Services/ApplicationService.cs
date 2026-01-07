using Application.DTOs.Applications;
using Application.DTOs.TimelineEvents;
using Application.Interfaces;
using Application.Scoring;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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
        var userId = _currentUser.GetUserId()
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var normalizedSourceUrl = string.IsNullOrWhiteSpace(request.SourceUrl)
            ? null
            : request.SourceUrl.Trim();

        var createdDate = DateTime.UtcNow;

        // Create application with initial timeline event
        var application = new Domain.Entities.Application
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CompanyName = string.Empty,
            RoleTitle = string.Empty,
            SourceName = GetSourceNameFromUrl(normalizedSourceUrl),
            SourceUrl = normalizedSourceUrl,
            Status = ApplicationStatus.Applied,
            WorkMode = WorkMode.Unknown,
            ParsedByAI = false,
            AiParsingStatus = AiParsingStatus.Pending,
            RawPageContent = request.RawPageContent.Trim(),
            Points = 1, // Will be computed from timeline events
            CreatedDate = createdDate,
            UpdatedDate = createdDate
        };

        // Create initial "Applied" timeline event
        var initialPoints = PointsRules.GetPoints(EventType.Applied);
        var timelineEvent = new TimelineEvent
        {
            Id = Guid.NewGuid(),
            ApplicationId = application.Id,
            EventType = EventType.Applied,
            InterviewRound = null,
            Timestamp = createdDate,
            Notes = "Application created",
            Points = initialPoints,
            CreatedDate = createdDate
        };

        application.TimelineEvents.Add(timelineEvent);
        _context.Applications.Add(application);
        _context.TimelineEvents.Add(timelineEvent);

        var totalPoints = await _pointsService.UpdateUserTotalPointsAsync(userId, initialPoints);

        await _context.SaveChangesAsync();

        return new CreateApplicationResponse
        {
            Id = application.Id,
            SourceUrl = application.SourceUrl,
            Status = application.Status.ToString(),
            AiParsingStatus = application.AiParsingStatus.ToString(),
            Points = application.Points,
            TotalPoints = totalPoints,
            CreatedDate = application.CreatedDate
        };
    }

    public async Task<ApplicationsListResponse> GetApplicationsAsync(int page, int pageSize)
    {
        var userId = _currentUser.GetUserId()
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var query = _context.Applications
            .AsNoTracking()
            .Where(application => application.UserId == userId)
            .OrderByDescending(application => application.CreatedDate);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize + 1)
            .Select(application => new ApplicationListDto
            {
                Id = application.Id,
                CompanyName = application.CompanyName,
                RoleTitle = application.RoleTitle,
                Status = application.Status.ToString(),
                CreatedDate = application.CreatedDate
            })
            .ToListAsync();

        var hasMore = items.Count > pageSize;
        if (hasMore)
        {
            items.RemoveAt(items.Count - 1);
        }

        return new ApplicationsListResponse
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            HasMore = hasMore
        };
    }

    public async Task<ApplicationDto?> GetApplicationAsync(Guid id)
    {
        var userId = _currentUser.GetUserId()
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var application = await _context.Applications
            .AsNoTracking()
            .Include(a => a.TimelineEvents)
            .FirstOrDefaultAsync(item => item.Id == id && item.UserId == userId);

        if (application == null)
        {
            return null;
        }

        return MapToDetailDto(application);
    }

    public async Task<ApplicationDto?> UpdateApplicationAsync(Guid id, UpdateApplicationRequest request)
    {
        var userId = _currentUser.GetUserId()
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var application = await _context.Applications
            .FirstOrDefaultAsync(item => item.Id == id && item.UserId == userId);

        if (application == null)
        {
            return null;
        }

        application.CompanyName = request.CompanyName.Trim();
        application.RoleTitle = request.RoleTitle.Trim();
        application.SourceName = request.SourceName.Trim();
        application.SourceUrl = string.IsNullOrWhiteSpace(request.SourceUrl)
            ? null
            : request.SourceUrl.Trim();
        application.Location = string.IsNullOrWhiteSpace(request.Location)
            ? null
            : request.Location.Trim();
        application.JobDescription = string.IsNullOrWhiteSpace(request.JobDescription)
            ? null
            : request.JobDescription.Trim();
        var requiredSkills = request.RequiredSkills ?? new List<string>();
        var niceToHaveSkills = request.NiceToHaveSkills ?? new List<string>();

        application.RequiredSkills = requiredSkills
            .Where(skill => !string.IsNullOrWhiteSpace(skill))
            .Select(skill => skill.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        application.NiceToHaveSkills = niceToHaveSkills
            .Where(skill => !string.IsNullOrWhiteSpace(skill))
            .Select(skill => skill.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        application.RawPageContent = string.IsNullOrWhiteSpace(request.RawPageContent)
            ? null
            : request.RawPageContent.Trim();

        if (!string.IsNullOrWhiteSpace(request.WorkMode))
        {
            if (!Enum.TryParse<WorkMode>(request.WorkMode.Trim(), true, out var parsedWorkMode))
            {
                throw new InvalidOperationException("Work mode must be a valid option");
            }

            application.WorkMode = parsedWorkMode;
        }

        if (request.SalaryMin.HasValue && request.SalaryMax.HasValue
            && request.SalaryMin > request.SalaryMax)
        {
            throw new InvalidOperationException("Salary minimum cannot exceed salary maximum");
        }

        application.SalaryMin = request.SalaryMin;
        application.SalaryMax = request.SalaryMax;
        application.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapToDetailDto(application);
    }

    public async Task<bool> DeleteApplicationAsync(Guid id)
    {
        var userId = _currentUser.GetUserId()
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var application = await _context.Applications
            .FirstOrDefaultAsync(item => item.Id == id && item.UserId == userId);

        if (application == null)
        {
            return false;
        }

        _context.Applications.Remove(application);

        if (application.Points != 0)
        {
            await _pointsService.UpdateUserTotalPointsAsync(userId, -application.Points);
        }

        await _context.SaveChangesAsync();

        return true;
    }

    private static ApplicationDto MapToDetailDto(Domain.Entities.Application application)
    {
        return new ApplicationDto
        {
            Id = application.Id,
            CompanyName = application.CompanyName,
            RoleTitle = application.RoleTitle,
            SourceName = application.SourceName,
            SourceUrl = application.SourceUrl,
            Status = application.Status.ToString(),
            WorkMode = application.WorkMode?.ToString(),
            Location = application.Location,
            SalaryMin = application.SalaryMin,
            SalaryMax = application.SalaryMax,
            JobDescription = application.JobDescription,
            RequiredSkills = application.RequiredSkills.ToList(),
            NiceToHaveSkills = application.NiceToHaveSkills.ToList(),
            ParsedByAI = application.ParsedByAI,
            AiParsingStatus = application.AiParsingStatus.ToString(),
            Points = application.Points,
            CreatedDate = application.CreatedDate,
            UpdatedDate = application.UpdatedDate,
            LastAIParsedDate = application.LastAIParsedDate,
            RawPageContent = application.RawPageContent,
            TimelineEvents = application.TimelineEvents
                .OrderByDescending(e => e.Timestamp)
                .ThenByDescending(e => e.CreatedDate)
                .ThenByDescending(e => e.Id)
                .Select(e => new TimelineEventDto
                {
                    Id = e.Id,
                    ApplicationId = e.ApplicationId,
                    EventType = e.EventType.ToString(),
                    InterviewRound = e.InterviewRound,
                    Timestamp = e.Timestamp,
                    Notes = e.Notes,
                    Points = e.Points,
                    CreatedDate = e.CreatedDate
                })
                .ToList()
        };
    }

    private static string GetSourceNameFromUrl(string? sourceUrl)
    {
        if (string.IsNullOrWhiteSpace(sourceUrl))
        {
            return string.Empty;
        }

        if (!Uri.TryCreate(sourceUrl, UriKind.Absolute, out var uri))
        {
            return string.Empty;
        }

        var host = uri.Host;
        if (host.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
        {
            host = host[4..];
        }

        return host;
    }
}
