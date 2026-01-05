// src/Domain/Entities/Application.cs
using Domain.Enums;

namespace Domain.Entities;

public class Application
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string RoleTitle { get; set; } = string.Empty;
    public string SourceName { get; set; } = string.Empty;
    public string? SourceUrl { get; set; }
    public ApplicationStatus Status { get; set; }
    public int Points { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }

    // Navigation property
    public ApplicationUser User { get; set; } = null!;
}
