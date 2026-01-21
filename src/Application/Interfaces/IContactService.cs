using Application.DTOs.Contacts;

namespace Application.Interfaces;

public interface IContactService
{
    Task<ContactDto> CreateContactAsync(Guid applicationId, CreateContactRequest request);
    Task<ContactsListResponse> GetContactsAsync(Guid applicationId);
    Task<ContactDto?> UpdateContactAsync(Guid applicationId, Guid contactId, UpdateContactRequest request);
    Task<bool> DeleteContactAsync(Guid applicationId, Guid contactId);
}
