namespace Application.DTOs.Applications;

public class CreateApplicationResponse
{
    public Guid Id { get; set; }
    public string? SourceUrl { get; set; }
    public string Status { get; set; } = "Applied";
    public string AiParsingStatus { get; set; } = "Pending";
    public int Points { get; set; }
    public int TotalPoints { get; set; }
    public DateTime CreatedDate { get; set; }
}
