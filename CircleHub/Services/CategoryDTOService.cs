using CircleHub.Client.Models;
using CircleHub.Client.Services.Interfaces;
using CircleHub.Models;
using CircleHub.Services.Interfaces;

namespace CircleHub.Services;

public class CategoryDTOService(ICategoryRepository repository) : ICategoryDTOService
{
    public async Task<CategoryDTO> CreateCategoryAsync(CategoryDTO category, string userId)
    {
        Category newCategory = new()
        {
            AppUserId = userId,
            Name = category.Name,
        };

        newCategory = await repository.CreateCategoryAsync(newCategory);

        return newCategory.ToDTO();
    }

    public async Task<List<CategoryDTO>> GetCategoriesAsync(string userId)
    {
        List<Category> categories = await repository.GetCategoriesAsync(userId);

        return categories.Select(c => c.ToDTO()).ToList();
    }

    public async Task<CategoryDTO?> GetCategoryAsync(int id, string userId)
    {
        Category? category = await repository.GetCategoryAsync(id, userId);

        return category?.ToDTO();
    }

    public async Task UpdateCategoryAsync(CategoryDTO category, string userId)
    {
        Category? categoryToUpdate = await repository.GetCategoryAsync(category.Id, userId);

        if (categoryToUpdate is not null)
        {
            categoryToUpdate.Name = category.Name;
            categoryToUpdate.Contacts.Clear();

            await repository.UpdateCategoryAsync(categoryToUpdate, userId);
        }
    }
}
