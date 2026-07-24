using CircleHub.Client.Models;
using CircleHub.Client.Services.Interfaces;
using System.Net.Http.Json;

namespace CircleHub.Client.Services;

public class WASMCategoryDTOService(HttpClient http) : ICategoryDTOService
{
    public async Task<CategoryDTO> CreateCategoryAsync(CategoryDTO category, string userId)
    {
        HttpResponseMessage response = await http.PostAsJsonAsync("api/categories", category);
        response.EnsureSuccessStatusCode();

        CategoryDTO? createdCategory = await response.Content.ReadFromJsonAsync<CategoryDTO>();

        return createdCategory ?? throw new HttpRequestException("Invalid JSON response from server.");
    }

    public async Task<List<CategoryDTO>> GetCategoriesAsync(string userId)
    {
        return await http.GetFromJsonAsync<List<CategoryDTO>>($"api/categories") ?? [];
    }

    public async Task<CategoryDTO?> GetCategoryAsync(int categoryId, string userId)
    {
        try
        {
            return await http.GetFromJsonAsync<CategoryDTO>($"api/categories/{categoryId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching category by ID: {ex.Message}");
            return null;
        }
    }

    public async Task UpdateCategoryAsync(CategoryDTO category, string userId)
    {
        HttpResponseMessage response = await http.PutAsJsonAsync($"api/categories/{category.Id}", category);
        response.EnsureSuccessStatusCode();

    }
    public async Task DeleteCategoryAsync(int categoryId, string userId)
    {
        HttpResponseMessage response = await http.DeleteAsync($"api/categories/{categoryId}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<bool> EmailCategoryAsync(int categoryId, EmailData emailData, string userId)
    {
        try
        {
            var response = await http.PostAsJsonAsync($"api/categories/{categoryId}/email", emailData);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error emailing category: {ex.Message}");
            return false;
        }
    }
}
