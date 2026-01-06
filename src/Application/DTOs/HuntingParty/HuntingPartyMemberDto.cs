namespace Application.DTOs.HuntingParty;

public class HuntingPartyMemberDto
{
    public string UserId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public int TotalPoints { get; set; }
    public DateTime JoinedDate { get; set; }
    public string Role { get; set; } = string.Empty;
}
