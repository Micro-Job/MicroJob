using JobCompany.Business.Dtos.TemplateDtos;
using JobCompany.Business.Exceptions.Common;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using Microsoft.EntityFrameworkCore;

namespace JobCompany.Business.Services.TemplateServices;

public class TemplateService(JobCompanyDbContext context) : ITemplateService
{
    public async Task<ICollection<TemplateListDto>> GetAllTemplatesAsync(int skip = 1, int take = 9)
    {
        var templates = await context.Templates.Select(x => new TemplateListDto
        {
            Id = x.Id,
            Name = x.Name,
        })
        .Skip(Math.Max(0, (skip - 1) * take))
        .Take(take)
        .ToListAsync();

        return templates;
    }

    public async Task<TemplateGetByIdDto> GetTemplateByIdAsync(string templateId)
    {
        var guidTemplateId = Guid.Parse(templateId);

        var template = await context.Templates.Include(x => x.Exams).FirstOrDefaultAsync(x => x.Id == guidTemplateId)
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

        var template = await context.Templates.FirstOrDefaultAsync(x => x.Id == templateGuidId)
            ?? throw new NotFoundException<Template>();

        var exam = await context.Exams.FirstOrDefaultAsync(x => x.Id == examGuidId)
            ?? throw new NotFoundException<Exam>();

        template.Exams.Add(exam);

        await context.SaveChangesAsync();
    }

    public async Task DeleteTemplateAsync(string templateId)
    {
        var guidTemplateId = Guid.Parse(templateId);

        var template = await context.Templates.FirstOrDefaultAsync(x => x.Id == guidTemplateId)
            ?? throw new NotFoundException<Template>();

        context.Templates.Remove(template);

        await context.SaveChangesAsync();
    }
}
