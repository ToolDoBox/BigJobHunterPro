using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
    public int Points { get; set; } = 0;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    // Streak tracking
    public int CurrentStreak { get; set; } = 0;
    public int LongestStreak { get; set; } = 0;
    public DateTime? LastActivityDate { get; set; }
    public DateTime? StreakLastUpdated { get; set; }

    // Navigation properties
    public ICollection<Application> Applications { get; set; } = new List<Application>();
    public int TotalPoints { get; set; }

    // Hunting party membership (one party per user for MVP)
    public ICollection<HuntingPartyMembership> PartyMemberships { get; set; } = new List<HuntingPartyMembership>();

    // Resume storage
    public string? ResumeText { get; set; }
    public DateTime? ResumeUpdatedAt { get; set; }
}
