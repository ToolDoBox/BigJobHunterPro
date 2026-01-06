using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Applications;

public class UpdateApplicationRequest
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

    [Required(ErrorMessage = "Status is required")]
    public string Status { get; set; } = string.Empty;

    public string? WorkMode { get; set; }

    [MaxLength(200, ErrorMessage = "Location cannot exceed 200 characters")]
    public string? Location { get; set; }

    [Range(0, 1000, ErrorMessage = "Salary minimum must be between 0 and 1000")]
    public int? SalaryMin { get; set; }

    [Range(0, 1000, ErrorMessage = "Salary maximum must be between 0 and 1000")]
    public int? SalaryMax { get; set; }

    [MaxLength(4000, ErrorMessage = "Job description cannot exceed 4000 characters")]
    public string? JobDescription { get; set; }

    public List<string> RequiredSkills { get; set; } = new();

    public List<string> NiceToHaveSkills { get; set; } = new();

    public string? RawPageContent { get; set; }
}
