using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
    public int Points { get; set; } = 0;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
