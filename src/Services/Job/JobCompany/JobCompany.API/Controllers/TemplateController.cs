using JobCompany.Business.Dtos.TemplateDtos;
using JobCompany.Business.Services.TemplateServices;
using Microsoft.AspNetCore.Mvc;

namespace JobCompany.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TemplateController(ITemplateService service) : ControllerBase
{
    [HttpGet("[action]")]
    public async Task<IActionResult> GetAllTemplatesAsync(int skip = 1, int take = 9)
    {
        var result = await service.GetAllTemplatesAsync(skip, take);
        return Ok(result);
    }
    
    [HttpGet("[action]")]
    public async Task<IActionResult> GetTemplateByIdAsync(string templateId)
    {
        var result = await service.GetTemplateByIdAsync(templateId);
        return Ok(result);
    }

    [HttpPut("[action]")]
    public async Task<IActionResult> UpdateTemplateAsync(TemplateUpdateDto templateUpdateDto)
    {
        await service.UpdateTemplateAsync(templateUpdateDto);
        return Ok();
    }

    [HttpDelete("[action]")]
    public async Task<IActionResult> DeleteTemplateAsync(string templateId)
    {
        await service.DeleteTemplateAsync(templateId);
        return Ok();
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> SaveExamToTemplateAsync(string examId, string templateId)
    {
        await service.SaveExamToTemplateAsync(examId, templateId);
        return Ok();
    }
}
