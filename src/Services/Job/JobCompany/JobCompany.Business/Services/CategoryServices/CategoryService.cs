using JobCompany.Business.Dtos.CategoryDtos;
using JobCompany.Business.Dtos.Common;
using JobCompany.Business.Extensions;
using JobCompany.Business.Statistics;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Exceptions;
using SharedLibrary.HelperServices.Current;

namespace JobCompany.Business.Services.CategoryServices;

public class CategoryService(JobCompanyDbContext _context, ICurrentUser _user) 
{
    public async Task CreateCategoryAsync(CategoryCreateDto dto)
    {
        var category = new Category
        {
            Translations = dto.Categories.Select(x => new CategoryTranslation
            {
                Language = x.language,
                Name = x.Name.Trim()
            }).ToList()
        };

        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteCategoryAsync(string id)
    {
        var categoryId = Guid.Parse(id);
        var category = await _context.Categories.FindAsync(categoryId) ??
            throw new NotFoundException();

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
    }

    public async Task<ICollection<CategoryListDto>> GetAllCategoriesAsync()
    {
        var categories = await _context.Categories
            .Include(x=> x.Translations)
        .AsNoTracking()
        .Select(b => new CategoryListDto
        {
            Id = b.Id,
            CategoryName = b.Translations.GetTranslation(_user.LanguageCode, GetTranslationPropertyName.Name)
        })
        .ToListAsync();

        return categories;
    }

    /// <summary>
    /// Vakansiyalar siyahısının yan panelindəki filtr bölməsində istifadə olunan kateqoriyaların siyahısını gətirir.
    /// </summary>
    public async Task<DataListDto<CategoryListDto>> GetCategoriesPagedAsync(int skip, int take, string? name)
    {
        var query = _context.Categories.Include(x=> x.Translations).AsNoTracking();

        if (!string.IsNullOrEmpty(name))
        {
            name = name.Trim();
            query = query.Where(x => x.Translations.Any(t => t.Name.Contains(name) && t.Language == _user.LanguageCode));
        }
        var totalCount = await query.CountAsync();

        var categories = await query
            .Skip((skip - 1) * take)
            .Take(take)
            .Select(b => new CategoryListDto
            {
                Id = b.Id,
                CategoryName = b.Translations.GetTranslation(_user.LanguageCode, GetTranslationPropertyName.Name)
            }).ToListAsync();

        return new DataListDto<CategoryListDto> { Datas = categories, TotalCount = totalCount };
    }

    public async Task<CategoryGetByIdDto> CategoryGetByIdAsync(Guid id)
    {
        var res = await _context.Categories
          .Where(x => x.Id == id)
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
          .FirstOrDefaultAsync() ?? throw new NotFoundException();

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