namespace Application.DTOs.CoverLetter;

public class CoverLetterResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string? CoverLetterHtml { get; set; }
    public DateTime? GeneratedAt { get; set; }

    public static CoverLetterResult Succeeded(string coverLetterHtml, DateTime generatedAt)
    {
        return new CoverLetterResult
        {
            Success = true,
            CoverLetterHtml = coverLetterHtml,
            GeneratedAt = generatedAt
        };
    }

    public static CoverLetterResult Failed(string errorMessage)
    {
        return new CoverLetterResult
        {
            Success = false,
            ErrorMessage = errorMessage
        };
    }
}
