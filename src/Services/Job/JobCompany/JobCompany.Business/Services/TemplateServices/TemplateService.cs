using JobCompany.Business.Dtos.TemplateDtos;
using JobCompany.Business.Exceptions.Common;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JobCompany.Business.Services.TemplateServices;

public class TemplateService : ITemplateService
{
    private readonly Guid userGuid;
    private readonly JobCompanyDbContext _context;
    private readonly IHttpContextAccessor _contextAccessor;

    public TemplateService(JobCompanyDbContext context, IHttpContextAccessor contextAccessor)
    {
        _context = context;
        _contextAccessor = contextAccessor;
        userGuid = Guid.Parse(_contextAccessor.HttpContext.User.FindFirst(ClaimTypes.Sid).Value);
    }

    public async Task<ICollection<TemplateListDto>> GetAllTemplatesAsync(int skip = 1, int take = 9)
    {
        var company = await _context.Companies.FirstOrDefaultAsync(x => x.UserId == userGuid)
            ?? throw new SharedLibrary.Exceptions.NotFoundException<Company>();

        IQueryable<Template?> templatesQuery = _context.Vacancies
            .Where(v => v.CompanyId == company.Id && v.Exam != null && v.Exam.TemplateId != null)
            .Select(v => v.Exam.Template)
            .Distinct();

        var paginatedTemplates = await templatesQuery
            .Skip((skip - 1) * take)
            .Take(take)
            .ToListAsync();

        return paginatedTemplates.Select(x => new TemplateListDto
        {
            Id = x.Id,
            Name = x.Name
        }).ToList();
    }

    public async Task<TemplateGetByIdDto> GetTemplateByIdAsync(string templateId)
    {
        var guidTemplateId = Guid.Parse(templateId);

        var template = await _context.Templates.Include(x => x.Exams).FirstOrDefaultAsync(x => x.Id == guidTemplateId)
            ?? throw new NotFoundException<Template>();

        return new TemplateGetByIdDto
        {
            Id = template.Id,
            Name = template.Name,
            Exams = template.Exams
        };
    }

    public async Task SaveExamToTemplateAsync(string examId, string templateId)
    {
        var examGuidId = Guid.Parse(examId);
        var templateGuidId = Guid.Parse(templateId);

        var template = await _context.Templates.FirstOrDefaultAsync(x => x.Id == templateGuidId)
            ?? throw new NotFoundException<Template>();

        var exam = await _context.Exams.FirstOrDefaultAsync(x => x.Id == examGuidId)
            ?? throw new NotFoundException<Exam>();

        template.Exams.Add(exam);

        await _context.SaveChangesAsync();
    }

    public async Task CreateTemplateAsync(TemplateCreateDto templateCreateDto)
    {
        var company = await _context.Companies.FirstOrDefaultAsync(x => x.UserId == userGuid)
            ?? throw new SharedLibrary.Exceptions.NotFoundException<Company>();

        var template = new Template
        {
            Name = templateCreateDto.Title,
        };
        
        await _context.Templates.AddAsync(template);
        
        await _context.SaveChangesAsync();
    }

    public async Task UpdateTemplateAsync(TemplateUpdateDto templateUpdateDto)
    {
        var templateGuidId = Guid.Parse(templateUpdateDto.Id);

        var template = await _context.Templates.FirstOrDefaultAsync(x => x.Id == templateGuidId)
            ?? throw new NotFoundException<Template>();

        template.Name = templateUpdateDto.Name;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteTemplateAsync(string templateId)
    {
        var guidTemplateId = Guid.Parse(templateId);

        var template = await _context.Templates.FirstOrDefaultAsync(x => x.Id == guidTemplateId)
            ?? throw new NotFoundException<Template>();

        _context.Templates.Remove(template);

        await _context.SaveChangesAsync();
    }

    public async Task<ICollection<TemplateWithExamListDto>> GetAllTemplatesWithQuestionCountAsync(int skip = 1, int take = 10)
    {
        var company = await _context.Companies
            .FirstOrDefaultAsync(c => c.UserId == userGuid)
            ?? throw new SharedLibrary.Exceptions.NotFoundException<Company>();

        IQueryable<Template?> templatesQuery = _context.Vacancies
            .Where(v => v.CompanyId == company.Id)
            .Skip((skip - 1) * take)
            .Take(take)    
            .Select(v => v.Exam.Template);

        var templates = await templatesQuery
            .Select(template => new TemplateWithExamListDto
            {
                Id = template!.Id.ToString(),
                Name = template.Name,
                QuestionCount = template.Exams
                    .SelectMany(e => e.Questions)
                    .Count()
            })
            .ToListAsync();

        return templates;
    }
}