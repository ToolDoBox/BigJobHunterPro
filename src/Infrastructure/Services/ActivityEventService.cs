using Application.DTOs.ActivityEvents;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class ActivityEventService : IActivityEventService
{
    private static readonly int[] MilestoneThresholds = { 10, 25, 50, 100 };

    private readonly ApplicationDbContext _context;
    private readonly IActivityNotifier? _activityNotifier;
    private readonly ICurrentUserService _currentUserService;

    public ActivityEventService(
        ApplicationDbContext context,
        ICurrentUserService currentUserService,
        IActivityNotifier? activityNotifier = null)
    {
        _context = context;
        _currentUserService = currentUserService;
        _activityNotifier = activityNotifier;
    }

    public async Task<ActivityEventDto?> CreateEventAsync(CreateActivityEventRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserId))
        {
            return null;
        }

        var membership = await _context.HuntingPartyMemberships
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.UserId == request.UserId
                && m.HuntingPartyId == request.PartyId
                && m.IsActive);

        if (membership == null)
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

        _context.ActivityEvents.Add(activityEvent);
        await _context.SaveChangesAsync();

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

            var isMember = await _context.HuntingPartyMemberships
                .AsNoTracking()
                .AnyAsync(m => m.HuntingPartyId == partyId && m.UserId == userId && m.IsActive);

            if (!isMember)
            {
                return null;
            }

            var events = await _context.ActivityEvents
                .AsNoTracking()
                .Include(e => e.User)
                .Where(e => e.PartyId == partyId)
                .OrderByDescending(e => e.CreatedDate)
                .ThenByDescending(e => e.Id)
                .Take(limit + 1)
                .Select(e => new
                {
                    Event = e,
                    DisplayName = e.User != null ? e.User.DisplayName : "Unknown Hunter"
                })
                .ToListAsync();

            var hasMore = events.Count > limit;
            if (hasMore)
            {
                events.RemoveAt(events.Count - 1);
            }

            var mapped = events.Select(e => new ActivityEventDto
            {
                Id = e.Event.Id,
                PartyId = e.Event.PartyId,
                UserId = e.Event.UserId,
                UserDisplayName = e.DisplayName,
                EventType = e.Event.EventType.ToString(),
                PointsDelta = e.Event.PointsDelta,
                CreatedDate = e.Event.CreatedDate,
                CompanyName = e.Event.CompanyName,
                RoleTitle = e.Event.RoleTitle,
                MilestoneLabel = e.Event.MilestoneLabel
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
        var displayName = await _context.Users
            .AsNoTracking()
            .Where(user => user.Id == activityEvent.UserId)
            .Select(user => user.DisplayName)
            .FirstOrDefaultAsync() ?? "Unknown Hunter";

        return new ActivityEventDto
        {
            Id = activityEvent.Id,
            PartyId = activityEvent.PartyId,
            UserId = activityEvent.UserId,
            UserDisplayName = displayName,
            EventType = activityEvent.EventType.ToString(),
            PointsDelta = activityEvent.PointsDelta,
            CreatedDate = activityEvent.CreatedDate,
            CompanyName = activityEvent.CompanyName,
            RoleTitle = activityEvent.RoleTitle,
            MilestoneLabel = milestoneLabel ?? activityEvent.MilestoneLabel
        };
    }

    private async Task TryCreateMilestoneAsync(Guid partyId, string userId)
    {
        var applicationCount = await _context.Applications
            .AsNoTracking()
            .CountAsync(application => application.UserId == userId);

        var hitThreshold = MilestoneThresholds.FirstOrDefault(threshold => threshold == applicationCount);
        if (hitThreshold == 0)
        {
            return;
        }

        var milestoneLabel = $"{hitThreshold} applications logged";

        var existingMilestone = await _context.ActivityEvents
            .AsNoTracking()
            .AnyAsync(e => e.PartyId == partyId
                && e.UserId == userId
                && e.EventType == ActivityEventType.MilestoneHit
                && e.RoleTitle == milestoneLabel);

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

        _context.ActivityEvents.Add(milestoneEvent);
        await _context.SaveChangesAsync();

        var dto = await MapToDtoAsync(milestoneEvent, milestoneLabel);
        if (_activityNotifier != null)
        {
            await _activityNotifier.NotifyActivityAsync(partyId, dto);
        }
    }
}
