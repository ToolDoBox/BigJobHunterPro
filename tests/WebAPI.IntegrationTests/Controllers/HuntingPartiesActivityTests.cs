using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.DTOs.ActivityEvents;
using Application.DTOs.Applications;
using Application.DTOs.HuntingParty;
using Application.DTOs.Auth;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using WebAPI.IntegrationTests.Helpers;
using WebAPI.IntegrationTests.Infrastructure;
using Xunit;

namespace WebAPI.IntegrationTests.Controllers;

public class HuntingPartiesActivityTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public HuntingPartiesActivityTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<HttpClient> CreateAuthenticatedClientAsync()
    {
        var registerRequest = TestDataHelper.ValidRegisterRequest;
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        var result = await response.Content.ReadFromJsonAsync<RegisterResponse>();

        var authenticatedClient = _factory.CreateClient();
        authenticatedClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", result!.Token);
        return authenticatedClient;
    }

    [Fact]
    public async Task GetPartyActivity_ReturnsEventsInReverseChronologicalOrder()
    {
        var client = await CreateAuthenticatedClientAsync();

        var createParty = new CreateHuntingPartyRequest { Name = "Signal Squad" };
        var partyResponse = await client.PostAsJsonAsync("/api/parties", createParty);
        var party = await partyResponse.Content.ReadFromJsonAsync<HuntingPartyDto>();

        var appRequest = new CreateApplicationRequest
        {
            SourceUrl = "https://example.com/job/one",
            RawPageContent = "Sample content"
        };

        await client.PostAsJsonAsync("/api/applications", appRequest);
        await Task.Delay(25);

        await client.PostAsJsonAsync("/api/applications", new CreateApplicationRequest
        {
            SourceUrl = "https://example.com/job/two",
            RawPageContent = "Sample content"
        });

        var feedResponse = await client.GetAsync($"/api/parties/{party!.Id}/activity?limit=50");

        feedResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var feed = await feedResponse.Content.ReadFromJsonAsync<ActivityFeedResponse>();
        feed.Should().NotBeNull();
        feed!.Events.Should().NotBeEmpty();
        feed.Events.Should().BeInDescendingOrder(e => e.CreatedDate);
    }

    [Fact]
    public async Task TimelineEventUpdate_CreatesActivityEvent()
    {
        var client = await CreateAuthenticatedClientAsync();

        var createParty = new CreateHuntingPartyRequest { Name = "Offer Squad" };
        var partyResponse = await client.PostAsJsonAsync("/api/parties", createParty);
        var party = await partyResponse.Content.ReadFromJsonAsync<HuntingPartyDto>();

        var appResponse = await client.PostAsJsonAsync("/api/applications", new CreateApplicationRequest
        {
            SourceUrl = "https://example.com/job/offer",
            RawPageContent = "Sample content"
        });
        var app = await appResponse.Content.ReadFromJsonAsync<CreateApplicationResponse>();

        var timelineResponse = await client.PostAsJsonAsync(
            $"/api/applications/{app!.Id}/timeline-events",
            new Application.DTOs.TimelineEvents.CreateTimelineEventRequest
            {
                EventType = Domain.Enums.EventType.Offer,
                Timestamp = DateTime.UtcNow,
                Notes = "Offer incoming!"
            });

        timelineResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var activityEvent = dbContext.ActivityEvents
            .FirstOrDefault(e => e.PartyId == party!.Id && e.EventType == Domain.Enums.ActivityEventType.OfferReceived);

        activityEvent.Should().NotBeNull();
    }
}
