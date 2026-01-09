using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.DTOs.ActivityEvents;
using Application.DTOs.Applications;
using Application.DTOs.Auth;
using Application.DTOs.HuntingParty;
using FluentAssertions;
using Microsoft.AspNetCore.SignalR.Client;
using WebAPI.IntegrationTests.Helpers;
using WebAPI.IntegrationTests.Infrastructure;
using Xunit;

namespace WebAPI.IntegrationTests.Controllers;

public class ActivityHubTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public ActivityHubTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ActivityHub_BroadcastsActivityEvent()
    {
        var client = _factory.CreateClient();
        var registerRequest = TestDataHelper.ValidRegisterRequest;
        var registerResponse = await client.PostAsJsonAsync("/api/auth/register", registerRequest);
        var registerResult = await registerResponse.Content.ReadFromJsonAsync<RegisterResponse>();
        var token = registerResult!.Token;

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createParty = new CreateHuntingPartyRequest { Name = "Broadcast Squad" };
        var partyResponse = await client.PostAsJsonAsync("/api/parties", createParty);
        var party = await partyResponse.Content.ReadFromJsonAsync<HuntingPartyDto>();

        var connection = new HubConnectionBuilder()
            .WithUrl(new Uri(client.BaseAddress!, "/hubs/activity"), options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(token);
                options.HttpMessageHandlerFactory = _ => _factory.Server.CreateHandler();
            })
            .WithAutomaticReconnect()
            .Build();

        var tcs = new TaskCompletionSource<ActivityEventDto>(TaskCreationOptions.RunContinuationsAsynchronously);
        connection.On<ActivityEventDto>("ActivityEventCreated", payload =>
        {
            tcs.TrySetResult(payload);
        });

        await connection.StartAsync();

        await client.PostAsJsonAsync("/api/applications", new CreateApplicationRequest
        {
            SourceUrl = "https://example.com/job/broadcast",
            RawPageContent = "Sample content"
        });

        var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(5)));
        completedTask.Should().Be(tcs.Task);

        var payload = await tcs.Task;
        payload.PartyId.Should().Be(party!.Id);
        payload.EventType.Should().Be("ApplicationLogged");

        await connection.StopAsync();
        await connection.DisposeAsync();
    }
}
