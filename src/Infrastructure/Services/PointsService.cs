using Application.Interfaces;
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
        return status switch
        {
            ApplicationStatus.Applied => 1,
            ApplicationStatus.Screening => 2,
            ApplicationStatus.Interview => 5,
            ApplicationStatus.Rejected => 5,
            ApplicationStatus.Offer => 50,
            _ => 0
        };
    }

    public async Task<int> UpdateUserTotalPointsAsync(string userId, int pointsToAdd)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) throw new InvalidOperationException("User not found");

        user.TotalPoints += pointsToAdd;
        await _context.SaveChangesAsync();

        return user.TotalPoints;
    }
}
