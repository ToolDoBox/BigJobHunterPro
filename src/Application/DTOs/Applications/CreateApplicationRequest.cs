using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Applications;

public class CreateApplicationRequest
{
    [Required(ErrorMessage = "Company name is required")]
    [MaxLength(200, ErrorMessage = "Company name cannot exceed 200 characters")]
    public string CompanyName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Role title is required")]
    [MaxLength(200, ErrorMessage = "Role title cannot exceed 200 characters")]
    public string RoleTitle { get; set; } = string.Empty;

    [Required(ErrorMessage = "Source name is required")]
    [MaxLength(100, ErrorMessage = "Source name cannot exceed 100 characters")]
    public string SourceName { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Source URL cannot exceed 500 characters")]
    [Url(ErrorMessage = "Source URL must be a valid URL")]
    public string? SourceUrl { get; set; }
}
