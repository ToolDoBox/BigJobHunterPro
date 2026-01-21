using Application.DTOs.Contacts;
using Application.DTOs.TimelineEvents;

namespace Application.DTOs.Applications;

public class ApplicationDto
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string RoleTitle { get; set; } = string.Empty;
    public string SourceName { get; set; } = string.Empty;
    public string? SourceUrl { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? WorkMode { get; set; }
    public string? Location { get; set; }
    public int? SalaryMin { get; set; }
    public int? SalaryMax { get; set; }
    public string? JobDescription { get; set; }
    public List<string> RequiredSkills { get; set; } = new();
    public List<string> NiceToHaveSkills { get; set; } = new();
    public bool ParsedByAI { get; set; }
    public string AiParsingStatus { get; set; } = string.Empty;
    public int Points { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public DateTime? LastAIParsedDate { get; set; }
    public string? RawPageContent { get; set; }
    public string? CoverLetterHtml { get; set; }
    public DateTime? CoverLetterGeneratedAt { get; set; }
    public List<TimelineEventDto> TimelineEvents { get; set; } = new();
    public List<ContactDto> Contacts { get; set; } = new();
}
