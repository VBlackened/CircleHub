using CircleHub.Client.Models;

namespace CircleHub.Client.Services.Interfaces;

public interface IContactDTOService
{
    //Create
    Task<ContactDTO> CreateContactAsync(ContactDTO contact, string userId);

    //Read
    Task<List<ContactDTO>> GetContactsAsync(string userId);
    Task<ContactDTO?> GetContactByIdAsync(int contactId, string userId);

    //update
    Task UpdateContactAsync(ContactDTO contact, string userId);

    //delete
    Task DeleteContactAsync(int contactId, string userId);

}
