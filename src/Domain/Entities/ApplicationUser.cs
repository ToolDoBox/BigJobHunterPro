using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
    public int Points { get; set; } = 0;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    // Add to ApplicationUser.cs
    public ICollection<Application> Applications { get; set; } = new List<Application>();
    public int TotalPoints { get; set; }
}
