using JobCompany.Business.Dtos.TemplateDtos;

namespace JobCompany.Business.Services.TemplateServices;

public interface ITemplateService
{
    Task<ICollection<TemplateListDto>> GetAllTemplatesAsync(int skip = 1, int take = 9);
    Task<TemplateGetByIdDto> GetTemplateByIdAsync(string templateId);
    Task UpdateTemplateAsync(TemplateUpdateDto templateUpdateDto);
    Task DeleteTemplateAsync(string templateId);
    Task SaveExamToTemplateAsync(string examId, string templateId);
}