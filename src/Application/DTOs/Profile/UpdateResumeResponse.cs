namespace Application.DTOs.Profile;

public class UpdateResumeResponse
{
    public bool Success { get; set; }
    public DateTime? ResumeUpdatedAt { get; set; }
    public int CharacterCount { get; set; }
    public string Message { get; set; } = string.Empty;
}
