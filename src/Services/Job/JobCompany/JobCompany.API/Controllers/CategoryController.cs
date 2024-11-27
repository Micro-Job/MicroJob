using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobCompany.Business.Dtos.CategoryDtos;
using JobCompany.Business.Services.CategoryServices;
using Microsoft.AspNetCore.Mvc;

namespace JobCompany.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        readonly ICategoryService _service;

        public CategoryController(ICategoryService service)
        {
            _service = service;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateCategoryAsync(string categoryName)
        {
            await _service.CreateCategoryAsync(categoryName);
            return Ok();
        }

        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> UpdateCategoryAsync(string id, string? categoryName)
        {
            await _service.UpdateCategoryAsync(id, categoryName);
            return Ok();
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteCategoryAsync(string id)
        {
            await _service.DeleteCategoryAsync(id);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllCategoriesAsync()
        {
            await _service.GetAllCategoriesAsync();
            return Ok();
        }
    }
}