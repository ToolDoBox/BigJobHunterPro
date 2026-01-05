namespace Application.DTOs.Applications;

public class CreateApplicationResponse
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string RoleTitle { get; set; } = string.Empty;
    public string SourceName { get; set; } = string.Empty;
    public string? SourceUrl { get; set; }
    public string Status { get; set; } = "Applied";
    public int Points { get; set; }
    public int TotalPoints { get; set; }
    public DateTime CreatedDate { get; set; }
}
