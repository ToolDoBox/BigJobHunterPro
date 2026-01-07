using System.Collections.Generic;
using Domain.Enums;

namespace Application.Scoring;

public static class PointsRules
{
    public static readonly IReadOnlyDictionary<ApplicationStatus, int> StatusPoints =
        new Dictionary<ApplicationStatus, int>
        {
            { ApplicationStatus.Applied, 1 },
            { ApplicationStatus.Screening, 2 },
            { ApplicationStatus.Interview, 5 },
            { ApplicationStatus.Offer, 50 },
            { ApplicationStatus.Rejected, 2 },
            { ApplicationStatus.Withdrawn, 0 }
        };

    public static readonly IReadOnlyDictionary<EventType, int> EventTypePoints =
        new Dictionary<EventType, int>
        {
            { EventType.Prospecting, 0 },
            { EventType.Applied, 1 },
            { EventType.Screening, 2 },
            { EventType.Interview, 5 },
            { EventType.Offer, 50 },
            { EventType.Rejected, 2 },
            { EventType.Withdrawn, 0 }
        };

    public static int GetPoints(ApplicationStatus status)
    {
        return StatusPoints.TryGetValue(status, out var points) ? points : 0;
    }

    public static int GetPoints(EventType eventType, int? interviewRound = null)
    {
        return EventTypePoints.TryGetValue(eventType, out var points) ? points : 0;
    }
}
