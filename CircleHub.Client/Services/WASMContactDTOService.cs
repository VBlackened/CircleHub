using CircleHub.Client.Models;
using CircleHub.Client.Services.Interfaces;
using System.Net.Http.Json;

namespace CircleHub.Client.Services;

public class WASMContactDTOService(HttpClient http) : IContactDTOService
{
    public async Task<ContactDTO> CreateContactAsync(ContactDTO contact, string userId)
    {
        HttpResponseMessage response = await http.PostAsJsonAsync("api/contacts", contact);
        response.EnsureSuccessStatusCode();

        ContactDTO? createdContact = await response.Content.ReadFromJsonAsync<ContactDTO>();
        return createdContact ?? throw new HttpRequestException("Failed to deserialize the created contact.");
    }

    public async Task DeleteContactAsync(int contactId, string userId)
    {
        HttpResponseMessage response = await http.DeleteAsync($"api/contacts/{contactId}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<bool> EmailContactAsync(int contactId, EmailData emailData, string userId)
    {
        try
        {
            HttpResponseMessage response = await http.PostAsJsonAsync($"api/contacts/{contactId}/email", emailData);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error emailing contact: {ex.Message}");
            return false;
        }
    }

    public async Task<ContactDTO?> GetContactByIdAsync(int contactId, string userId)
    {
        try
        {
            return await http.GetFromJsonAsync<ContactDTO>($"api/contacts/{contactId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching contact by ID: {ex.Message}");
            return null;
        }
    }

    public async Task<List<ContactDTO>> GetContactsAsync(string userId)
    {
        try
        {
            return await http.GetFromJsonAsync<List<ContactDTO>>($"api/contacts") ?? [];
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching contacts: {ex.Message}");
            throw;
        }
    }

    public async Task<List<ContactDTO>> GetContactsByCategoryAsync(int categoryId, string userId)
    {
        return await http.GetFromJsonAsync<List<ContactDTO>>($"api/contacts?categoryId={categoryId}") ?? [];
    }

    public async Task<List<ContactDTO>> SearchContactsAsync(string searchTerm, string userId)
    {
        try
        {
            return await http.GetFromJsonAsync<List<ContactDTO>>($"api/contacts/search?searchTerm={searchTerm}") ?? [];
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return [];
        }
    }

    public async Task UpdateContactAsync(ContactDTO contact, string userId)
    {
        HttpResponseMessage response = await http.PutAsJsonAsync($"api/contacts/{contact.Id}", contact);
        response.EnsureSuccessStatusCode();
    }
}
