using System.Text;
using Application.DTOs.Auth;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebAPI.Hubs;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add Database Context - Use SQLite for development if SQL Server not available
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var useSqlite = builder.Environment.IsDevelopment() &&
    (string.IsNullOrEmpty(connectionString) || connectionString.Contains("PLACEHOLDER") || connectionString.Contains("USER SECRETS"));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (useSqlite)
    {
        var dbPath = Path.Combine(Environment.CurrentDirectory, "bigjobhunterpro.db");
        options.UseSqlite($"Data Source={dbPath}");
    }
    else
    {
        options.UseSqlServer(connectionString, sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        });
    }
});

// Add Identity with Password Policy
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;

    // User settings
    options.User.RequireUniqueEmail = true;

    // Sign in settings
    options.SignIn.RequireConfirmedEmail = false;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Add JWT Authentication
var jwtSecret = builder.Configuration["JwtSettings:Secret"]
    ?? throw new InvalidOperationException("JWT Secret not configured");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        NameClaimType = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub
    };

    // Configure SignalR to read JWT from query string
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// Add HTTP Context Accessor for CurrentUserService
builder.Services.AddHttpContextAccessor();

// Add rate limiting
builder.Services.AddRateLimiter(options =>
{
    var isTestEnvironment = builder.Environment.IsEnvironment("Test");
    var permitLimit = isTestEnvironment
        ? int.MaxValue
        : int.TryParse(builder.Configuration["RateLimiting:QuickCapture:PermitLimit"], out var parsedPermit)
            ? parsedPermit
            : 10;
    var windowSeconds = isTestEnvironment
        ? 1
        : int.TryParse(builder.Configuration["RateLimiting:QuickCapture:WindowSeconds"], out var parsedWindow)
            ? parsedWindow
            : 60;
    var queueLimit = isTestEnvironment
        ? 0
        : int.TryParse(builder.Configuration["RateLimiting:QuickCapture:QueueLimit"], out var parsedQueue)
            ? parsedQueue
            : 0;

    options.AddPolicy("QuickCapture", context =>
    {
        var userId = context.User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
        var partitionKey = string.IsNullOrWhiteSpace(userId)
            ? context.Connection.RemoteIpAddress?.ToString() ?? "anonymous"
            : userId;

        return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = permitLimit,
            Window = TimeSpan.FromSeconds(windowSeconds),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = queueLimit
        });
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsync(
            "{\"error\":\"Rate limit exceeded. Please wait before creating more applications.\"}",
            token);
    };
});

// Add Application Insights telemetry
builder.Services.AddApplicationInsightsTelemetry();

// Add Application Services
builder.Services.AddScoped<Application.Interfaces.IJwtTokenService, Infrastructure.Services.JwtTokenService>();
builder.Services.AddScoped<Application.Interfaces.ICurrentUserService, Infrastructure.Services.CurrentUserService>();
builder.Services.AddScoped<Application.Interfaces.IHealthCheckService, Infrastructure.Services.HealthCheckService>();

// Add after other service registrations
builder.Services.AddScoped<Application.Interfaces.IPointsService, Infrastructure.Services.PointsService>();
builder.Services.AddScoped<Application.Interfaces.IApplicationService, Infrastructure.Services.ApplicationService>();
builder.Services.AddScoped<Application.Interfaces.ITimelineEventService, Infrastructure.Services.TimelineEventService>();
builder.Services.AddScoped<Application.Interfaces.IActivityEventService, Infrastructure.Services.ActivityEventService>();

// Add HttpClient for Anthropic API
builder.Services.AddHttpClient("Anthropic", client =>
{
    var apiKey = builder.Configuration["AnthropicSettings:ApiKey"]
        ?? builder.Configuration["AnthropicApiKey"]
        ?? throw new InvalidOperationException("Anthropic API key not configured");

    client.BaseAddress = new Uri("https://api.anthropic.com/v1/");
    client.DefaultRequestHeaders.Add("x-api-key", apiKey);
    client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");

    var timeoutSeconds = int.TryParse(builder.Configuration["AnthropicSettings:TimeoutSeconds"], out var seconds)
        ? seconds
        : 30;
    client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
});

// Add AI Parsing Services
builder.Services.AddScoped<Application.Interfaces.IAiParsingService, Infrastructure.Services.AiParsingService>();
builder.Services.AddHostedService<Infrastructure.Services.AiParsingBackgroundService>();

// Add Hunting Party Services
builder.Services.AddScoped<Application.Interfaces.IHuntingPartyService, Infrastructure.Services.HuntingPartyService>();
builder.Services.AddScoped<Application.Interfaces.ILeaderboardNotifier, WebAPI.Services.LeaderboardNotifier>();
builder.Services.AddScoped<Application.Interfaces.IActivityNotifier, WebAPI.Services.ActivityNotifier>();

// Add SignalR
builder.Services.AddSignalR();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var configOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();

        string[] allowedOrigins;
        if (configOrigins != null && configOrigins.Length > 0)
        {
            allowedOrigins = configOrigins;
        }
        else if (builder.Environment.IsDevelopment())
        {
            allowedOrigins = new[] { "http://localhost:5173", "http://localhost:5174" };
        }
        else
        {
            // Production fallback - always allow these origins
            allowedOrigins = new[] {
                "https://orange-cliff-0e76fcd10.1.azurestaticapps.net",
                "https://bigjobhunter.pro",
                "https://www.bigjobhunter.pro"
            };
        }

        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Add Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState.Values
            .SelectMany(value => value.Errors)
            .Select(error => error.ErrorMessage)
            .ToList();

        return new BadRequestObjectResult(new ErrorResponse("Validation failed", errors));
    };
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.MapControllers();

// Map SignalR hub with CORS
app.MapHub<LeaderboardHub>("/hubs/leaderboard").RequireCors("AllowFrontend");
app.MapHub<ActivityHub>("/hubs/activity").RequireCors("AllowFrontend");

// Ensure database is created and seed data in development environment
if (app.Environment.IsDevelopment())
{
    // SeedData.InitializeAsync handles database creation internally
    await Infrastructure.Data.SeedData.InitializeAsync(app.Services);
}

app.Run();

// Make Program class accessible to integration tests
public partial class Program { }
