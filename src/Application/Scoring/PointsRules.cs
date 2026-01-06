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
            { ApplicationStatus.Rejected, 5 },
            { ApplicationStatus.Withdrawn, 0 }
        };

    public static int GetPoints(ApplicationStatus status)
    {
        return StatusPoints.TryGetValue(status, out var points) ? points : 0;
    }
}
