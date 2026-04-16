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
    private string _userId => userManager.GetUserId(User)!; //[Authorize] means userId will never be null

    [HttpGet]
    public async Task<ActionResult<List<CategoryDTO>>> GetCategories()
    {
        try
        {
            return await categoryService.GetCategoriesAsync(_userId);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return Problem();
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDTO>> GetCategory(int id)
    {
        try
        {
            CategoryDTO? category = await categoryService.GetCategoryAsync(id, _userId);
            return category is null ? NotFound() : category;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return Problem();
        }
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDTO>> CreateCategory(CategoryDTO category)
    {
        try
        {
            CategoryDTO createdCategory = await categoryService.CreateCategoryAsync(category, _userId);
            return CreatedAtAction(nameof(GetCategory), new { id = createdCategory.Id }, createdCategory);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return Problem();
        }
    }
}
