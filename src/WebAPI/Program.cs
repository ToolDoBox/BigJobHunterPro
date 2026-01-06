using System.Text;
using Application.DTOs.Auth;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
});

builder.Services.AddAuthorization();

// Add HTTP Context Accessor for CurrentUserService
builder.Services.AddHttpContextAccessor();

// Add Application Insights telemetry
builder.Services.AddApplicationInsightsTelemetry();

// Add Application Services
builder.Services.AddScoped<Application.Interfaces.IJwtTokenService, Infrastructure.Services.JwtTokenService>();
builder.Services.AddScoped<Application.Interfaces.ICurrentUserService, Infrastructure.Services.CurrentUserService>();

// Add after other service registrations
builder.Services.AddScoped<Application.Interfaces.IPointsService, Infrastructure.Services.PointsService>();
builder.Services.AddScoped<Application.Interfaces.IApplicationService, Infrastructure.Services.ApplicationService>();

// Add HttpClient for Anthropic API
builder.Services.AddHttpClient("Anthropic", client =>
{
    var apiKey = builder.Configuration["AnthropicSettings:ApiKey"]
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

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins")
            .Get<string[]>()
            ?? new[] { "http://localhost:5173", "http://localhost:5174" };

        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Add Controllers
builder.Services.AddControllers();

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

app.MapControllers();

// Seed database in development environment
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        await Infrastructure.Data.SeedData.InitializeAsync(scope.ServiceProvider);
    }
}

app.Run();

// Make Program class accessible to integration tests
public partial class Program { }
