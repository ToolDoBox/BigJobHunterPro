using Application.Interfaces;
using BigJobHunterPro.Application.DTOs.Analytics;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Services;

/// <summary>
/// Service for calculating meta-analytics on user applications
/// Implements caching to reduce database load for expensive queries
/// </summary>
public class AnalyticsService : IAnalyticsService
{
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private const int CacheExpirationMinutes = 10;

    // Common English stopwords to filter out
    private static readonly HashSet<string> Stopwords = new(StringComparer.OrdinalIgnoreCase)
    {
        "the", "and", "or", "but", "in", "on", "at", "to", "for", "of", "with", "a", "an",
        "is", "are", "was", "were", "be", "been", "being", "have", "has", "had", "do", "does",
        "did", "will", "would", "should", "could", "may", "might", "must", "can", "as", "by",
        "from", "this", "that", "these", "those", "it", "its", "if", "then", "than", "so",
        "we", "you", "your", "our", "their", "his", "her", "who", "what", "when", "where",
        "why", "how", "all", "each", "every", "both", "few", "more", "most", "other", "some",
        "such", "no", "not", "only", "own", "same", "than", "too", "very", "just"
    };

    public AnalyticsService(ApplicationDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<List<KeywordFrequency>> GetTopKeywordsAsync(string userId, int topCount = 20, CancellationToken cancellationToken = default)
    {
        // Generate cache key
        var cacheKey = $"keywords-{userId}-{topCount}";

        // Try to get from cache
        if (_cache.TryGetValue(cacheKey, out List<KeywordFrequency>? cachedKeywords) && cachedKeywords != null)
        {
            return cachedKeywords;
        }

        // Get successful applications (reached interview stage or offer)
        var successfulApplications = await _context.Applications
            .Include(a => a.TimelineEvents)
            .Where(a => a.UserId == userId)
            .ToListAsync(cancellationToken);

        // Filter to only those with interview or offer events
        var applicationsWithSuccess = successfulApplications
            .Where(a => a.TimelineEvents.Any(e => e.EventType == EventType.Interview || e.EventType == EventType.Offer))
            .ToList();

        if (applicationsWithSuccess.Count == 0)
        {
            return new List<KeywordFrequency>();
        }

        // Extract all text content from successful applications
        var allWords = new List<string>();

        foreach (var application in applicationsWithSuccess)
        {
            // Add words from RoleTitle
            if (!string.IsNullOrWhiteSpace(application.RoleTitle))
            {
                allWords.AddRange(ExtractWords(application.RoleTitle));
            }

            // Add words from JobDescription
            if (!string.IsNullOrWhiteSpace(application.JobDescription))
            {
                allWords.AddRange(ExtractWords(application.JobDescription));
            }

            // Add skills (already tokenized)
            allWords.AddRange(application.RequiredSkills);
            allWords.AddRange(application.NiceToHaveSkills);
        }

        // Count word frequencies
        var wordCounts = allWords
            .Where(word => !string.IsNullOrWhiteSpace(word))
            .Where(word => word.Length >= 3) // Ignore very short words
            .Where(word => !Stopwords.Contains(word))
            .GroupBy(word => word.ToLower())
            .Select(g => new { Word = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(topCount)
            .ToList();

        // Calculate percentages
        var keywords = wordCounts.Select(wc => new KeywordFrequency
        {
            Keyword = wc.Word,
            Frequency = wc.Count,
            Percentage = Math.Round((decimal)wc.Count / applicationsWithSuccess.Count * 100, 1)
        }).ToList();

        // Cache for 10 minutes
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheExpirationMinutes)
        };

        _cache.Set(cacheKey, keywords, cacheOptions);

        return keywords;
    }

    public async Task<List<ConversionBySource>> GetConversionBySourceAsync(string userId, CancellationToken cancellationToken = default)
    {
        // Generate cache key
        var cacheKey = $"conversion-by-source-{userId}";

        // Try to get from cache
        if (_cache.TryGetValue(cacheKey, out List<ConversionBySource>? cachedConversions) && cachedConversions != null)
        {
            return cachedConversions;
        }

        // Get all applications with their timeline events
        var applications = await _context.Applications
            .Include(a => a.TimelineEvents)
            .Where(a => a.UserId == userId)
            .ToListAsync(cancellationToken);

        if (applications.Count == 0)
        {
            return new List<ConversionBySource>();
        }

        // Group by source and calculate conversion rates
        var conversionData = applications
            .GroupBy(a => string.IsNullOrWhiteSpace(a.SourceName) ? "Unknown" : a.SourceName)
            .Select(g => new ConversionBySource
            {
                SourceName = g.Key,
                TotalApplications = g.Count(),
                InterviewCount = g.Count(a => a.TimelineEvents.Any(e => e.EventType == EventType.Interview || e.EventType == EventType.Offer)),
                ConversionRate = g.Count() == 0
                    ? 0
                    : Math.Round((decimal)g.Count(a => a.TimelineEvents.Any(e => e.EventType == EventType.Interview || e.EventType == EventType.Offer)) / g.Count() * 100, 1)
            })
            .OrderByDescending(c => c.ConversionRate)
            .ThenByDescending(c => c.TotalApplications)
            .ToList();

        // Cache for 10 minutes
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheExpirationMinutes)
        };

        _cache.Set(cacheKey, conversionData, cacheOptions);

        return conversionData;
    }

    /// <summary>
    /// Extracts individual words from text, splitting on whitespace and common delimiters
    /// </summary>
    private static List<string> ExtractWords(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return new List<string>();
        }

        // Split on whitespace, punctuation, and common delimiters
        var words = text.Split(new[] { ' ', '\t', '\n', '\r', ',', '.', ';', ':', '!', '?', '(', ')', '[', ']', '{', '}', '/', '\\', '-', '_', '+', '=', '*', '&', '%', '$', '#', '@' },
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return words
            .Where(w => !string.IsNullOrWhiteSpace(w))
            .Select(w => w.Trim())
            .ToList();
    }
}
