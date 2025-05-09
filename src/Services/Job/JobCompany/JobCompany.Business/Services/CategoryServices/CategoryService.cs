using JobCompany.Business.Dtos.CategoryDtos;
using JobCompany.Business.Exceptions.Common;
using JobCompany.Business.Extensions;
using JobCompany.Business.Statistics;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Helpers;
using SharedLibrary.HelperServices.Current;

namespace JobCompany.Business.Services.CategoryServices
{
    public class CategoryService(JobCompanyDbContext context, ICurrentUser _user) : ICategoryService
    {
        readonly JobCompanyDbContext _context = context;

        public async Task CreateCategoryAsync(CategoryCreateDto dto)
        {
            Category category = new();
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var categoryTranslations = dto.Categories.Select(x => new CategoryTranslation
            {
                CategoryId = category.Id,
                Language = x.language,
                Name = x.Name.Trim()
            }).ToList();

            await _context.CategoryTranslations.AddRangeAsync(categoryTranslations);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(string id)
        {
            var categoryId = Guid.Parse(id);
            var category = await _context.Categories.FindAsync(categoryId) ??
            throw new NotFoundException<Category>(MessageHelper.GetMessage("NOT_FOUND"));

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        public async Task<ICollection<CategoryListDto>> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories
                .IncludeTranslations()
            .Select(b => new CategoryListDto
            {
                Id = b.Id,
                CategoryName = b.GetTranslation(_user.LanguageCode,GetTranslationPropertyName.Name)
            })
            .ToListAsync();

            return categories;
        }

        public async Task<CategoryGetByIdDto> CategoryGetByIdAsync(Guid id)
        {
            var res = await _context.Categories
              .Where(x => x.Id == id)
              .Include(x => x.Translations)
              .Select(x => new CategoryGetByIdDto
              {
                  Id = x.Id,
                  CategoryTranslations = x.Translations.Select(t => new CategoryTranslationGetByIdDto
                  {
                      Id = t.Id,
                      Name = t.Name,
                      LanguageCode = t.Language
                  }).ToList()
              })
              .FirstOrDefaultAsync();

            return res;
        }

        public async Task UpdateCategoryAsync(List<CategoryUpdateDto> dtos)
        {
            var categoryTranslations = await _context.CategoryTranslations
            .Where(x => dtos.Select(b => b.Id).Contains(x.Id))
            .ToListAsync();

            foreach (var translation in categoryTranslations)
            {
                var category = dtos.FirstOrDefault(b => b.Id == translation.Id);
                if (category != null)
                {
                    translation.Name = category.Name;
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}