using JobCompany.Business.Services.CategoryServices;
using Microsoft.AspNetCore.Mvc;

namespace JobCompany.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController(ICategoryService service) : ControllerBase
    {
        readonly ICategoryService _service = service;

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateCategory(string categoryName)
        {
            await _service.CreateCategoryAsync(categoryName);
            return Ok();
        }

        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> UpdateCategory(string id, string? categoryName)
        {
            await _service.UpdateCategoryAsync(id, categoryName);
            return Ok();
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            await _service.DeleteCategoryAsync(id);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllCategories()
        {
            var data = await _service.GetAllCategoriesAsync();
            return Ok(data);
        }
    }
}