using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobCompany.Business.Dtos.CategoryDtos;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using Microsoft.EntityFrameworkCore;

namespace JobCompany.Business.Services.CategoryServices
{
    public class CategoryService : ICategoryService
    {
        readonly JobCompanyDbContext _context;

        public CategoryService(JobCompanyDbContext context)
        {
            _context = context;
        }

        public async Task CreateCategoryAsync(CreateCategoryDto dto)
        {
            var existCategory = await _context.Categories.FindAsync(dto.CategoryName);
            if (existCategory != null) throw new Exceptions.Common.IsAlreadyExistException<Category>();

            var category = new Category
            {
                CategoryName = dto.CategoryName,
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(string id)
        {
            var categoryId = Guid.Parse(id);
            var category = await _context.Categories.FindAsync(categoryId)??
            throw new Exceptions.Common.NotFoundException<Category>();

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        public async Task<List<CategoryListDto>> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories.Select(c => new CategoryListDto
            {
                CategoryName = c.CategoryName
            }).ToListAsync();
            return categories;
        }

        public async Task UpdateCategoryAsync(string id, UpdateCategoryDto dto)
        {
            var categoryId = Guid.Parse(id);
            var category = await _context.Categories.FindAsync(categoryId) ??
            throw new Exceptions.Common.NotFoundException<Category>();

            var isExistCat = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryName == dto.CategoryName && c.Id != categoryId);
            if (isExistCat != null) throw new Exceptions.Common.IsAlreadyExistException<Category>();
            category.CategoryName = dto.CategoryName;
            await _context.SaveChangesAsync();
        }
    }
}