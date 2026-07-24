using CircleHub.Client.Models;

namespace CircleHub.Client.Services.Interfaces;

public interface IContactDTOService
{
    //Create
    Task<ContactDTO> CreateContactAsync(ContactDTO contact, string userId);

    //Read
    Task<List<ContactDTO>> GetContactsAsync(string userId);
    Task<ContactDTO?> GetContactByIdAsync(int contactId, string userId);
    Task<List<ContactDTO>> GetContactsByCategoryAsync(int categoryId, string userId);
    Task<List<ContactDTO>> SearchContactsAsync(string searchTerm, string userId);

    //update
    Task UpdateContactAsync(ContactDTO contact, string userId);

    //delete
    Task DeleteContactAsync(int contactId, string userId);

    //email
    Task<bool> EmailContactAsync(int contactId, EmailData emailData, string userId);

}
