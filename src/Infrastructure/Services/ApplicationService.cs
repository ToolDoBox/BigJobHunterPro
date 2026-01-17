using Application.DTOs.Applications;
using Application.Exceptions;
using Application.DTOs.TimelineEvents;
using Application.Interfaces.Data;
using Application.Interfaces;
using Application.Scoring;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Services;

public class ApplicationService : IApplicationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IPointsService _pointsService;
    private readonly IConfiguration _configuration;
    private readonly IActivityEventService _activityEventService;
    private readonly IHuntingPartyService _huntingPartyService;
    private readonly ITimelineEventService _timelineEventService;
    private readonly IMemoryCache _cache;
    private const int CacheExpirationMinutes = 2;

    public ApplicationService(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IPointsService pointsService,
        IConfiguration configuration,
        IActivityEventService activityEventService,
        IHuntingPartyService huntingPartyService,
        ITimelineEventService timelineEventService,
        IMemoryCache cache)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _pointsService = pointsService;
        _configuration = configuration;
        _activityEventService = activityEventService;
        _huntingPartyService = huntingPartyService;
        _timelineEventService = timelineEventService;
        _cache = cache;
    }

    public async Task<CreateApplicationResponse> CreateApplicationAsync(CreateApplicationRequest request)
    {
        var userId = _currentUser.GetUserId()
            ?? throw new UnauthorizedAccessException("User not authenticated");

        if (string.IsNullOrWhiteSpace(request.RawPageContent))
        {
            throw new InvalidOperationException("Job page content is required.");
        }

        await EnforceDailyAiLimitAsync(userId);

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
        await _unitOfWork.Applications.AddAsync(application);
        await _unitOfWork.TimelineEvents.AddAsync(timelineEvent);

        var totalPoints = await _pointsService.UpdateUserTotalPointsAsync(userId, initialPoints);

        await _unitOfWork.SaveChangesAsync();

        // Invalidate application list cache
        InvalidateApplicationListCache(userId);

        var partyId = await _huntingPartyService.GetUserPartyIdAsync(userId);
        if (partyId.HasValue)
        {
            await _activityEventService.CreateEventAsync(new Application.DTOs.ActivityEvents.CreateActivityEventRequest
            {
                PartyId = partyId.Value,
                UserId = userId,
                EventType = Domain.Enums.ActivityEventType.ApplicationLogged,
                PointsDelta = initialPoints,
                CreatedDate = createdDate,
                CompanyName = application.CompanyName,
                RoleTitle = application.RoleTitle
            });
        }

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

        // Generate cache key
        var cacheKey = $"applications-list-{userId}-{page}-{pageSize}";

        // Try to get from cache
        if (_cache.TryGetValue(cacheKey, out ApplicationsListResponse? cachedResponse) && cachedResponse != null)
        {
            return cachedResponse;
        }

        var items = await _unitOfWork.Applications.GetPageByUserIdAsync(
            userId,
            (page - 1) * pageSize,
            pageSize + 1);

        var mappedItems = items.Select(application => new ApplicationListDto
        {
            Id = application.Id,
            CompanyName = application.CompanyName,
            RoleTitle = application.RoleTitle,
            Status = application.Status.ToString(),
            CreatedDate = DateTime.SpecifyKind(application.CreatedDate, DateTimeKind.Utc)
        }).ToList();

        var hasMore = mappedItems.Count > pageSize;
        if (hasMore)
        {
            mappedItems.RemoveAt(mappedItems.Count - 1);
        }

        var response = new ApplicationsListResponse
        {
            Items = mappedItems,
            Page = page,
            PageSize = pageSize,
            HasMore = hasMore
        };

        // Cache for 2 minutes
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheExpirationMinutes)
        };

        _cache.Set(cacheKey, response, cacheOptions);

        return response;
    }

    public async Task<ApplicationDto?> GetApplicationAsync(Guid id)
    {
        var userId = _currentUser.GetUserId()
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var application = await _unitOfWork.Applications
            .GetByIdWithTimelineAsync(id);

        if (application == null || application.UserId != userId)
        {
            return null;
        }

        return MapToDetailDto(application);
    }

    public async Task<ApplicationDto?> UpdateApplicationAsync(Guid id, UpdateApplicationRequest request)
    {
        var userId = _currentUser.GetUserId()
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var application = await _unitOfWork.Applications.GetByIdAsync(id);

        if (application == null || application.UserId != userId)
        {
            return null;
        }

        if (!Enum.TryParse<ApplicationStatus>(request.Status.Trim(), true, out var parsedStatus))
        {
            throw new InvalidOperationException("Status must be a valid option");
        }

        if (application.Status != parsedStatus)
        {
            await _timelineEventService.CreateTimelineEventAsync(id, new CreateTimelineEventRequest
            {
                EventType = MapStatusToEventType(parsedStatus),
                Timestamp = DateTime.UtcNow,
                Notes = $"Status updated to {parsedStatus}"
            });
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

        await _unitOfWork.SaveChangesAsync();

        // Invalidate application list cache
        InvalidateApplicationListCache(userId);

        return MapToDetailDto(application);
    }

    public async Task<ApplicationDto?> UpdateApplicationStatusAsync(Guid id, UpdateStatusRequest request)
    {
        var userId = _currentUser.GetUserId()
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var application = await _unitOfWork.Applications
            .GetByIdWithTimelineAsync(id);

        if (application == null || application.UserId != userId)
        {
            return null;
        }

        var currentStatus = application.ComputeCurrentStatus();
        if (currentStatus == request.Status)
        {
            application.Status = currentStatus;
            return MapToDetailDto(application);
        }

        await _timelineEventService.CreateTimelineEventAsync(id, new CreateTimelineEventRequest
        {
            EventType = MapStatusToEventType(request.Status),
            Timestamp = DateTime.UtcNow,
            Notes = $"Status updated to {request.Status}"
        });

        var updatedApplication = await _unitOfWork.Applications.GetByIdWithTimelineAsync(id);
        if (updatedApplication == null || updatedApplication.UserId != userId)
        {
            return MapToDetailDto(application);
        }

        return MapToDetailDto(updatedApplication);
    }

    public async Task<bool> DeleteApplicationAsync(Guid id)
    {
        var userId = _currentUser.GetUserId()
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var application = await _unitOfWork.Applications.GetByIdAsync(id);

        if (application == null || application.UserId != userId)
        {
            return false;
        }

        _unitOfWork.Applications.Delete(application);

        if (application.Points != 0)
        {
            await _pointsService.UpdateUserTotalPointsAsync(userId, -application.Points);
        }

        await _unitOfWork.SaveChangesAsync();

        // Invalidate application list cache
        InvalidateApplicationListCache(userId);

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

    private async Task EnforceDailyAiLimitAsync(string userId)
    {
        var limit = CalculateDailyAiLimitPerUser();
        if (limit <= 0)
        {
            return;
        }

        var todayUtc = DateTime.UtcNow.Date;
        var tomorrowUtc = todayUtc.AddDays(1);

        var usageCount = await _unitOfWork.Applications.CountByUserIdInRangeAsync(
            userId,
            todayUtc,
            tomorrowUtc);

        if (usageCount >= limit)
        {
            throw new RateLimitExceededException(
                $"Daily AI limit reached ({limit} quick captures per day). Please try again tomorrow.");
        }
    }

    private int CalculateDailyAiLimitPerUser()
    {
        var monthlyBudget = GetDoubleSetting("AiBudget:MonthlyBudgetUsd", 5);
        var activeUsers = GetIntSetting("AiBudget:ActiveMonthlyUsers", 10);
        var estimatedInputTokens = GetIntSetting("AiBudget:EstimatedInputTokensPerRequest", 3800);
        var maxOutputTokens = GetIntSetting("AnthropicSettings:MaxTokens", 1024);
        var inputCostPerMillion = GetDoubleSetting("AiBudget:CostPerMillionInputTokensUsd", 0.25);
        var outputCostPerMillion = GetDoubleSetting("AiBudget:CostPerMillionOutputTokensUsd", 1.25);
        var maxDailyLimit = GetIntSetting("AiBudget:MaxDailyPerUserLimit", 25);

        if (monthlyBudget <= 0 || activeUsers <= 0 || estimatedInputTokens <= 0 || maxOutputTokens <= 0)
        {
            return 0;
        }

        var estimatedCostPerRequest =
            (estimatedInputTokens / 1_000_000d) * inputCostPerMillion +
            (maxOutputTokens / 1_000_000d) * outputCostPerMillion;

        if (estimatedCostPerRequest <= 0)
        {
            return 0;
        }

        var monthlyRequests = monthlyBudget / estimatedCostPerRequest;
        var perUserMonthly = monthlyRequests / activeUsers;
        var perUserDaily = Math.Floor(perUserMonthly / 30d);

        var clampedDaily = Math.Max(1, Math.Min(maxDailyLimit, (int)perUserDaily));
        return clampedDaily;
    }

    private int GetIntSetting(string key, int fallback)
    {
        return int.TryParse(_configuration[key], out var parsed) ? parsed : fallback;
    }

    private double GetDoubleSetting(string key, double fallback)
    {
        return double.TryParse(_configuration[key], out var parsed) ? parsed : fallback;
    }

    private static EventType MapStatusToEventType(ApplicationStatus status)
    {
        return status switch
        {
            ApplicationStatus.Applied => EventType.Applied,
            ApplicationStatus.Screening => EventType.Screening,
            ApplicationStatus.Interview => EventType.Interview,
            ApplicationStatus.Offer => EventType.Offer,
            ApplicationStatus.Rejected => EventType.Rejected,
            ApplicationStatus.Withdrawn => EventType.Withdrawn,
            _ => EventType.Applied
        };
    }

    /// <summary>
    /// Invalidates all cached application list pages for a user
    /// </summary>
    private void InvalidateApplicationListCache(string userId)
    {
        // Since we cache by page and pageSize, we need to remove all possible combinations
        // Common page sizes: 25, 50, 100
        // Max reasonable pages: 20 (500 applications)
        var commonPageSizes = new[] { 25, 50, 100 };
        var maxPages = 20;

        foreach (var pageSize in commonPageSizes)
        {
            for (var page = 1; page <= maxPages; page++)
            {
                var cacheKey = $"applications-list-{userId}-{page}-{pageSize}";
                _cache.Remove(cacheKey);
            }
        }
    }
}
