using Application.DTOs.ActivityEvents;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Infrastructure.UnitTests.Services;

public class ActivityEventServiceTests
{
    private sealed class TestCurrentUserService : Application.Interfaces.ICurrentUserService
    {
        private readonly string _userId;

        public TestCurrentUserService(string userId)
        {
            _userId = userId;
        }

        public string? GetUserId()
        {
            return _userId;
        }

        public Task<ApplicationUser?> GetCurrentUserAsync()
        {
            return Task.FromResult<ApplicationUser?>(null);
        }
    }

    private static (ApplicationDbContext Context, ActivityEventService Service, string UserId, Guid PartyId)
        CreateService()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationDbContext(options);
        var userId = Guid.NewGuid().ToString();
        var partyId = Guid.NewGuid();

        context.Users.Add(new ApplicationUser
        {
            Id = userId,
            DisplayName = "Test Hunter",
            Email = "hunter@example.com",
            UserName = "hunter@example.com",
            CreatedDate = DateTime.UtcNow
        });

        context.HuntingParties.Add(new HuntingParty
        {
            Id = partyId,
            Name = "Test Party",
            InviteCode = "ABCDEFGH",
            CreatorId = userId,
            CreatedDate = DateTime.UtcNow
        });

        context.HuntingPartyMemberships.Add(new HuntingPartyMembership
        {
            Id = Guid.NewGuid(),
            HuntingPartyId = partyId,
            UserId = userId,
            Role = PartyRole.Creator,
            JoinedDate = DateTime.UtcNow,
            IsActive = true
        });

        context.SaveChanges();

        var unitOfWork = new UnitOfWork(context);
        var service = new ActivityEventService(unitOfWork, new TestCurrentUserService(userId));
        return (context, service, userId, partyId);
    }

    [Fact]
    public async Task CreateEventAsync_PersistsActivityEvent()
    {
        var (context, service, userId, partyId) = CreateService();

        var request = new CreateActivityEventRequest
        {
            PartyId = partyId,
            UserId = userId,
            EventType = ActivityEventType.StatusUpdated,
            PointsDelta = 5,
            CreatedDate = DateTime.UtcNow,
            CompanyName = "Acme Corp",
            RoleTitle = "Backend Engineer"
        };

        var result = await service.CreateEventAsync(request);

        result.Should().NotBeNull();
        result!.EventType.Should().Be(ActivityEventType.StatusUpdated.ToString());
        result.PointsDelta.Should().Be(5);
        result.CompanyName.Should().Be("Acme Corp");
        result.RoleTitle.Should().Be("Backend Engineer");

        var stored = await context.ActivityEvents.FirstOrDefaultAsync();
        stored.Should().NotBeNull();
        stored!.PartyId.Should().Be(partyId);
        stored.UserId.Should().Be(userId);
        stored.EventType.Should().Be(ActivityEventType.StatusUpdated);
    }

    [Fact]
    public async Task CreateEventAsync_WhenApplicationMilestoneHit_CreatesMilestoneEvent()
    {
        var (context, service, userId, partyId) = CreateService();

        for (var i = 0; i < 10; i++)
        {
            context.Applications.Add(new Domain.Entities.Application
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CompanyName = $"Company {i}",
                RoleTitle = "Engineer",
                SourceName = "Test",
                Status = ApplicationStatus.Applied,
                WorkMode = WorkMode.Unknown,
                ParsedByAI = false,
                AiParsingStatus = AiParsingStatus.Pending,
                RawPageContent = "content",
                Points = 1,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();

        await service.CreateEventAsync(new CreateActivityEventRequest
        {
            PartyId = partyId,
            UserId = userId,
            EventType = ActivityEventType.ApplicationLogged,
            PointsDelta = 1,
            CreatedDate = DateTime.UtcNow
        });

        var milestone = await context.ActivityEvents
            .FirstOrDefaultAsync(e => e.EventType == ActivityEventType.MilestoneHit);

        milestone.Should().NotBeNull();
        milestone!.MilestoneLabel.Should().Be("10 applications logged");
    }
}
