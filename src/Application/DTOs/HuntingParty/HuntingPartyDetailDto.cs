namespace Application.DTOs.HuntingParty;

public class HuntingPartyDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string InviteCode { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public bool IsCreator { get; set; }
    public List<HuntingPartyMemberDto> Members { get; set; } = new();
}
