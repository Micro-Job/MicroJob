using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobCompany.Business.Dtos.CategoryDtos;

namespace JobCompany.Business.Services.CategoryServices
{
    public interface ICategoryService
    {
        Task CreateCategoryAsync(string categoryName);
        Task UpdateCategoryAsync(string id , string? categoryName);
        Task<ICollection<CategoryListDto>> GetAllCategoriesAsync();
        Task DeleteCategoryAsync(string id);
    }
}