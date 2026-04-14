using CircleHub.Client.Models;

namespace CircleHub.Client.Services.Interfaces;

public interface ICategoryDTOService
{
    //Create    
    Task<CategoryDTO> CreateCategoryAsync(CategoryDTO category, string userId);

    //Read
    Task<List<CategoryDTO>> GetCategoriesAsync(string userId);
    Task<CategoryDTO?> GetCategoryAsync(int id, string userId);

    //Update
    Task UpdateCategoryAsync(CategoryDTO category, string userId);
}
