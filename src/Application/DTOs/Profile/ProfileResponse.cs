namespace Application.DTOs.Profile;

public class ProfileResponse
{
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? ResumeText { get; set; }
    public DateTime? ResumeUpdatedAt { get; set; }
    public int CharacterCount { get; set; }
}
