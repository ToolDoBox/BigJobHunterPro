using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Profile;

public class UpdateResumeHtmlRequest
{
    [MaxLength(200000, ErrorMessage = "Resume HTML cannot exceed 200,000 characters")]
    public string? ResumeHtml { get; set; }
}
