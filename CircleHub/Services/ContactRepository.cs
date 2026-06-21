using CircleHub.Data;
using CircleHub.Models;
using CircleHub.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CircleHub.Services;

public class ContactRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory) : IContactRepository
{
    public async Task<Contact> CreateContactAsync(Contact contact)
    {
        using ApplicationDbContext context = dbContextFactory.CreateDbContext();

        context.Contacts.Add(contact);
        await context.SaveChangesAsync();

        return contact;
    }

    public async Task AddCategoriesToContact(int contactId, string userId, List<int> categoryIds)
    {
        using ApplicationDbContext context = dbContextFactory.CreateDbContext();

        Contact? contact = await context.Contacts
            .Include(c => c.Categories)
            .FirstOrDefaultAsync(c => c.Id == contactId && c.AppUserId == userId);

        if (contact is not null)
        {
            foreach (int categoryId in categoryIds)
            {
                Category? category = await context.Categories
                    .Include(c => c.Contacts)
                    .FirstOrDefaultAsync(c => c.Id == categoryId && c.AppUserId == userId);
                if (category is not null)
                {
                    contact.Categories.Add(category);
                }
            }

            await context.SaveChangesAsync();
        }
    }

    public async Task<Contact?> GetContactByIdAsync(int contactId, string userId)
    {
        using ApplicationDbContext context = dbContextFactory.CreateDbContext();

        Contact? contact = await context.Contacts
            .Include(c => c.Categories)
            .FirstOrDefaultAsync(c => c.Id == contactId && c.AppUserId == userId);

        return contact;
    }

    public async Task<List<Contact>> GetContactsAsync(string userId)
    {
        using ApplicationDbContext context = dbContextFactory.CreateDbContext();

        List<Contact> contacts = await context.Contacts
            .Where(c => c.AppUserId == userId)
            .Include(c => c.Categories)
            .ToListAsync();

        return contacts;
    }
    public async Task<List<Contact>> SearchContactsAsync(string searchTerm, string userId)
    {
        using ApplicationDbContext context = dbContextFactory.CreateDbContext();

        string searchTermLower = searchTerm.Trim().ToLower();

        List<Contact> contacts = await context.Contacts
            .Where(c => c.AppUserId == userId)
            .Include(c => c.Categories)
            .Where(c => string.IsNullOrEmpty(searchTermLower) 
            || c.FirstName!.ToLower().Contains(searchTermLower)
            || c.LastName!.ToLower().Contains(searchTermLower)
            || c.Address1!.ToLower().Contains(searchTermLower)
            || c.Address2!.ToLower().Contains(searchTermLower)
            || c.Email!.ToLower().Contains(searchTermLower)
            || c.PhoneNumber!.ToLower().Contains(searchTermLower)
            || c.Categories.Any(cat => cat.Name!.ToLower().Contains(searchTermLower)))
            .ToListAsync();

        return contacts;
    }

    public async Task UpdateContactAsync(Contact contact)
    {
        using ApplicationDbContext context = dbContextFactory.CreateDbContext();

        if (await context.Contacts.AnyAsync(c => c.Id == contact.Id && c.AppUserId == contact.AppUserId))
        {
            ImageUpload? oldImage = null;

            if (contact.Image is not null) //new image provided
            {
                if (contact.Image.Id != contact.ImageId) //find the old img
                {
                    oldImage = await context.Images.FirstOrDefaultAsync(img => img.Id == contact.ImageId);
                }

                //save the new image
                contact.ImageId = contact.Image.Id;
                context.Images.Add(contact.Image);
            }

            context.Contacts.Update(contact);
            await context.SaveChangesAsync();

            if (oldImage is not null)
            {
                context.Images.Remove(oldImage);
                await context.SaveChangesAsync();
            }

        }

    }

    public async Task RemoveCategoriesFromContact(int contactId, string userId)
    {
        using ApplicationDbContext context = dbContextFactory.CreateDbContext();

        Contact? contact = await context.Contacts
            .Include(c => c.Categories)
            .FirstOrDefaultAsync(c => c.Id == contactId && c.AppUserId == userId);

        if (contact is not null)
        {
            contact.Categories.Clear();
            await context.SaveChangesAsync();
        }
    }

    public async Task DeleteContactAsync(int contactId, string userId)
    {
        using ApplicationDbContext context = dbContextFactory.CreateDbContext();

        Contact? contact = await context.Contacts
            .FirstOrDefaultAsync(c => c.Id == contactId && c.AppUserId == userId);

        if (contact is not null)
        {
            context.Contacts.Remove(contact);
            await context.SaveChangesAsync();
        }
    }
}
