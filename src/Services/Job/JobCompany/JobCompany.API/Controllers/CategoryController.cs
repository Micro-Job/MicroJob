using JobCompany.Business.Services.CategoryServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Attributes;
using SharedLibrary.Enums;

namespace JobCompany.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AuthorizeRole(UserRole.CompanyUser, UserRole.EmployeeUser)]
    public class CategoryController(ICategoryService service) : ControllerBase
    {
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateCategory(string categoryName)
        {
            await service.CreateCategoryAsync(categoryName);
            return Ok();
        }

        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> UpdateCategory(string id, string? categoryName)
        {
            await service.UpdateCategoryAsync(id, categoryName);
            return Ok();
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            await service.DeleteCategoryAsync(id);
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllCategories()
        {
            var data = await service.GetAllCategoriesAsync();
            return Ok(data);
        }
    }
}