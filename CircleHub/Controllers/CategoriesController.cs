using CircleHub.Client.Models;
using CircleHub.Client.Services.Interfaces;
using CircleHub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using CircleHub.Data;


namespace CircleHub.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CategoriesController(ICategoryDTOService categoryService, UserManager<ApplicationUser> userManager) : ControllerBase
{
    private string UserId => userManager.GetUserId(User)!; //[Authorize] means userId will never be null

    [HttpGet]
    public async Task<ActionResult<List<CategoryDTO>>> GetCategories()
    {
        try
        {
            return await categoryService.GetCategoriesAsync(UserId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetCategories: {ex.Message}");
            return Problem(detail: ex.Message);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryDTO>> GetCategoryById([FromRoute] int id)
    {
        try
        {
            CategoryDTO? category = await categoryService.GetCategoryAsync(id, UserId);
            return category is null ? NotFound() : category;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetCategory: {ex.Message}");
            return Problem(detail: ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDTO>> CreateCategory([FromBody] CategoryDTO category)
    {
        try
        {
            CategoryDTO createdCategory = await categoryService.CreateCategoryAsync(category, UserId);
            return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.Id }, createdCategory);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CreateCategory: {ex.Message}");
            return Problem(detail: ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task <ActionResult> UpdateCategory([FromRoute] int id, [FromBody] CategoryDTO category)
    {
        if( id != category.Id)
        {
            return BadRequest();
        }

        try
        {
            await categoryService.UpdateCategoryAsync(category, UserId);
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in UpdateCategory: {ex.Message}");
            return Problem(detail: ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteCategory([FromRoute]int id)
    {
        try
        {
            await categoryService.DeleteCategoryAsync(id, UserId);
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in DeleteCategory: {ex.Message}");
            return Problem(detail: ex.Message);
        }
    }

    [HttpPost("{categoryId:int}/email")]
    public async Task<ActionResult> EmailCategory([FromRoute] int categoryId, [FromBody] EmailData emailData)
    {
        try
        {
            bool success = await categoryService.EmailCategoryAsync(categoryId, emailData, UserId);
            return success ? Ok() : BadRequest("Failed to send email.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in EmailCategory: {ex.Message}");
            return Problem(detail: ex.Message);
        }
    }

}
