using Domain.Enums;

namespace Application.Interfaces;

public interface IPointsService
{
    int CalculatePoints(ApplicationStatus status);
    int CalculatePoints(EventType eventType, int? interviewRound = null);
    Task<int> UpdateUserTotalPointsAsync(string userId, int pointsToAdd);
}
