using Application.DTOs.TimelineEvents;
using Application.Interfaces;
using Application.Scoring;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class TimelineEventService : ITimelineEventService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IPointsService _pointsService;

    public TimelineEventService(
        ApplicationDbContext context,
        ICurrentUserService currentUser,
        IPointsService pointsService)
    {
        _context = context;
        _currentUser = currentUser;
        _pointsService = pointsService;
    }

    public async Task<TimelineEventDto> CreateTimelineEventAsync(Guid applicationId, CreateTimelineEventRequest request)
    {
        var userId = _currentUser.GetUserId()
            ?? throw new UnauthorizedAccessException("User not authenticated");

        // Verify application belongs to user
        var application = await _context.Applications
            .Include(a => a.TimelineEvents)
            .FirstOrDefaultAsync(a => a.Id == applicationId && a.UserId == userId)
            ?? throw new KeyNotFoundException($"Application {applicationId} not found");

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

        _context.TimelineEvents.Add(timelineEvent);

        // Recalculate application points and status
        var oldPoints = application.Points;
        var totalPoints = application.TimelineEvents.Sum(e => e.Points);
        if (!application.TimelineEvents.Any(e => e.Id == timelineEvent.Id))
        {
            totalPoints += points;
        }
        application.Points = totalPoints;
        application.Status = application.ComputeCurrentStatus();
        application.UpdatedDate = DateTime.UtcNow;

        // Update user total points
        var pointsDelta = application.Points - oldPoints;
        if (pointsDelta != 0)
        {
            await _pointsService.UpdateUserTotalPointsAsync(userId, pointsDelta);
        }

        await _context.SaveChangesAsync();

        // Leaderboard notification handled by PointsService
        return MapToDto(timelineEvent);
    }

    public async Task<TimelineEventsListResponse> GetTimelineEventsAsync(Guid applicationId)
    {
        var userId = _currentUser.GetUserId()
            ?? throw new UnauthorizedAccessException("User not authenticated");

        // Verify application belongs to user
        var application = await _context.Applications
            .Include(a => a.TimelineEvents)
            .FirstOrDefaultAsync(a => a.Id == applicationId && a.UserId == userId)
            ?? throw new KeyNotFoundException($"Application {applicationId} not found");

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
        var application = await _context.Applications
            .Include(a => a.TimelineEvents)
            .FirstOrDefaultAsync(a => a.Id == applicationId && a.UserId == userId)
            ?? throw new KeyNotFoundException($"Application {applicationId} not found");

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
        application.Points = application.ComputeTotalPoints();
        application.Status = application.ComputeCurrentStatus();
        application.UpdatedDate = DateTime.UtcNow;

        // Update user total points
        var pointsDelta = application.Points - previousAppPoints;
        if (pointsDelta != 0)
        {
            await _pointsService.UpdateUserTotalPointsAsync(userId, pointsDelta);
        }

        await _context.SaveChangesAsync();

        // Leaderboard notification handled by PointsService
        return MapToDto(timelineEvent);
    }

    public async Task<bool> DeleteTimelineEventAsync(Guid applicationId, Guid eventId)
    {
        var userId = _currentUser.GetUserId()
            ?? throw new UnauthorizedAccessException("User not authenticated");

        // Verify application belongs to user
        var application = await _context.Applications
            .Include(a => a.TimelineEvents)
            .FirstOrDefaultAsync(a => a.Id == applicationId && a.UserId == userId)
            ?? throw new KeyNotFoundException($"Application {applicationId} not found");

        var timelineEvent = application.TimelineEvents.FirstOrDefault(e => e.Id == eventId);
        if (timelineEvent == null)
            return false;

        // Remove event
        _context.TimelineEvents.Remove(timelineEvent);
        application.TimelineEvents.Remove(timelineEvent);

        // Recalculate application points and status
        var oldPoints = application.Points;
        application.Points = application.ComputeTotalPoints();
        application.Status = application.ComputeCurrentStatus();
        application.UpdatedDate = DateTime.UtcNow;

        // Update user total points
        var pointsDelta = application.Points - oldPoints;
        if (pointsDelta != 0)
        {
            await _pointsService.UpdateUserTotalPointsAsync(userId, pointsDelta);
        }

        await _context.SaveChangesAsync();

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
}
