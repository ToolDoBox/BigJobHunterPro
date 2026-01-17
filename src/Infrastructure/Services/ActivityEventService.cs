using Application.DTOs.ActivityEvents;
using Application.Interfaces.Data;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Infrastructure.Services;

public class ActivityEventService : IActivityEventService
{
    private static readonly int[] MilestoneThresholds = { 10, 25, 50, 100 };

    private readonly IUnitOfWork _unitOfWork;
    private readonly IActivityNotifier? _activityNotifier;
    private readonly ICurrentUserService _currentUserService;

    public ActivityEventService(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IActivityNotifier? activityNotifier = null)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _activityNotifier = activityNotifier;
    }

    public async Task<ActivityEventDto?> CreateEventAsync(CreateActivityEventRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserId))
        {
            return null;
        }

        var isMember = await _unitOfWork.HuntingPartyMemberships.IsMemberAsync(
            request.PartyId,
            request.UserId);

        if (!isMember)
        {
            return null;
        }

        var createdDate = request.CreatedDate.Kind == DateTimeKind.Utc
            ? request.CreatedDate
            : request.CreatedDate.ToUniversalTime();

        var activityEvent = new ActivityEvent
        {
            Id = Guid.NewGuid(),
            PartyId = request.PartyId,
            UserId = request.UserId,
            EventType = request.EventType,
            PointsDelta = request.PointsDelta,
            CreatedDate = createdDate,
            CompanyName = string.IsNullOrWhiteSpace(request.CompanyName) ? null : request.CompanyName.Trim(),
            RoleTitle = string.IsNullOrWhiteSpace(request.RoleTitle) ? null : request.RoleTitle.Trim(),
            MilestoneLabel = string.IsNullOrWhiteSpace(request.MilestoneLabel) ? null : request.MilestoneLabel.Trim()
        };

        await _unitOfWork.ActivityEvents.AddAsync(activityEvent);
        await _unitOfWork.SaveChangesAsync();

        var dto = await MapToDtoAsync(activityEvent, request.MilestoneLabel);

        if (_activityNotifier != null)
        {
            await _activityNotifier.NotifyActivityAsync(request.PartyId, dto);
        }

        if (request.EventType == ActivityEventType.ApplicationLogged)
        {
            await TryCreateMilestoneAsync(request.PartyId, request.UserId);
        }

        return dto;
    }

    public async Task<ActivityFeedResponse?> GetPartyActivityAsync(Guid partyId, int limit)
    {
        try
        {
            var userId = _currentUserService.GetUserId()
                ?? throw new UnauthorizedAccessException("User not authenticated");

            var isMember = await _unitOfWork.HuntingPartyMemberships.IsMemberAsync(
                partyId,
                userId);

            if (!isMember)
            {
                return null;
            }

            var events = await _unitOfWork.ActivityEvents.GetByPartyIdAsync(partyId, limit + 1);
            var orderedEvents = events
                .OrderByDescending(e => e.CreatedDate)
                .ThenByDescending(e => e.Id)
                .ToList();

            var hasMore = orderedEvents.Count > limit;
            if (hasMore)
            {
                orderedEvents.RemoveAt(orderedEvents.Count - 1);
            }

            var mapped = orderedEvents.Select(e => new ActivityEventDto
            {
                Id = e.Id,
                PartyId = e.PartyId,
                UserId = e.UserId,
                UserDisplayName = e.User?.DisplayName ?? "Unknown Hunter",
                EventType = e.EventType.ToString(),
                PointsDelta = e.PointsDelta,
                CreatedDate = DateTime.SpecifyKind(e.CreatedDate, DateTimeKind.Utc),
                CompanyName = e.CompanyName,
                RoleTitle = e.RoleTitle,
                MilestoneLabel = e.MilestoneLabel
            }).ToList();

            return new ActivityFeedResponse
            {
                PartyId = partyId,
                Events = mapped,
                HasMore = hasMore
            };
        }
        catch (Exception ex)
        {
            // Log the exception details to console for debugging
            Console.WriteLine($"ERROR in GetPartyActivityAsync: {ex.GetType().Name}: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.GetType().Name}: {ex.InnerException.Message}");
            }
            throw; // Re-throw to maintain error behavior
        }
    }

    private async Task<ActivityEventDto> MapToDtoAsync(ActivityEvent activityEvent, string? milestoneLabel)
    {
        var displayName = await _unitOfWork.Users.GetDisplayNameAsync(activityEvent.UserId)
            ?? "Unknown Hunter";

        return new ActivityEventDto
        {
            Id = activityEvent.Id,
            PartyId = activityEvent.PartyId,
            UserId = activityEvent.UserId,
            UserDisplayName = displayName,
            EventType = activityEvent.EventType.ToString(),
            PointsDelta = activityEvent.PointsDelta,
            CreatedDate = DateTime.SpecifyKind(activityEvent.CreatedDate, DateTimeKind.Utc),
            CompanyName = activityEvent.CompanyName,
            RoleTitle = activityEvent.RoleTitle,
            MilestoneLabel = milestoneLabel ?? activityEvent.MilestoneLabel
        };
    }

    private async Task TryCreateMilestoneAsync(Guid partyId, string userId)
    {
        var applicationCount = await _unitOfWork.Applications.CountByUserIdAsync(userId);

        var hitThreshold = MilestoneThresholds.FirstOrDefault(threshold => threshold == applicationCount);
        if (hitThreshold == 0)
        {
            return;
        }

        var milestoneLabel = $"{hitThreshold} applications logged";

        var existingMilestone = await _unitOfWork.ActivityEvents.MilestoneExistsAsync(
            partyId,
            userId,
            milestoneLabel);

        if (existingMilestone)
        {
            return;
        }

        var milestoneEvent = new ActivityEvent
        {
            Id = Guid.NewGuid(),
            PartyId = partyId,
            UserId = userId,
            EventType = ActivityEventType.MilestoneHit,
            PointsDelta = 0,
            CreatedDate = DateTime.UtcNow,
            MilestoneLabel = milestoneLabel
        };

        await _unitOfWork.ActivityEvents.AddAsync(milestoneEvent);
        await _unitOfWork.SaveChangesAsync();

        var dto = await MapToDtoAsync(milestoneEvent, milestoneLabel);
        if (_activityNotifier != null)
        {
            await _activityNotifier.NotifyActivityAsync(partyId, dto);
        }
    }
}
