namespace Application.DTOs.Profile;

public class UpdateResumeHtmlResponse
{
    public bool Success { get; set; }
    public DateTime? ResumeHtmlUpdatedAt { get; set; }
    public int HtmlCharacterCount { get; set; }
    public string Message { get; set; } = string.Empty;
}
