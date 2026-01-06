namespace Application.DTOs.HuntingParty;

public class LeaderboardEntryDto
{
    public int Rank { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public int TotalPoints { get; set; }
    public int ApplicationCount { get; set; }
    public bool IsCurrentUser { get; set; }
}
