using Job.Business.Services.Category;
using Microsoft.AspNetCore.Mvc;

namespace Job.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController(ICategoryService categoryService) : ControllerBase
    {
        private readonly ICategoryService _categoryService = categoryService;

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllCategories([FromQuery] int skip = 1, int take = 6)
        {
            var data = await _categoryService.GetAllCategroies(skip, take);
            return Ok(data);
        }
    }
}