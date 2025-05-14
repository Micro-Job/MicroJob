using JobCompany.Business.Dtos.CategoryDtos;
using JobCompany.Business.Dtos.Common;

namespace JobCompany.Business.Services.CategoryServices;

public interface ICategoryService
{
    Task CreateCategoryAsync(CategoryCreateDto dto);
    Task UpdateCategoryAsync(List<CategoryUpdateDto> dtos);
    Task<ICollection<CategoryListDto>> GetAllCategoriesAsync();
    Task<DataListDto<CategoryListDto>> GetCategoriesPagedAsync(int skip, int take, string? name);
    Task<CategoryGetByIdDto> CategoryGetByIdAsync(Guid id);
    Task DeleteCategoryAsync(string id);
}