using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobCompany.Business.Dtos.CategoryDtos;

namespace JobCompany.Business.Services.CategoryServices
{
    public interface ICategoryService
    {
        Task CreateCategoryAsync(CreateCategoryDto dto);
        Task UpdateCategoryAsync(string id , UpdateCategoryDto dto);
        Task<ICollection<CategoryListDto>> GetAllCategoriesAsync();
        Task DeleteCategoryAsync(string id);
    }
}