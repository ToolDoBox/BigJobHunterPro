using Application.DTOs.AiParsing;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class AiParsingBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AiParsingBackgroundService> _logger;
    private readonly TimeSpan _pollingInterval;
    private readonly int _batchSize;
    private readonly HashSet<string> _usersToInvalidate = new();

    public AiParsingBackgroundService(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        ILogger<AiParsingBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;

        var intervalSeconds = int.TryParse(configuration["AnthropicSettings:PollingIntervalSeconds"], out var seconds)
            ? seconds
            : 5;
        _pollingInterval = TimeSpan.FromSeconds(intervalSeconds);
        _batchSize = 10; // Process up to 10 applications per cycle
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("AI Parsing Background Service started. Polling every {Interval} seconds",
            _pollingInterval.TotalSeconds);

        // Wait a bit on startup to let the app initialize
        await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingApplicationsAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Graceful shutdown, don't log as error
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AI Parsing Background Service");
            }

            await Task.Delay(_pollingInterval, stoppingToken);
        }

        _logger.LogInformation("AI Parsing Background Service stopped");
    }

    private async Task ProcessPendingApplicationsAsync(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var aiService = scope.ServiceProvider.GetRequiredService<IAiParsingService>();

        // Get pending applications
        var pendingApplications = await dbContext.Applications
            .Where(a => a.AiParsingStatus == AiParsingStatus.Pending)
            .OrderBy(a => a.CreatedDate)
            .Take(_batchSize)
            .ToListAsync(stoppingToken);

        if (pendingApplications.Count == 0)
        {
            return;
        }

        _logger.LogInformation("Processing {Count} pending applications for AI parsing",
            pendingApplications.Count);

        foreach (var application in pendingApplications)
        {
            if (stoppingToken.IsCancellationRequested)
            {
                break;
            }

            try
            {
                var result = await aiService.ParseJobPostingAsync(application.RawPageContent ?? string.Empty);

                if (result.Success)
                {
                    // Update application with parsed data
                    application.CompanyName = result.CompanyName ?? string.Empty;
                    application.RoleTitle = result.RoleTitle ?? string.Empty;
                    application.Location = result.Location;
                    application.SalaryMin = result.SalaryMin;
                    application.SalaryMax = result.SalaryMax;
                    application.JobDescription = result.JobDescription;
                    application.RequiredSkills = result.RequiredSkills;
                    application.NiceToHaveSkills = result.NiceToHaveSkills;

                    // Parse work mode if provided
                    if (!string.IsNullOrEmpty(result.WorkMode) &&
                        Enum.TryParse<WorkMode>(result.WorkMode, ignoreCase: true, out var workMode))
                    {
                        application.WorkMode = workMode;
                    }

                    // Create Contact entities from parsed contacts
                    if (result.Contacts.Count > 0)
                    {
                        foreach (var parsedContact in result.Contacts)
                        {
                            var relationship = ContactRelationship.Other;
                            if (!string.IsNullOrEmpty(parsedContact.Relationship) &&
                                Enum.TryParse<ContactRelationship>(parsedContact.Relationship, ignoreCase: true, out var parsedRelationship))
                            {
                                relationship = parsedRelationship;
                            }

                            var contact = new Contact
                            {
                                Id = Guid.NewGuid(),
                                ApplicationId = application.Id,
                                Name = parsedContact.Name,
                                Role = parsedContact.Role,
                                Relationship = relationship,
                                Emails = parsedContact.Emails,
                                Phones = parsedContact.Phones,
                                LinkedIn = parsedContact.LinkedIn,
                                CreatedDate = DateTime.UtcNow
                            };

                            dbContext.Contacts.Add(contact);
                        }

                        _logger.LogInformation(
                            "Created {ContactCount} contacts for application {Id}",
                            result.Contacts.Count, application.Id);
                    }

                    application.ParsedByAI = true;
                    application.AiParsingStatus = AiParsingStatus.Success;

                    _logger.LogInformation(
                        "Successfully parsed application {Id}: {Company} - {Role}",
                        application.Id, application.CompanyName, application.RoleTitle);
                }
                else
                {
                    application.AiParsingStatus = AiParsingStatus.Failed;
                    _logger.LogWarning(
                        "Failed to parse application {Id}: {Error}",
                        application.Id, result.ErrorMessage);
                }

                application.LastAIParsedDate = DateTime.UtcNow;
                application.UpdatedDate = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing application {Id}", application.Id);
                application.AiParsingStatus = AiParsingStatus.Failed;
                application.LastAIParsedDate = DateTime.UtcNow;
                application.UpdatedDate = DateTime.UtcNow;
            }

            // Track user for cache invalidation if skills were extracted
            if (application.AiParsingStatus == AiParsingStatus.Success &&
                (application.RequiredSkills.Count > 0 || application.NiceToHaveSkills.Count > 0))
            {
                _usersToInvalidate.Add(application.UserId);
            }
        }

        await dbContext.SaveChangesAsync(stoppingToken);

        // Invalidate analytics cache for affected users
        if (_usersToInvalidate.Count > 0)
        {
            var analyticsService = scope.ServiceProvider.GetRequiredService<IAnalyticsService>();
            foreach (var userId in _usersToInvalidate)
            {
                analyticsService.InvalidateUserCache(userId);
                _logger.LogDebug("Invalidated analytics cache for user {UserId} after AI parsing", userId);
            }
            _usersToInvalidate.Clear();
        }
    }
}
