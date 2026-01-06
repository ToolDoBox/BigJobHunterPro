// src/Domain/Entities/Application.cs
using Domain.Enums;

namespace Domain.Entities;

public class Application
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string RoleTitle { get; set; } = string.Empty;
    public string SourceName { get; set; } = string.Empty;
    public string? SourceUrl { get; set; }
    public ApplicationStatus Status { get; set; }
    public WorkMode? WorkMode { get; set; }
    public string? Location { get; set; }
    public int? SalaryMin { get; set; }
    public int? SalaryMax { get; set; }
    public string? JobDescription { get; set; }
    public List<string> RequiredSkills { get; set; } = new();
    public List<string> NiceToHaveSkills { get; set; } = new();
    public bool ParsedByAI { get; set; }
    public AiParsingStatus AiParsingStatus { get; set; } = AiParsingStatus.Pending;
    public DateTime? LastAIParsedDate { get; set; }
    public string? RawPageContent { get; set; }
    public int Points { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }

    // Navigation property
    public ApplicationUser User { get; set; } = null!;
}
