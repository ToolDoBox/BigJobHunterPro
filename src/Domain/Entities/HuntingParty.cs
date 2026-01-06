// src/Domain/Entities/HuntingParty.cs
namespace Domain.Entities;

public class HuntingParty
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string InviteCode { get; set; } = string.Empty;
    public string CreatorId { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ApplicationUser Creator { get; set; } = null!;
    public ICollection<HuntingPartyMembership> Memberships { get; set; } = new List<HuntingPartyMembership>();
}
