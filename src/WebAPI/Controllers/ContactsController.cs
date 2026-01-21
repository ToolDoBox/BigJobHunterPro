using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Contacts;
using Application.DTOs.Auth;
using Application.Interfaces;

namespace WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/applications/{applicationId:guid}/contacts")]
public class ContactsController : ControllerBase
{
    private readonly IContactService _contactService;

    public ContactsController(IContactService contactService)
    {
        _contactService = contactService;
    }

    /// <summary>
    /// Creates a new contact for an application
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ContactDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateContact(Guid applicationId, [FromBody] CreateContactRequest request)
    {
        try
        {
            var result = await _contactService.CreateContactAsync(applicationId, request);

            return CreatedAtAction(
                nameof(GetContacts),
                new { applicationId },
                result
            );
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { error = "Your session expired. Please log in again." });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Application not found" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ErrorResponse("Validation failed", new List<string> { ex.Message }));
        }
    }

    /// <summary>
    /// Gets all contacts for an application
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ContactsListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetContacts(Guid applicationId)
    {
        try
        {
            var result = await _contactService.GetContactsAsync(applicationId);
            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { error = "Your session expired. Please log in again." });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Application not found" });
        }
    }

    /// <summary>
    /// Updates an existing contact
    /// </summary>
    [HttpPut("{contactId:guid}")]
    [ProducesResponseType(typeof(ContactDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateContact(Guid applicationId, Guid contactId, [FromBody] UpdateContactRequest request)
    {
        try
        {
            var result = await _contactService.UpdateContactAsync(applicationId, contactId, request);

            if (result == null)
            {
                return NotFound(new { error = "Contact not found" });
            }

            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { error = "Your session expired. Please log in again." });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Application not found" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ErrorResponse("Validation failed", new List<string> { ex.Message }));
        }
    }

    /// <summary>
    /// Deletes a contact
    /// </summary>
    [HttpDelete("{contactId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteContact(Guid applicationId, Guid contactId)
    {
        try
        {
            var deleted = await _contactService.DeleteContactAsync(applicationId, contactId);

            if (!deleted)
            {
                return NotFound(new { error = "Contact not found" });
            }

            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { error = "Your session expired. Please log in again." });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Application not found" });
        }
    }
}
