using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.CoverLetter;

public class SaveCoverLetterRequest
{
    /// <summary>
    /// The cover letter HTML content.
    /// </summary>
    [Required]
    [MaxLength(15000)]
    public string CoverLetterHtml { get; set; } = string.Empty;
}
