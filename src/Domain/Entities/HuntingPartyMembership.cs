// src/Domain/Entities/HuntingPartyMembership.cs
using Domain.Enums;

namespace Domain.Entities;

public class HuntingPartyMembership
{
    public Guid Id { get; set; }
    public Guid HuntingPartyId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public PartyRole Role { get; set; }
    public DateTime JoinedDate { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public HuntingParty HuntingParty { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
}
