using CircleHub.Client.Models;
using CircleHub.Client.Services.Interfaces;
using CircleHub.Helpers;
using CircleHub.Models;
using CircleHub.Services.Interfaces;

namespace CircleHub.Services;

public class ContactDTOService(IContactRepository repository) : IContactDTOService
{
    public async Task<ContactDTO> CreateContactAsync(ContactDTO dto, string userId)
    {
        Contact newContact = new Contact
        {
            AppUserId = userId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            BirthDate = dto.BirthDate,
            Address1 = dto.Address1,
            Address2 = dto.Address2,
            City = dto.City,
            PostalCode = dto.PostalCode,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Created = DateTimeOffset.UtcNow, 
        };

        //Save image. Convert the URL to the imageUpload type.
        if (dto.ProfileImageUrl?.StartsWith("data:") == true)
        {
            newContact.Image = ImageHelper.GetImageUploadFromUrl(dto.ProfileImageUrl);
        } 

        newContact = await repository.CreateContactAsync(newContact);

        //add categories to the contact (update the join table)
        List<int> categoryIds = dto.Categories.Select(c => c.Id).ToList();
        await repository.AddCategoriesToContact(newContact.Id, userId, categoryIds);

        //requery the database to get the updated contact with the categories included
        newContact = (await repository.GetContactByIdAsync(newContact.Id, userId))!;

        return newContact.ToDTO();
    }

    public async Task<List<ContactDTO>> GetContactsAsync(string userId)
    {
        List<Contact> contacts = await repository.GetContactsAsync(userId);

        List<ContactDTO> contactDTOs = [.. contacts.Select(c => c.ToDTO())];

        return contactDTOs;
    }
    public async Task<ContactDTO?> GetContactByIdAsync(int contactId, string userId)
    {
        Contact? contact = await repository.GetContactByIdAsync(contactId, userId);
        return contact?.ToDTO();
    }
    public async Task<List<ContactDTO>> GetContactsByCategoryAsync(int categoryId, string userId)
    {
        List<Contact> contacts = await repository.GetContactsByCategoryAsync(categoryId, userId);

        List<ContactDTO> contactDTOs = contacts.Select(c => c.ToDTO()).ToList();

        return contactDTOs;
    }
    public async Task<List<ContactDTO>> SearchContactsAsync(string searchTerm, string userId)
    {
        List<Contact> contacts = await repository.SearchContactsAsync(searchTerm, userId);

        List<ContactDTO> contactDTOs = [.. contacts.Select(c => c.ToDTO())];

        return contactDTOs;
    }

    public async Task UpdateContactAsync(ContactDTO dto, string userId)
    {
        Contact? contact = await repository.GetContactByIdAsync(dto.Id, userId);

        if (contact is not null)
        {
            contact.FirstName = dto.FirstName;
            contact.LastName = dto.LastName;
            contact.BirthDate = dto.BirthDate;
            contact.Address1 = dto.Address1;
            contact.Address2 = dto.Address2;
            contact.City = dto.City;
            contact.PostalCode = dto.PostalCode;
            contact.Email = dto.Email;
            contact.PhoneNumber = dto.PhoneNumber;
            //Created date doesn't change on update
            //AppuserId doesn't change on update
            //ID doesn't change on update
        }

        if (dto.ProfileImageUrl?.StartsWith("data:") == true)
        {
            contact.Image = ImageHelper.GetImageUploadFromUrl(dto.ProfileImageUrl);
        }
        else
        {
            // If the ProfileImageUrl is null or empty, remove the image
            contact.Image = null;
        }

        contact.Categories.Clear();
        await repository.UpdateContactAsync(contact);
        await repository.RemoveCategoriesFromContact(contact.Id, userId);

        List<int> categoryIds = dto.Categories.Select(c => c.Id).ToList();
        await repository.AddCategoriesToContact(contact.Id, userId, categoryIds);
    }

    public async Task DeleteContactAsync(int contactId, string userId)
    {
        await repository.DeleteContactAsync(contactId, userId);
    }
}
