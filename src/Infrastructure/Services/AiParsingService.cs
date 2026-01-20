using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
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
    private readonly string _apiKeyState;

    public AiParsingService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<AiParsingService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _model = configuration["AnthropicSettings:Model"] ?? "claude-haiku-4-5-20251016";
        _maxTokens = int.TryParse(configuration["AnthropicSettings:MaxTokens"], out var tokens) ? tokens : 1024;
        var apiKey = configuration["AnthropicSettings:ApiKey"]
            ?? configuration["AnthropicApiKey"]
            ?? string.Empty;
        _isApiKeyConfigured = !string.IsNullOrWhiteSpace(apiKey)
            && !apiKey.Contains("PLACEHOLDER", StringComparison.OrdinalIgnoreCase)
            && !apiKey.Contains("LOADED FROM AZURE KEY VAULT", StringComparison.OrdinalIgnoreCase);
        _apiKeyState = GetApiKeyState(apiKey, _isApiKeyConfigured);

        if (!_isApiKeyConfigured)
        {
            _logger.LogWarning("Anthropic API key is not configured. State={KeyState}. AI parsing will be skipped.",
                _apiKeyState);
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
            _logger.LogWarning("Anthropic API key not configured. State={KeyState}.", _apiKeyState);
            return AiParsingResult.Failed("Anthropic API key not configured");
        }

        try
        {
            var client = _httpClientFactory.CreateClient("Anthropic");
            var prompt = BuildPrompt(rawPageContent);
            var requestBody = BuildRequestBody(prompt);

            _logger.LogInformation(
                "Calling Anthropic API to parse job posting (Model={Model}, MaxTokens={MaxTokens}, TimeoutSeconds={TimeoutSeconds}, ContentLength={ContentLength}, KeyState={KeyState})",
                _model, _maxTokens, client.Timeout.TotalSeconds, rawPageContent.Length, _apiKeyState);

            var response = await client.PostAsJsonAsync("messages", requestBody);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var requestId = TryGetHeader(response.Headers, "x-request-id")
                    ?? TryGetHeader(response.Headers, "request-id");
                var rateLimit = BuildRateLimitSummary(response);
                var contentType = response.Content.Headers.ContentType?.ToString() ?? "unknown";

                _logger.LogWarning(
                    "Anthropic API error. Status={StatusCode}, RequestId={RequestId}, ContentType={ContentType}, RateLimit={RateLimit}, Error={Error}",
                    response.StatusCode,
                    requestId ?? "n/a",
                    contentType,
                    rateLimit,
                    TrimForLog(errorContent, 2000));
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

    private static string GetApiKeyState(string apiKey, bool isConfigured)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return "missing";
        }

        if (!isConfigured)
        {
            return "placeholder";
        }

        return $"fingerprint:{ComputeFingerprint(apiKey)}";
    }

    private static string ComputeFingerprint(string apiKey)
    {
        var bytes = Encoding.UTF8.GetBytes(apiKey);
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(bytes);
        return Convert.ToHexString(hash[..4]).ToLowerInvariant();
    }

    private static string? TryGetHeader(System.Net.Http.Headers.HttpResponseHeaders headers, string name)
    {
        return headers.TryGetValues(name, out var values) ? values.FirstOrDefault() : null;
    }

    private static string BuildRateLimitSummary(HttpResponseMessage response)
    {
        var headers = response.Headers;
        var requestsLimit = TryGetHeader(headers, "anthropic-ratelimit-limit-requests") ?? "n/a";
        var requestsRemaining = TryGetHeader(headers, "anthropic-ratelimit-remaining-requests") ?? "n/a";
        var requestsReset = TryGetHeader(headers, "anthropic-ratelimit-reset-requests") ?? "n/a";
        var tokensLimit = TryGetHeader(headers, "anthropic-ratelimit-limit-tokens") ?? "n/a";
        var tokensRemaining = TryGetHeader(headers, "anthropic-ratelimit-remaining-tokens") ?? "n/a";
        var tokensReset = TryGetHeader(headers, "anthropic-ratelimit-reset-tokens") ?? "n/a";

        return $"requests {requestsRemaining}/{requestsLimit} reset={requestsReset}; tokens {tokensRemaining}/{tokensLimit} reset={tokensReset}";
    }

    private static string TrimForLog(string value, int maxChars)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "<empty>";
        }

        var trimmed = value.Trim();
        return trimmed.Length <= maxChars ? trimmed : $"{trimmed[..maxChars]}...[truncated]";
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
