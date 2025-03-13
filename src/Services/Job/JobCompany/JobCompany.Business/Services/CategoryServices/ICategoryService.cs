using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobCompany.Business.Dtos.CategoryDtos;

namespace JobCompany.Business.Services.CategoryServices
{
    public interface ICategoryService
    {
        Task CreateCategoryAsync(CategoryCreateDto dto);
        Task UpdateCategoryAsync(List<CategoryUpdateDto> dtos);
        Task<ICollection<CategoryListDto>> GetAllCategoriesAsync();
        Task<CategoryGetByIdDto> CategoryGetByIdAsync(Guid id);
        Task DeleteCategoryAsync(string id);
    }
}