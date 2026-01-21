using Application.DTOs.Contacts;
using Application.Interfaces;
using Application.Interfaces.Data;
using Domain.Entities;

namespace Infrastructure.Services;

public class ContactService : IContactService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public ContactService(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<ContactDto> CreateContactAsync(Guid applicationId, CreateContactRequest request)
    {
        var userId = _currentUser.GetUserId()
            ?? throw new UnauthorizedAccessException("User not authenticated");

        // Verify application belongs to user
        var application = await _unitOfWork.Applications.GetByIdAsync(applicationId);
        if (application == null || application.UserId != userId)
        {
            throw new KeyNotFoundException($"Application {applicationId} not found");
        }

        var contact = new Contact
        {
            Id = Guid.NewGuid(),
            ApplicationId = applicationId,
            Name = request.Name.Trim(),
            Role = request.Role?.Trim(),
            Relationship = request.Relationship,
            Emails = request.Emails ?? new List<string>(),
            Phones = request.Phones ?? new List<string>(),
            LinkedIn = request.LinkedIn?.Trim(),
            Notes = request.Notes?.Trim(),
            CreatedDate = DateTime.UtcNow
        };

        await _unitOfWork.Contacts.AddAsync(contact);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(contact);
    }

    public async Task<ContactsListResponse> GetContactsAsync(Guid applicationId)
    {
        var userId = _currentUser.GetUserId()
            ?? throw new UnauthorizedAccessException("User not authenticated");

        // Verify application belongs to user
        var application = await _unitOfWork.Applications.GetByIdAsync(applicationId);
        if (application == null || application.UserId != userId)
        {
            throw new KeyNotFoundException($"Application {applicationId} not found");
        }

        var contacts = await _unitOfWork.Contacts.GetByApplicationIdAsync(applicationId);
        var contactsList = contacts.Select(MapToDto).ToList();

        return new ContactsListResponse
        {
            ApplicationId = applicationId,
            Contacts = contactsList,
            TotalCount = contactsList.Count
        };
    }

    public async Task<ContactDto?> UpdateContactAsync(Guid applicationId, Guid contactId, UpdateContactRequest request)
    {
        var userId = _currentUser.GetUserId()
            ?? throw new UnauthorizedAccessException("User not authenticated");

        // Verify application belongs to user
        var application = await _unitOfWork.Applications.GetByIdAsync(applicationId);
        if (application == null || application.UserId != userId)
        {
            throw new KeyNotFoundException($"Application {applicationId} not found");
        }

        var contact = await _unitOfWork.Contacts.GetByIdAsync(contactId);
        if (contact == null || contact.ApplicationId != applicationId)
        {
            return null;
        }

        contact.Name = request.Name.Trim();
        contact.Role = request.Role?.Trim();
        contact.Relationship = request.Relationship;
        contact.Emails = request.Emails ?? new List<string>();
        contact.Phones = request.Phones ?? new List<string>();
        contact.LinkedIn = request.LinkedIn?.Trim();
        contact.Notes = request.Notes?.Trim();
        contact.UpdatedDate = DateTime.UtcNow;

        _unitOfWork.Contacts.Update(contact);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(contact);
    }

    public async Task<bool> DeleteContactAsync(Guid applicationId, Guid contactId)
    {
        var userId = _currentUser.GetUserId()
            ?? throw new UnauthorizedAccessException("User not authenticated");

        // Verify application belongs to user
        var application = await _unitOfWork.Applications.GetByIdAsync(applicationId);
        if (application == null || application.UserId != userId)
        {
            throw new KeyNotFoundException($"Application {applicationId} not found");
        }

        var contact = await _unitOfWork.Contacts.GetByIdAsync(contactId);
        if (contact == null || contact.ApplicationId != applicationId)
        {
            return false;
        }

        _unitOfWork.Contacts.Delete(contact);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    private static ContactDto MapToDto(Contact entity)
    {
        return new ContactDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Role = entity.Role,
            Relationship = entity.Relationship.ToString(),
            Emails = entity.Emails,
            Phones = entity.Phones,
            LinkedIn = entity.LinkedIn,
            Notes = entity.Notes,
            CreatedDate = entity.CreatedDate,
            UpdatedDate = entity.UpdatedDate
        };
    }
}
