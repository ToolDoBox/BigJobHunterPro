namespace Application.DTOs.AiParsing;

public class AiParsingResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }

    // Parsed fields from job posting
    public string? CompanyName { get; set; }
    public string? RoleTitle { get; set; }
    public string? WorkMode { get; set; }  // "Remote", "Hybrid", "Onsite", or null
    public string? Location { get; set; }
    public int? SalaryMin { get; set; }
    public int? SalaryMax { get; set; }
    public string? JobDescription { get; set; }
    public List<string> RequiredSkills { get; set; } = new();
    public List<string> NiceToHaveSkills { get; set; } = new();
    public List<ParsedContactResult> Contacts { get; set; } = new();

    public static AiParsingResult Failed(string errorMessage) => new()
    {
        Success = false,
        ErrorMessage = errorMessage
    };
}

public class ParsedContactResult
{
    public string Name { get; set; } = string.Empty;
    public string? Role { get; set; }
    public string Relationship { get; set; } = "Other";
    public List<string> Emails { get; set; } = new();
    public List<string> Phones { get; set; } = new();
    public string? LinkedIn { get; set; }
}
