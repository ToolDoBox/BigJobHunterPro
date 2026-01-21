using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.DTOs.Contacts;

public class CreateContactRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Role { get; set; }

    [Required]
    public ContactRelationship Relationship { get; set; }

    public List<string> Emails { get; set; } = new();

    public List<string> Phones { get; set; } = new();

    [MaxLength(500)]
    public string? LinkedIn { get; set; }

    [MaxLength(2000)]
    public string? Notes { get; set; }
}
