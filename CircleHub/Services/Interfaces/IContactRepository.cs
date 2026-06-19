using CircleHub.Models;

namespace CircleHub.Services.Interfaces;

public interface IContactRepository
{
    //Create
    Task<Contact> CreateContactAsync(Contact contact);

    //Read
    Task<Contact?> GetContactByIdAsync(int contactId, string userId);
    Task<List<Contact>> GetContactsAsync(string userId);

    //Update
    Task UpdateContactAsync(Contact contact);

    Task AddCategoriesToContact(int contactId, string userId, List<int> categoryIds);
    Task RemoveCategoriesFromContact(int contactId, string userId);

    //Delete
    Task DeleteContactAsync(int contactId, string userId);
}
