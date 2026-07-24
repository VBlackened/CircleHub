using CircleHub.Client.Models;
using CircleHub.Client.Services.Interfaces;
using CircleHub.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CircleHub.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ContactsController(IContactDTOService contactService, UserManager<ApplicationUser> userManager) : ControllerBase
{
    private string UserId => userManager.GetUserId(User)!; //[Authorize] means userId will never be null

    [HttpGet]
    public async Task<ActionResult<List<ContactDTO>>> GetContacts([FromQuery] int? categoryId)
    {
        try
        {
            if (categoryId is not null or 0)
            {
                return await contactService.GetContactsByCategoryAsync(categoryId.Value, UserId);
            }
            else
            {
                return await contactService.GetContactsAsync(UserId);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetContacts: {ex.Message}");
            return Problem(detail: ex.Message);
        }
    }

    [HttpGet("{contactId:int}")]
    public async Task<ActionResult<ContactDTO>> GetContactById([FromRoute] int contactId)
    {
        try
        {
            ContactDTO? contact = await contactService.GetContactByIdAsync(contactId, UserId);
            return contact is null ? NotFound() : contact;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetContactById: {ex.Message}");
            return Problem(detail: ex.Message);
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<ContactDTO>>> SearchContacts([FromQuery] string searchTerm)
    {
        if (string.IsNullOrEmpty(searchTerm))
        {
            return BadRequest("Search term cannot be empty.");
        }

        try
        {
            return await contactService.SearchContactsAsync(searchTerm, UserId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in SearchContacts: {ex.Message}");
            return Problem(detail: ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<ContactDTO>> CreateContact([FromBody] ContactDTO contact)
    {
        try
        {
            ContactDTO newContact = await contactService.CreateContactAsync(contact, UserId);
            return CreatedAtAction(nameof(GetContactById), new { contactId = newContact.Id }, newContact);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CreateContact: {ex.Message}");
            return Problem(detail: ex.Message);
        }
    }

    [HttpDelete("{contactId:int}")]
    public async Task<ActionResult> DeleteContact([FromRoute] int contactId)
    {
        try
        {
            await contactService.DeleteContactAsync(contactId, UserId);
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in DeleteContact: {ex.Message}");
            return Problem(detail: ex.Message);
        }
    }

    [HttpPut("{contactId:int}")]
    public async Task<ActionResult> UpdateContact([FromRoute] int contactId, [FromBody] ContactDTO contact)
    {
        if (contactId != contact.Id)
        {
            return BadRequest("Contact ID mismatch.");
        }
        try
        {
            await contactService.UpdateContactAsync(contact, UserId);
            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in UpdateContact: {ex.Message}");
            return Problem(detail: ex.Message);
        }
    }

    [HttpPost("{contactId:int}/email")]
    public async Task<ActionResult> EmailContact([FromRoute] int contactId, [FromBody] EmailData emailData)
    {
        try
        {
            bool success = await contactService.EmailContactAsync(contactId, emailData, UserId);
            return success ? Ok() : BadRequest("Failed to send email.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in EmailContact: {ex.Message}");
            return Problem(detail: ex.Message);
        }
    }
}