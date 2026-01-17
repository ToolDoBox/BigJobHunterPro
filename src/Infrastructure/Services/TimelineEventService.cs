using Application.DTOs.TimelineEvents;
using Application.Interfaces.Data;
using Application.Interfaces;
using Application.Scoring;
using Domain.Entities;

namespace Infrastructure.Services;

public class TimelineEventService : ITimelineEventService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IPointsService _pointsService;
    private readonly IActivityEventService _activityEventService;
    private readonly IHuntingPartyService _huntingPartyService;

    public TimelineEventService(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IPointsService pointsService,
        IActivityEventService activityEventService,
        IHuntingPartyService huntingPartyService)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _pointsService = pointsService;
        _activityEventService = activityEventService;
        _huntingPartyService = huntingPartyService;
    }

    public async Task<TimelineEventDto> CreateTimelineEventAsync(Guid applicationId, CreateTimelineEventRequest request)
    {
        var userId = _currentUser.GetUserId()
            ?? throw new UnauthorizedAccessException("User not authenticated");

        // Verify application belongs to user
        var application = await _unitOfWork.Applications.GetByIdWithTimelineAsync(applicationId);
        if (application == null || application.UserId != userId)
        {
            throw new KeyNotFoundException($"Application {applicationId} not found");
        }

        // Calculate points for this event
        var points = PointsRules.GetPoints(request.EventType, request.InterviewRound);

        // Create timeline event
        var timelineEvent = new TimelineEvent
        {
            Id = Guid.NewGuid(),
            ApplicationId = applicationId,
            EventType = request.EventType,
            InterviewRound = request.InterviewRound,
            Timestamp = request.Timestamp.ToUniversalTime(),
            Notes = request.Notes?.Trim(),
            Points = points,
            CreatedDate = DateTime.UtcNow
        };

        await _unitOfWork.TimelineEvents.AddAsync(timelineEvent);

        // Recalculate application points and status
        var oldPoints = application.Points;
        application.Status = application.ComputeCurrentStatus();
        application.Points = application.ComputeTotalPoints();
        application.UpdatedDate = DateTime.UtcNow;

        // Update user total points
        var pointsDelta = application.Points - oldPoints;
        if (pointsDelta != 0)
        {
            await _pointsService.UpdateUserTotalPointsAsync(userId, pointsDelta);
        }

        await _unitOfWork.SaveChangesAsync();

        var partyId = await _huntingPartyService.GetUserPartyIdAsync(userId);
        if (partyId.HasValue)
        {
            await _activityEventService.CreateEventAsync(new Application.DTOs.ActivityEvents.CreateActivityEventRequest
            {
                PartyId = partyId.Value,
                UserId = userId,
                EventType = MapActivityEventType(request.EventType),
                PointsDelta = pointsDelta,
                CreatedDate = DateTime.UtcNow,
                CompanyName = application.CompanyName,
                RoleTitle = application.RoleTitle
            });
        }

        // Leaderboard notification handled by PointsService
        return MapToDto(timelineEvent);
    }

    public async Task<TimelineEventsListResponse> GetTimelineEventsAsync(Guid applicationId)
    {
        var userId = _currentUser.GetUserId()
            ?? throw new UnauthorizedAccessException("User not authenticated");

        // Verify application belongs to user
        var application = await _unitOfWork.Applications.GetByIdWithTimelineAsync(applicationId);
        if (application == null || application.UserId != userId)
        {
            throw new KeyNotFoundException($"Application {applicationId} not found");
        }

        var events = application.TimelineEvents
            .OrderByDescending(e => e.Timestamp)
            .ThenByDescending(e => e.CreatedDate)
            .ThenByDescending(e => e.Id)
            .Select(MapToDto)
            .ToList();

        return new TimelineEventsListResponse
        {
            ApplicationId = applicationId,
            Events = events,
            TotalPoints = application.Points,
            CurrentStatus = application.Status.ToString()
        };
    }

    public async Task<TimelineEventDto?> UpdateTimelineEventAsync(Guid applicationId, Guid eventId, UpdateTimelineEventRequest request)
    {
        var userId = _currentUser.GetUserId()
            ?? throw new UnauthorizedAccessException("User not authenticated");

        // Verify application belongs to user
        var application = await _unitOfWork.Applications.GetByIdWithTimelineAsync(applicationId);
        if (application == null || application.UserId != userId)
        {
            throw new KeyNotFoundException($"Application {applicationId} not found");
        }

        var timelineEvent = application.TimelineEvents.FirstOrDefault(e => e.Id == eventId);
        if (timelineEvent == null)
            return null;

        // Update event
        var oldPoints = timelineEvent.Points;
        timelineEvent.EventType = request.EventType;
        timelineEvent.InterviewRound = request.InterviewRound;
        timelineEvent.Timestamp = request.Timestamp.ToUniversalTime();
        timelineEvent.Notes = request.Notes?.Trim();
        timelineEvent.Points = PointsRules.GetPoints(request.EventType, request.InterviewRound);
        timelineEvent.UpdatedDate = DateTime.UtcNow;

        // Recalculate application points and status
        var previousAppPoints = application.Points;
        application.Status = application.ComputeCurrentStatus();
        application.Points = application.ComputeTotalPoints();
        application.UpdatedDate = DateTime.UtcNow;

        // Update user total points
        var pointsDelta = application.Points - previousAppPoints;
        if (pointsDelta != 0)
        {
            await _pointsService.UpdateUserTotalPointsAsync(userId, pointsDelta);
        }

        await _unitOfWork.SaveChangesAsync();

        var partyId = await _huntingPartyService.GetUserPartyIdAsync(userId);
        if (partyId.HasValue)
        {
            await _activityEventService.CreateEventAsync(new Application.DTOs.ActivityEvents.CreateActivityEventRequest
            {
                PartyId = partyId.Value,
                UserId = userId,
                EventType = MapActivityEventType(request.EventType),
                PointsDelta = pointsDelta,
                CreatedDate = DateTime.UtcNow,
                CompanyName = application.CompanyName,
                RoleTitle = application.RoleTitle
            });
        }

        // Leaderboard notification handled by PointsService
        return MapToDto(timelineEvent);
    }

    public async Task<bool> DeleteTimelineEventAsync(Guid applicationId, Guid eventId)
    {
        var userId = _currentUser.GetUserId()
            ?? throw new UnauthorizedAccessException("User not authenticated");

        // Verify application belongs to user
        var application = await _unitOfWork.Applications.GetByIdWithTimelineAsync(applicationId);
        if (application == null || application.UserId != userId)
        {
            throw new KeyNotFoundException($"Application {applicationId} not found");
        }

        var timelineEvent = application.TimelineEvents.FirstOrDefault(e => e.Id == eventId);
        if (timelineEvent == null)
            return false;

        // Remove event
        _unitOfWork.TimelineEvents.Delete(timelineEvent);
        application.TimelineEvents.Remove(timelineEvent);

        // Recalculate application points and status
        var oldPoints = application.Points;
        application.Status = application.ComputeCurrentStatus();
        application.Points = application.ComputeTotalPoints();
        application.UpdatedDate = DateTime.UtcNow;

        // Update user total points
        var pointsDelta = application.Points - oldPoints;
        if (pointsDelta != 0)
        {
            await _pointsService.UpdateUserTotalPointsAsync(userId, pointsDelta);
        }

        await _unitOfWork.SaveChangesAsync();

        // Leaderboard notification handled by PointsService
        return true;
    }

    private static TimelineEventDto MapToDto(TimelineEvent entity)
    {
        return new TimelineEventDto
        {
            Id = entity.Id,
            ApplicationId = entity.ApplicationId,
            EventType = entity.EventType.ToString(),
            InterviewRound = entity.InterviewRound,
            Timestamp = entity.Timestamp,
            Notes = entity.Notes,
            Points = entity.Points,
            CreatedDate = entity.CreatedDate
        };
    }

    private static Domain.Enums.ActivityEventType MapActivityEventType(Domain.Enums.EventType eventType)
    {
        return eventType switch
        {
            Domain.Enums.EventType.Offer => Domain.Enums.ActivityEventType.OfferReceived,
            Domain.Enums.EventType.Screening => Domain.Enums.ActivityEventType.Screening,
            Domain.Enums.EventType.Interview => Domain.Enums.ActivityEventType.Interview,
            Domain.Enums.EventType.Rejected => Domain.Enums.ActivityEventType.Rejected,
            Domain.Enums.EventType.Withdrawn => Domain.Enums.ActivityEventType.Withdrawn,
            _ => Domain.Enums.ActivityEventType.StatusUpdated
        };
    }
}
