namespace Application.DTOs.Auth;

public class GetMeResponse
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public int Points { get; set; }
}
