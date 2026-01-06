using Application.Interfaces;
using Application.Scoring;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class PointsService : IPointsService
{
    private readonly ApplicationDbContext _context;

    public PointsService(ApplicationDbContext context)
    {
        _context = context;
    }

    public int CalculatePoints(ApplicationStatus status)
    {
        return PointsRules.GetPoints(status);
    }

    public async Task<int> UpdateUserTotalPointsAsync(string userId, int pointsToAdd)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) throw new InvalidOperationException("User not found");

        if (user.TotalPoints == 0 && user.Points > 0)
        {
            user.TotalPoints = user.Points;
        }

        user.Points = Math.Max(0, user.Points + pointsToAdd);
        user.TotalPoints = Math.Max(0, user.TotalPoints + pointsToAdd);

        return user.TotalPoints;
    }
}
