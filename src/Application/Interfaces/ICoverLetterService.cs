using Application.DTOs.CoverLetter;

namespace Application.Interfaces;

public interface ICoverLetterService
{
    /// <summary>
    /// Generates a cover letter for the specified application using AI.
    /// </summary>
    /// <param name="applicationId">The application ID</param>
    /// <param name="resumeText">Optional resume text (if not provided, uses user's saved resume)</param>
    /// <returns>Result containing the generated cover letter or error</returns>
    Task<CoverLetterResult> GenerateCoverLetterAsync(Guid applicationId, string? resumeText = null);

    /// <summary>
    /// Saves or updates the cover letter for the specified application.
    /// </summary>
    /// <param name="applicationId">The application ID</param>
    /// <param name="coverLetterHtml">The cover letter HTML content</param>
    /// <returns>Result indicating success or failure</returns>
    Task<CoverLetterResult> SaveCoverLetterAsync(Guid applicationId, string coverLetterHtml);
}
