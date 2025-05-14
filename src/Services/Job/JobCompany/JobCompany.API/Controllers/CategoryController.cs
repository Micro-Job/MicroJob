using JobCompany.Business.Dtos.CategoryDtos;
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
        public async Task<IActionResult> CreateCategory(CategoryCreateDto dto)
        {
            await service.CreateCategoryAsync(dto);
            return Ok();
        }

        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> UpdateCategory(List<CategoryUpdateDto> dtos)
        {
            await service.UpdateCategoryAsync(dtos);
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

        [AllowAnonymous]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetCategoriesPaged(string? name, int skip = 1, int take = 5)
        {
            return Ok(await service.GetCategoriesPagedAsync(skip, take, name));
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> CategoryGetById(Guid id)
        {
            var data = await service.CategoryGetByIdAsync(id);
            return Ok(data);
        }
    }
}