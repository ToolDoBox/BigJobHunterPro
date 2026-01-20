using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Profile;

public class UpdateResumeRequest
{
    [MaxLength(50000, ErrorMessage = "Resume text cannot exceed 50,000 characters")]
    public string? ResumeText { get; set; }
}
