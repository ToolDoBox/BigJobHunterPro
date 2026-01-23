using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Data;

public static class SeedData
{
    /// <summary>
    /// Seeds the database with test data for development environment.
    /// Call this from Program.cs in Development environment only.
    /// </summary>
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Ensure database is created - use EnsureCreated for SQLite (no migrations), Migrate for SQL Server
        if (context.Database.IsSqlite())
        {
            await context.Database.EnsureCreatedAsync();
        }
        else
        {
            await context.Database.MigrateAsync();
        }

        // Seed test users if they don't exist
        await SeedTestUsersAsync(userManager);

        // Seed test applications for hunter@test.com
        await SeedTestApplicationsAsync(context, userManager);
    }

    private static async Task SeedTestUsersAsync(UserManager<ApplicationUser> userManager)
    {
        // Test User 1: Active Hunter
        if (await userManager.FindByEmailAsync("hunter@test.com") == null)
        {
            var testUser1 = new ApplicationUser
            {
                Email = "hunter@test.com",
                UserName = "hunter@test.com",
                DisplayName = "Test Hunter",
                Points = 150,
                TotalPoints = 150,
                EmailConfirmed = true,
                CreatedDate = DateTime.UtcNow.AddDays(-30)
            };

            await userManager.CreateAsync(testUser1, "Hunter123!");
        }

        // Test User 2: New Hunter
        if (await userManager.FindByEmailAsync("newbie@test.com") == null)
        {
            var testUser2 = new ApplicationUser
            {
                Email = "newbie@test.com",
                UserName = "newbie@test.com",
                DisplayName = "Newbie Hunter",
                Points = 0,
                TotalPoints = 0,
                EmailConfirmed = true,
                CreatedDate = DateTime.UtcNow
            };

            await userManager.CreateAsync(testUser2, "Newbie123!");
        }

        // Test User 3: Pro Hunter
        if (await userManager.FindByEmailAsync("pro@test.com") == null)
        {
            var testUser3 = new ApplicationUser
            {
                Email = "pro@test.com",
                UserName = "pro@test.com",
                DisplayName = "Pro Hunter",
                Points = 500,
                TotalPoints = 500,
                EmailConfirmed = true,
                CreatedDate = DateTime.UtcNow.AddDays(-90)
            };

            await userManager.CreateAsync(testUser3, "ProHunter123!");
        }
    }

    private static async Task SeedTestApplicationsAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        var testUser = await userManager.FindByEmailAsync("hunter@test.com");
        if (testUser == null) return;

        // Check if applications already exist for this user
        if (await context.Applications.AnyAsync(a => a.UserId == testUser.Id))
        {
            return; // Applications already seeded
        }

        var applications = new List<Domain.Entities.Application>
        {
            // Application 1: Careington - Recently Applied
            new Domain.Entities.Application
            {
                Id = Guid.NewGuid(),
                UserId = testUser.Id,
                CompanyName = "Careington International Corporation",
                RoleTitle = "Software Developer",
                SourceName = "indeed.com",
                SourceUrl = "https://www.indeed.com/viewjob?jk=f466bbf7d3abcc8f",
                Status = ApplicationStatus.Applied,
                WorkMode = WorkMode.Hybrid,
                Location = "7400 Gaylord Pkwy, Frisco, TX 75034",
                JobDescription = "Careington International seeks a Mid Level Software Developer to develop software solutions using C#, .NET Core, and cloud architecture knowledge. The role involves translating business needs into technical requirements, defining development practices, and improving operations through systems analysis and innovation across multiple teams.",
                RequiredSkills = new List<string> { "C#", ".NET (version 6 or higher)", "ASP.NET", "SQL Server", "Object-oriented programming", "Design patterns", "Agile development", "SCRUM", "Test case design", "MVC", "JavaScript" },
                NiceToHaveSkills = new List<string> { "TDD (Test-Driven Development)", "HIPAA experience", "PCI experience", ".NET Framework", "JQuery", "Microservices", "Angular", "Microsoft Azure", "Azure DevOps", "CI/CD" },
                ParsedByAI = true,
                AiParsingStatus = AiParsingStatus.Success,
                Points = 1,
                CreatedDate = DateTime.UtcNow.AddDays(-2),
                UpdatedDate = DateTime.UtcNow.AddDays(-2),
                TimelineEvents = new List<TimelineEvent>
                {
                    new TimelineEvent
                    {
                        Id = Guid.NewGuid(),
                        EventType = EventType.Applied,
                        Notes = "Application submitted through Indeed",
                        Timestamp = DateTime.UtcNow.AddDays(-2),
                        Points = 1,
                        CreatedDate = DateTime.UtcNow.AddDays(-2)
                    }
                }
            },

            // Application 2: Fleetsoft - Screening Stage
            new Domain.Entities.Application
            {
                Id = Guid.NewGuid(),
                UserId = testUser.Id,
                CompanyName = "Fleetsoft",
                RoleTitle = "Full Stack Developer",
                SourceName = "indeed.com",
                SourceUrl = "https://www.indeed.com/viewjob?jk=520ff90215d8d0e9",
                Status = ApplicationStatus.Screening,
                WorkMode = WorkMode.Onsite,
                Location = "Carrollton, TX 75010",
                SalaryMin = 75000,
                SalaryMax = 95000,
                JobDescription = "Fleetsoft is seeking a Developer/Technical Support person to develop and provide technical support for software that helps fleets run operations more efficiently and track costs. The role involves building and maintaining install packages, developing software utilizing Microsoft SQL Server, troubleshooting issues, and providing quality technical support to customers.",
                RequiredSkills = new List<string> { "C++", "Visual Studio", "SQL", "Microsoft SQL Server", "Event driven programming", "Object oriented programming", "Data structures", "Customer service" },
                NiceToHaveSkills = new List<string> { ".Net", "Clarion", "C#", "Java", "MS Access", "Mobile application development (iOS and Android)" },
                ParsedByAI = true,
                AiParsingStatus = AiParsingStatus.Success,
                Points = 3,
                CreatedDate = DateTime.UtcNow.AddDays(-7),
                UpdatedDate = DateTime.UtcNow.AddDays(-4),
                TimelineEvents = new List<TimelineEvent>
                {
                    new TimelineEvent
                    {
                        Id = Guid.NewGuid(),
                        EventType = EventType.Applied,
                        Notes = "Application submitted",
                        Timestamp = DateTime.UtcNow.AddDays(-7),
                        Points = 1,
                        CreatedDate = DateTime.UtcNow.AddDays(-7)
                    },
                    new TimelineEvent
                    {
                        Id = Guid.NewGuid(),
                        EventType = EventType.Screening,
                        Notes = "Recruiter reached out to schedule phone screen",
                        Timestamp = DateTime.UtcNow.AddDays(-4),
                        Points = 2,
                        CreatedDate = DateTime.UtcNow.AddDays(-4)
                    }
                }
            },

            // Application 3: Goldman Sachs - Interview Stage
            new Domain.Entities.Application
            {
                Id = Guid.NewGuid(),
                UserId = testUser.Id,
                CompanyName = "Goldman Sachs",
                RoleTitle = "UI Developer - Analyst",
                SourceName = "indeed.com",
                SourceUrl = "https://www.indeed.com/viewjob?jk=b037a88709bcd9c6",
                Status = ApplicationStatus.Interview,
                WorkMode = WorkMode.Onsite,
                Location = "Dallas, TX",
                JobDescription = "Design and develop scalable UI applications using React for the Private Wealth Management Asset Transfers Engineering Team. Collaborate with cross-functional teams to implement new features and support existing software platforms.",
                RequiredSkills = new List<string> { "React JS", "HTML5", "JavaScript", "TypeScript", "Microservice architectures", "RESTful APIs", "Cross-functional communication" },
                NiceToHaveSkills = new List<string> { "Analytical mindset", "Multi-tasking", "Technical problem-solving" },
                ParsedByAI = true,
                AiParsingStatus = AiParsingStatus.Success,
                Points = 8,
                CreatedDate = DateTime.UtcNow.AddDays(-14),
                UpdatedDate = DateTime.UtcNow.AddDays(-3),
                TimelineEvents = new List<TimelineEvent>
                {
                    new TimelineEvent
                    {
                        Id = Guid.NewGuid(),
                        EventType = EventType.Applied,
                        Notes = "Application submitted",
                        Timestamp = DateTime.UtcNow.AddDays(-14),
                        Points = 1,
                        CreatedDate = DateTime.UtcNow.AddDays(-14)
                    },
                    new TimelineEvent
                    {
                        Id = Guid.NewGuid(),
                        EventType = EventType.Screening,
                        Notes = "Initial phone screen with recruiter",
                        Timestamp = DateTime.UtcNow.AddDays(-10),
                        Points = 2,
                        CreatedDate = DateTime.UtcNow.AddDays(-10)
                    },
                    new TimelineEvent
                    {
                        Id = Guid.NewGuid(),
                        EventType = EventType.Interview,
                        Notes = "First round technical interview completed",
                        Timestamp = DateTime.UtcNow.AddDays(-3),
                        Points = 5,
                        CreatedDate = DateTime.UtcNow.AddDays(-3)
                    }
                }
            },

            // Application 4: PY DATA - Rejected
            new Domain.Entities.Application
            {
                Id = Guid.NewGuid(),
                UserId = testUser.Id,
                CompanyName = "PY DATA, INC.",
                RoleTitle = ".NET Developer",
                SourceName = "dice.com",
                SourceUrl = "https://www.dice.com/job-detail/a1405126-c968-45e1-9f96-67d54ca17f21",
                Status = ApplicationStatus.Rejected,
                WorkMode = WorkMode.Remote,
                Location = "Remote",
                SalaryMin = 55,
                SalaryMax = 60,
                JobDescription = "A 6-month extendable contract role for a .NET Developer to design, develop, implement, test, and maintain business and computer applications software with a focus on modernizing services, codebases, and infrastructure.",
                RequiredSkills = new List<string> { "C#", ".NET", "Cloud Services", "Infrastructure as Code", "ARM templates", "Bicep", "Terraform", "RESTful Web APIs", "Azure DevOps", "CI/CD", "Unit testing" },
                NiceToHaveSkills = new List<string> { "Enterprise systems experience", "Legacy system modernization", "Agile/Scrum", "Test tooling improvement", "Security best practices" },
                ParsedByAI = true,
                AiParsingStatus = AiParsingStatus.Success,
                Points = 5,
                CreatedDate = DateTime.UtcNow.AddDays(-20),
                UpdatedDate = DateTime.UtcNow.AddDays(-12),
                TimelineEvents = new List<TimelineEvent>
                {
                    new TimelineEvent
                    {
                        Id = Guid.NewGuid(),
                        EventType = EventType.Applied,
                        Notes = "Application submitted via Dice",
                        Timestamp = DateTime.UtcNow.AddDays(-20),
                        Points = 1,
                        CreatedDate = DateTime.UtcNow.AddDays(-20)
                    },
                    new TimelineEvent
                    {
                        Id = Guid.NewGuid(),
                        EventType = EventType.Screening,
                        Notes = "Phone screen with hiring manager",
                        Timestamp = DateTime.UtcNow.AddDays(-15),
                        Points = 2,
                        CreatedDate = DateTime.UtcNow.AddDays(-15)
                    },
                    new TimelineEvent
                    {
                        Id = Guid.NewGuid(),
                        EventType = EventType.Rejected,
                        Notes = "Decided to go with a candidate with more IaC experience",
                        Timestamp = DateTime.UtcNow.AddDays(-12),
                        Points = 2,
                        CreatedDate = DateTime.UtcNow.AddDays(-12)
                    }
                }
            },

            // Application 5: Robert Half - Recently Applied
            new Domain.Entities.Application
            {
                Id = Guid.NewGuid(),
                UserId = testUser.Id,
                CompanyName = "Robert Half",
                RoleTitle = "Software Engineer",
                SourceName = "linkedin.com",
                SourceUrl = "https://www.linkedin.com/jobs/view/4362590552",
                Status = ApplicationStatus.Applied,
                WorkMode = WorkMode.Hybrid,
                Location = "Fort Worth, TX",
                SalaryMin = 60,
                SalaryMax = 65,
                JobDescription = "We are seeking a highly skilled .NET Developer to design, develop, and maintain software applications. This role is ideal for a developer who excels at troubleshooting, integrating complex solutions, and working within distributed messaging environments.",
                RequiredSkills = new List<string> { "C#", ".NET", "MVC", "APIs", "SQL Server", "JavaScript", "HTML", "Git/TFS", "Windows OS", "Messaging Systems (Azure Service Bus, Apache Kafka, or RabbitMQ)" },
                NiceToHaveSkills = new List<string> { "Azure Service Bus", "Apache Kafka", "RabbitMQ" },
                ParsedByAI = true,
                AiParsingStatus = AiParsingStatus.Success,
                Points = 1,
                CreatedDate = DateTime.UtcNow.AddDays(-5),
                UpdatedDate = DateTime.UtcNow.AddDays(-5),
                TimelineEvents = new List<TimelineEvent>
                {
                    new TimelineEvent
                    {
                        Id = Guid.NewGuid(),
                        EventType = EventType.Applied,
                        Notes = "Application submitted through LinkedIn",
                        Timestamp = DateTime.UtcNow.AddDays(-5),
                        Points = 1,
                        CreatedDate = DateTime.UtcNow.AddDays(-5)
                    }
                }
            }
        };

        await context.Applications.AddRangeAsync(applications);
        await context.SaveChangesAsync();
    }
}
