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

    public Task<CategoryDTO?> GetCategoryAsync(int id, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateCategoryAsync(CategoryDTO category, string userId)
    {
        HttpResponseMessage response = await http.PutAsJsonAsync($"api/categories/{category.Id}", category);
        response.EnsureSuccessStatusCode();

    }
    public async Task DeleteCategoryAsync(int id, string userId)
    {
        HttpResponseMessage response = await http.DeleteAsync($"api/categories/{id}");
        response.EnsureSuccessStatusCode();
    }
}
