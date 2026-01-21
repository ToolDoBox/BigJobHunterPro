namespace Application.DTOs.Contacts;

public class ContactsListResponse
{
    public Guid ApplicationId { get; set; }
    public List<ContactDto> Contacts { get; set; } = new();
    public int TotalCount { get; set; }
}
