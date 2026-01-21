using Domain.Enums;

namespace Domain.Entities;

public class Contact
{
    public Guid Id { get; set; }
    public Guid ApplicationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Role { get; set; }
    public ContactRelationship Relationship { get; set; }
    public List<string> Emails { get; set; } = new();
    public List<string> Phones { get; set; } = new();
    public string? LinkedIn { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }

    // Navigation property
    public Application Application { get; set; } = null!;
}
