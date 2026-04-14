using CircleHub.Data;
using CircleHub.Models;
using CircleHub.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CircleHub.Services;

public class CategoryRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory) : ICategoryRepository
{
    public async Task<Category> CreateCategoryAsync(Category category)
    {
        using ApplicationDbContext context = dbContextFactory.CreateDbContext();
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        return category;
    }

    public async Task<List<Category>> GetCategoriesAsync(string userId)
    {
        using ApplicationDbContext context = dbContextFactory.CreateDbContext();

        List<Category> categories = await context.Categories
            .Where(c => c.AppUserId == userId)
            .Include(c => c.Contacts)
            .ToListAsync();

        return categories;
    }
    public async Task<Category?> GetCategoryAsync(int id, string userId)
    {
        using ApplicationDbContext context = dbContextFactory.CreateDbContext();

        Category? category = await context.Categories
            .Include(c => c.Contacts)
            .FirstOrDefaultAsync(c => c.Id == id && c.AppUserId == userId);
        return category;
    }

    public async Task UpdateCategoryAsync(Category category, string userId)
    {
        using ApplicationDbContext context = dbContextFactory.CreateDbContext();

        if (await context.Categories.AnyAsync(c => c.Id == category.Id && c.AppUserId == userId))
        {
            context.Categories.Update(category);
            await context.SaveChangesAsync();
        }
    }
}
