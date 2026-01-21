namespace Application.DTOs.Contacts;

public class ContactDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Role { get; set; }
    public string Relationship { get; set; } = string.Empty;
    public List<string> Emails { get; set; } = new();
    public List<string> Phones { get; set; } = new();
    public string? LinkedIn { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}
