using Application.DTOs.Applications;
using Application.Interfaces;
using Domain.Enums;
using Infrastructure.Data;

namespace Infrastructure.Services;

public class ApplicationService : IApplicationService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IPointsService _pointsService;

    public ApplicationService(
        ApplicationDbContext context,
        ICurrentUserService currentUser,
        IPointsService pointsService)
    {
        _context = context;
        _currentUser = currentUser;
        _pointsService = pointsService;
    }

    public async Task<CreateApplicationResponse> CreateApplicationAsync(CreateApplicationRequest request)
    {
        var userId = _currentUser.GetUserId()
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var points = _pointsService.CalculatePoints(ApplicationStatus.Applied);

        var application = new Domain.Entities.Application
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CompanyName = request.CompanyName,
            RoleTitle = request.RoleTitle,
            SourceName = request.SourceName,
            SourceUrl = request.SourceUrl,
            Status = ApplicationStatus.Applied,
            Points = points,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        };

        _context.Applications.Add(application);

        var totalPoints = await _pointsService.UpdateUserTotalPointsAsync(userId, points);

        await _context.SaveChangesAsync();

        return new CreateApplicationResponse
        {
            Id = application.Id,
            CompanyName = application.CompanyName,
            RoleTitle = application.RoleTitle,
            SourceName = application.SourceName,
            SourceUrl = application.SourceUrl,
            Status = application.Status.ToString(),
            Points = application.Points,
            TotalPoints = totalPoints,
            CreatedDate = application.CreatedDate
        };
    }
}
