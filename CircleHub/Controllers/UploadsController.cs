using CircleHub.Data;
using CircleHub.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;


namespace CircleHub.Controllers
{
    [Route("uploads")]
    [ApiController]
    public class UploadsController(ApplicationDbContext context) : ControllerBase
    {
        [HttpGet("{id:guid}")]
        [OutputCache(VaryByRouteValueNames = ["id"], Duration = 60 * 60 * 24)]
        public async Task<IActionResult> GetImage(Guid id)
        {

            ImageUpload? image = await context.Images.FirstOrDefaultAsync(i => i.Id == id);

            if (image is null) return NotFound();

            return File(image.Data!, image.Type!);
        }
    }
}
