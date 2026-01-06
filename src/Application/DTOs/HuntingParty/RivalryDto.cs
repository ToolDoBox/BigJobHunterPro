namespace Application.DTOs.HuntingParty;

public class RivalryDto
{
    public int CurrentRank { get; set; }
    public int TotalMembers { get; set; }
    public RivalInfoDto? UserAhead { get; set; }
    public RivalInfoDto? UserBehind { get; set; }
}

public class RivalInfoDto
{
    public string DisplayName { get; set; } = string.Empty;
    public int Points { get; set; }
    public int Gap { get; set; }
}
