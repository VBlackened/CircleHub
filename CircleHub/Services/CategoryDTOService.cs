using CircleHub.Client.Models;
using CircleHub.Client.Services.Interfaces;
using CircleHub.Data;
using CircleHub.Models;
using CircleHub.Services.Email;
using CircleHub.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Resend;

namespace CircleHub.Services;

public class CategoryDTOService(ICategoryRepository repository, IEmailService emailService, UserManager<ApplicationUser> _userManager) : ICategoryDTOService
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

    public async Task DeleteCategoryAsync(int id, string userId)
    {
        await repository.DeleteCategoryAsync(id, userId);
    }

    public async Task<bool> EmailCategoryAsync(int categoryId, EmailData emailData, string userId)
    {
        Category? category = await repository.GetCategoryAsync(categoryId, userId);
        if (category is null || !emailData.Recipients.Any())
        {
            return false;
        }

        try
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                return false;
            }

            var request = new EmailRequest
            {
                Recipients = emailData.Recipients,
                Subject = emailData.Subject,
                HtmlBody = $"""
                <p>{emailData.Body.Replace("\n", "<br>")}</p>
                """,
                ReplyToEmail = emailData.ReplyToEmail,
                FromName = $"{user.FirstName} {user.LastName} by CircleHub"
            };

            await emailService.SendEmailAsync(request);
            return true;
        }
        catch
        {
            return false;
        }

    }
}
