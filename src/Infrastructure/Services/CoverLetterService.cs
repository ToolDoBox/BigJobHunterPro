using System.Net.Http.Json;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application.DTOs.CoverLetter;
using Application.Interfaces;
using Application.Interfaces.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class CoverLetterService : ICoverLetterService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<CoverLetterService> _logger;
    private readonly string _model;
    private readonly int _maxTokens;
    private readonly bool _isApiKeyConfigured;
    private readonly string _promptTemplate;

    public CoverLetterService(
        IHttpClientFactory httpClientFactory,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IConfiguration configuration,
        ILogger<CoverLetterService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _logger = logger;
        _model = configuration["AnthropicSettings:Model"] ?? "claude-haiku-4-5";
        _maxTokens = 2048; // Cover letters need more tokens than parsing

        var apiKey = configuration["AnthropicSettings:ApiKey"]
            ?? configuration["AnthropicApiKey"]
            ?? string.Empty;
        _isApiKeyConfigured = !string.IsNullOrWhiteSpace(apiKey)
            && !apiKey.Contains("PLACEHOLDER", StringComparison.OrdinalIgnoreCase)
            && !apiKey.Contains("LOADED FROM AZURE KEY VAULT", StringComparison.OrdinalIgnoreCase);

        _promptTemplate = LoadPromptTemplate();
    }

    public async Task<CoverLetterResult> GenerateCoverLetterAsync(Guid applicationId, string? resumeText = null)
    {
        var userId = _currentUser.GetUserId()
            ?? throw new UnauthorizedAccessException("User not authenticated");

        if (!_isApiKeyConfigured)
        {
            _logger.LogWarning("Anthropic API key not configured for cover letter generation");
            return CoverLetterResult.Failed("AI service is not configured");
        }

        // Get the application
        var application = await _unitOfWork.Applications.GetByIdAsync(applicationId);
        if (application == null || application.UserId != userId)
        {
            return CoverLetterResult.Failed("Application not found");
        }

        // Get resume text - use provided text or fall back to user's saved resume
        var finalResumeText = resumeText;
        if (string.IsNullOrWhiteSpace(finalResumeText))
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null || string.IsNullOrWhiteSpace(user.ResumeText))
            {
                return CoverLetterResult.Failed("No resume provided. Please paste your resume or save one in your profile.");
            }
            finalResumeText = user.ResumeText;
        }

        // Build job description text from application data
        var jobDescriptionText = BuildJobDescriptionText(application);
        if (string.IsNullOrWhiteSpace(jobDescriptionText))
        {
            return CoverLetterResult.Failed("Application has no job description or company/role information.");
        }

        try
        {
            var coverLetterHtml = await CallAnthropicApiAsync(finalResumeText, jobDescriptionText);
            if (string.IsNullOrWhiteSpace(coverLetterHtml))
            {
                return CoverLetterResult.Failed("Failed to generate cover letter");
            }

            // Save the generated cover letter
            var generatedAt = DateTime.UtcNow;
            application.CoverLetterHtml = coverLetterHtml;
            application.CoverLetterGeneratedAt = generatedAt;
            application.UpdatedDate = generatedAt;
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Generated cover letter for application {ApplicationId}", applicationId);
            return CoverLetterResult.Succeeded(coverLetterHtml, generatedAt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating cover letter for application {ApplicationId}", applicationId);
            return CoverLetterResult.Failed($"Error generating cover letter: {ex.Message}");
        }
    }

    public async Task<CoverLetterResult> SaveCoverLetterAsync(Guid applicationId, string coverLetterHtml)
    {
        var userId = _currentUser.GetUserId()
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var application = await _unitOfWork.Applications.GetByIdAsync(applicationId);
        if (application == null || application.UserId != userId)
        {
            return CoverLetterResult.Failed("Application not found");
        }

        var now = DateTime.UtcNow;
        application.CoverLetterHtml = coverLetterHtml;
        application.CoverLetterGeneratedAt = now;
        application.UpdatedDate = now;
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Saved cover letter for application {ApplicationId}", applicationId);
        return CoverLetterResult.Succeeded(coverLetterHtml, now);
    }

    private static string LoadPromptTemplate()
    {
        // Try to load from file in the Infrastructure/Prompts directory
        var assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var possiblePaths = new[]
        {
            Path.Combine(assemblyDir ?? "", "Prompts", "cover-letter-prompt.txt"),
            Path.Combine(AppContext.BaseDirectory, "Prompts", "cover-letter-prompt.txt"),
            Path.Combine(Directory.GetCurrentDirectory(), "src", "Infrastructure", "Prompts", "cover-letter-prompt.txt"),
        };

        foreach (var path in possiblePaths)
        {
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }
        }

        // Fallback to embedded prompt
        return GetDefaultPromptTemplate();
    }

    private static string GetDefaultPromptTemplate()
    {
        return """
            You are an expert cover-letter writer and meticulous fact-checker.

            You ONLY have two inputs:
            (1) RESUME_TEXT (the candidate's resume)
            (2) JOB_DESCRIPTION_TEXT (the job posting)

            You must NOT invent, assume, or add facts not explicitly supported by RESUME_TEXT or JOB_DESCRIPTION_TEXT.

            GOAL
            Generate a one-page, professional cover letter in clean, print-ready HTML that can be converted to a PDF. The tone should sound human—confident, direct, and tailored—without being overly long or "template-y."

            OUTPUT REQUIREMENTS (HARD)
            - Output MUST be valid HTML only (no markdown, no backticks, no commentary).
            - Single page when rendered to US Letter (8.5x11) with 1in margins.
            - Use simple, PDF-friendly HTML + inline CSS in a <style> block.
            - Include @page { size: letter; margin: 1in; } in the CSS.
            - Use a single wrapper <div class="page"> with a max content width around 6.5in and a centered layout.
            - Use fonts that render well everywhere (system fonts).
            - No external assets, no links required (only include URLs if present in the resume).
            - No tables unless needed for alignment; prefer simple <div>/<p> structure.
            - 3–4 short paragraphs in the letter body (plus header + closing).
            - Target length: ~250–400 words. Absolute max: 420 words.
            - No bullet lists (ATS-safe and clean print), unless the job explicitly asks for them (rare).

            TRUTHFULNESS + EVIDENCE (HARD)
            - Every accomplishment, metric, tool, title, employer, certification, and claim must be supported by RESUME_TEXT.
            - Company/role details (company name, role title) must come from JOB_DESCRIPTION_TEXT.
            - If the job asks for a skill not in RESUME_TEXT, do NOT claim it.
            - Avoid empty adjectives and clichés.

            NOW GENERATE THE REAL OUTPUT
            Inputs:
            [RESUME_TEXT]
            {{RESUME_TEXT}}

            [JOB_DESCRIPTION_TEXT]
            {{JOB_DESCRIPTION_TEXT}}

            Return ONLY the final HTML cover letter.
            """;
    }

    private static string BuildJobDescriptionText(Domain.Entities.Application application)
    {
        var parts = new List<string>();

        if (!string.IsNullOrWhiteSpace(application.CompanyName))
        {
            parts.Add($"Company: {application.CompanyName}");
        }

        if (!string.IsNullOrWhiteSpace(application.RoleTitle))
        {
            parts.Add($"Role: {application.RoleTitle}");
        }

        if (!string.IsNullOrWhiteSpace(application.Location))
        {
            parts.Add($"Location: {application.Location}");
        }

        if (application.WorkMode.HasValue)
        {
            parts.Add($"Work Mode: {application.WorkMode}");
        }

        if (!string.IsNullOrWhiteSpace(application.JobDescription))
        {
            parts.Add($"\nJob Description:\n{application.JobDescription}");
        }

        if (application.RequiredSkills.Any())
        {
            parts.Add($"\nRequired Skills: {string.Join(", ", application.RequiredSkills)}");
        }

        if (application.NiceToHaveSkills.Any())
        {
            parts.Add($"\nNice to Have Skills: {string.Join(", ", application.NiceToHaveSkills)}");
        }

        return string.Join("\n", parts);
    }

    private async Task<string?> CallAnthropicApiAsync(string resumeText, string jobDescriptionText)
    {
        var client = _httpClientFactory.CreateClient("Anthropic");

        // Build the prompt by replacing placeholders
        var prompt = _promptTemplate
            .Replace("{{RESUME_TEXT}}", resumeText)
            .Replace("{{JOB_DESCRIPTION_TEXT}}", jobDescriptionText);

        var requestBody = new
        {
            model = _model,
            max_tokens = _maxTokens,
            messages = new object[]
            {
                new { role = "user", content = prompt }
            }
        };

        _logger.LogInformation(
            "Calling Anthropic API to generate cover letter (Model={Model}, MaxTokens={MaxTokens})",
            _model, _maxTokens);

        var response = await client.PostAsJsonAsync("messages", requestBody);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Anthropic API error: {StatusCode} - {Error}",
                response.StatusCode, errorContent.Length > 500 ? errorContent[..500] : errorContent);
            throw new Exception($"API error: {response.StatusCode}");
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        return ParseCoverLetterResponse(responseContent);
    }

    private string? ParseCoverLetterResponse(string responseContent)
    {
        try
        {
            var response = JsonSerializer.Deserialize<AnthropicResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var textContent = response?.Content?.FirstOrDefault(c => c.Type == "text")?.Text;

            if (string.IsNullOrEmpty(textContent))
            {
                _logger.LogWarning("Empty text content in API response");
                return null;
            }

            // The response should be raw HTML - clean up any potential markdown code fences
            var html = textContent.Trim();
            if (html.StartsWith("```html", StringComparison.OrdinalIgnoreCase))
            {
                html = html[7..]; // Remove ```html
            }
            else if (html.StartsWith("```"))
            {
                html = html[3..]; // Remove ```
            }

            if (html.EndsWith("```"))
            {
                html = html[..^3]; // Remove trailing ```
            }

            return html.Trim();
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to parse AI response as JSON");
            return null;
        }
    }

    private class AnthropicResponse
    {
        [JsonPropertyName("content")]
        public List<AnthropicContent>? Content { get; set; }
    }

    private class AnthropicContent
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }
}
