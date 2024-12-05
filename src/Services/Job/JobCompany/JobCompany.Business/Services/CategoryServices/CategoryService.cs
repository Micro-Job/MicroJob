using JobCompany.Business.Dtos.CategoryDtos;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using Microsoft.EntityFrameworkCore;

namespace JobCompany.Business.Services.CategoryServices
{
    public class CategoryService(JobCompanyDbContext context) : ICategoryService
    {
        readonly JobCompanyDbContext _context = context;

        public async Task CreateCategoryAsync(string categoryName)
        {
            var existCategory = await _context.Categories.FirstOrDefaultAsync(x => x.CategoryName == categoryName);
            if (existCategory != null) throw new Exceptions.Common.IsAlreadyExistException<Category>();

            var category = new Category
            {
                CategoryName = categoryName,
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(string id)
        {
            var categoryId = Guid.Parse(id);
            var category = await _context.Categories.FindAsync(categoryId) ??
            throw new Exceptions.Common.NotFoundException<Category>();

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        public async Task<ICollection<CategoryListDto>> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories.Select(c => new CategoryListDto
            {
                Id = c.Id,
                CategoryName = c.CategoryName
            }).ToListAsync();
            return categories;
        }

        public async Task UpdateCategoryAsync(string id, string? categoryName)
        {
            var categoryId = Guid.Parse(id);
            var category = await _context.Categories.FindAsync(categoryId) ??
            throw new Exceptions.Common.NotFoundException<Category>();

            var isExistCat = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryName == categoryName && c.Id != categoryId);
            if (isExistCat != null) throw new Exceptions.Common.IsAlreadyExistException<Category>();
            category.CategoryName = categoryName;
            await _context.SaveChangesAsync();
        }
    }
}