using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.CoverLetter;

public class GenerateCoverLetterRequest
{
    /// <summary>
    /// Optional resume text. If not provided, the user's saved resume will be used.
    /// </summary>
    [MaxLength(50000)]
    public string? ResumeText { get; set; }
}
