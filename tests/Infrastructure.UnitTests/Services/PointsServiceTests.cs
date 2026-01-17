using Domain.Enums;
using FluentAssertions;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Infrastructure.UnitTests.Services;

public class PointsServiceTests
{
    private static PointsService CreateService()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationDbContext(options);
        var unitOfWork = new UnitOfWork(context);
        var streakService = new StreakService(unitOfWork);

        return new PointsService(unitOfWork, streakService);
    }

    [Theory]
    [InlineData(ApplicationStatus.Applied, 1)]
    [InlineData(ApplicationStatus.Screening, 2)]
    [InlineData(ApplicationStatus.Interview, 5)]
    [InlineData(ApplicationStatus.Offer, 50)]
    [InlineData(ApplicationStatus.Rejected, 2)]
    [InlineData(ApplicationStatus.Withdrawn, 0)]
    public void CalculatePoints_ReturnsExpectedValue(ApplicationStatus status, int expected)
    {
        var service = CreateService();

        var result = service.CalculatePoints(status);

        result.Should().Be(expected);
    }
}
