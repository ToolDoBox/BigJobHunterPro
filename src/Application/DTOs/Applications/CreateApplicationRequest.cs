using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Applications;

public class CreateApplicationRequest
{
    [MaxLength(500, ErrorMessage = "Source URL cannot exceed 500 characters")]
    [Url(ErrorMessage = "Source URL must be a valid URL")]
    public string? SourceUrl { get; set; }

    [Required(ErrorMessage = "Job page content is required")]
    public string RawPageContent { get; set; } = string.Empty;
}
