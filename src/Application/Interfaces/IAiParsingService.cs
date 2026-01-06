using Application.DTOs.AiParsing;

namespace Application.Interfaces;

public interface IAiParsingService
{
    /// <summary>
    /// Parses raw job posting content using Claude AI and extracts structured fields.
    /// </summary>
    /// <param name="rawPageContent">The raw text content from a job posting page.</param>
    /// <returns>Parsing result with extracted fields or error information.</returns>
    Task<AiParsingResult> ParseJobPostingAsync(string rawPageContent);
}
