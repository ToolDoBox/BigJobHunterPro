using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application.DTOs.AiParsing;
using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class AiParsingService : IAiParsingService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AiParsingService> _logger;
    private readonly string _model;
    private readonly int _maxTokens;
    private readonly bool _isApiKeyConfigured;

    public AiParsingService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<AiParsingService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _model = configuration["AnthropicSettings:Model"] ?? "claude-haiku-4-5";
        _maxTokens = int.TryParse(configuration["AnthropicSettings:MaxTokens"], out var tokens) ? tokens : 1024;
        var apiKey = configuration["AnthropicSettings:ApiKey"] ?? string.Empty;
        _isApiKeyConfigured = !string.IsNullOrWhiteSpace(apiKey)
            && !apiKey.Contains("PLACEHOLDER", StringComparison.OrdinalIgnoreCase)
            && !apiKey.Contains("LOADED FROM AZURE KEY VAULT", StringComparison.OrdinalIgnoreCase);

        if (!_isApiKeyConfigured)
        {
            _logger.LogWarning("Anthropic API key is not configured. AI parsing will be skipped.");
        }
    }

    public async Task<AiParsingResult> ParseJobPostingAsync(string rawPageContent)
    {
        if (string.IsNullOrWhiteSpace(rawPageContent))
        {
            return AiParsingResult.Failed("No content to parse");
        }

        if (!_isApiKeyConfigured)
        {
            return AiParsingResult.Failed("Anthropic API key not configured");
        }

        try
        {
            var client = _httpClientFactory.CreateClient("Anthropic");
            var prompt = BuildPrompt(rawPageContent);
            var requestBody = BuildRequestBody(prompt);

            _logger.LogInformation("Calling Anthropic API to parse job posting ({ContentLength} chars)",
                rawPageContent.Length);

            var response = await client.PostAsJsonAsync("messages", requestBody);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Anthropic API returned {StatusCode}: {Error}",
                    response.StatusCode, errorContent);
                return AiParsingResult.Failed($"API error: {response.StatusCode}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            return ParseApiResponse(responseContent);
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            _logger.LogWarning(ex, "AI parsing request timed out");
            return AiParsingResult.Failed("Request timed out");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Network error during AI parsing");
            return AiParsingResult.Failed($"Network error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during AI parsing");
            return AiParsingResult.Failed($"Unexpected error: {ex.Message}");
        }
    }

    private static string BuildPrompt(string rawPageContent)
    {
        // Truncate extremely long content to avoid token limits
        var truncatedContent = rawPageContent.Length > 15000
            ? rawPageContent[..15000] + "\n... [content truncated]"
            : rawPageContent;

        return $$"""
            You are a job posting parser. Extract structured information from the job posting content below.

            IMPORTANT RULES:
            1. Extract ONLY information explicitly stated in the posting
            2. For workMode, use EXACTLY one of: "Remote", "Hybrid", "Onsite", or null if unclear
            3. For salary, extract the numeric values only (no currency symbols). Keep as-is (e.g., 18 for $18/hour, not 18000)
            4. For skills, separate into "requiredSkills" (must-have, mandatory, required) and "niceToHaveSkills" (preferred, bonus, nice to have)
            5. Keep jobDescription to 2-3 sentences summarizing the role
            6. If a field cannot be determined from the posting, use null (or empty array for skills)

            Respond with ONLY valid JSON in this exact format (no markdown, no explanation):
            {
              "companyName": string | null,
              "roleTitle": string | null,
              "workMode": "Remote" | "Hybrid" | "Onsite" | null,
              "location": string | null,
              "salaryMin": number | null,
              "salaryMax": number | null,
              "jobDescription": string | null,
              "requiredSkills": string[],
              "niceToHaveSkills": string[]
            }

            JOB POSTING CONTENT:
            {{truncatedContent}}
            """;
    }

    private object BuildRequestBody(string prompt)
    {
        return new
        {
            model = _model,
            max_tokens = _maxTokens,
            messages = new object[]
            {
                new { role = "user", content = prompt },
                new { role = "assistant", content = "{" }  // Prefill technique for guaranteed JSON
            }
        };
    }

    private AiParsingResult ParseApiResponse(string responseContent)
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
                return AiParsingResult.Failed("Empty response from API");
            }

            // Prepend the "{" since we used prefill
            var jsonText = "{" + textContent;

            // Clean up any trailing content after the JSON
            var lastBrace = jsonText.LastIndexOf('}');
            if (lastBrace > 0)
            {
                jsonText = jsonText[..(lastBrace + 1)];
            }

            var parsed = JsonSerializer.Deserialize<ParsedJobPosting>(jsonText, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (parsed == null)
            {
                _logger.LogWarning("Failed to deserialize parsed job posting");
                return AiParsingResult.Failed("Failed to parse JSON response");
            }

            _logger.LogInformation("Successfully parsed job posting: {Company} - {Role}",
                parsed.CompanyName, parsed.RoleTitle);

            return new AiParsingResult
            {
                Success = true,
                CompanyName = parsed.CompanyName,
                RoleTitle = parsed.RoleTitle,
                WorkMode = parsed.WorkMode,
                Location = parsed.Location,
                SalaryMin = parsed.SalaryMin,
                SalaryMax = parsed.SalaryMax,
                JobDescription = parsed.JobDescription,
                RequiredSkills = parsed.RequiredSkills ?? new List<string>(),
                NiceToHaveSkills = parsed.NiceToHaveSkills ?? new List<string>()
            };
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to parse AI response as JSON: {Response}",
                responseContent.Length > 500 ? responseContent[..500] : responseContent);
            return AiParsingResult.Failed("Invalid JSON in API response");
        }
    }

    // Internal classes for Anthropic API response deserialization
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

    private class ParsedJobPosting
    {
        public string? CompanyName { get; set; }
        public string? RoleTitle { get; set; }
        public string? WorkMode { get; set; }
        public string? Location { get; set; }
        public int? SalaryMin { get; set; }
        public int? SalaryMax { get; set; }
        public string? JobDescription { get; set; }
        public List<string>? RequiredSkills { get; set; }
        public List<string>? NiceToHaveSkills { get; set; }
    }
}
