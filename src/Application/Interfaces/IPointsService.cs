using Domain.Enums;

namespace Application.Interfaces;

public interface IPointsService
{
    int CalculatePoints(ApplicationStatus status);
    Task<int> UpdateUserTotalPointsAsync(string userId, int pointsToAdd);
}
