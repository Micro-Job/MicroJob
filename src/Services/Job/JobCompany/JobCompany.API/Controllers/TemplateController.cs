using JobCompany.Business.Dtos.TemplateDtos;
using JobCompany.Business.Services.TemplateServices;
using Microsoft.AspNetCore.Mvc;

namespace JobCompany.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TemplateController(ITemplateService templateService) : ControllerBase
{
    private readonly ITemplateService _templateService = templateService;

    [HttpGet("[action]")]
    public async Task<IActionResult> GetAllTemplates(int skip = 1, int take = 9)
    {
        var result = await _templateService.GetAllTemplatesAsync(skip, take);
        return Ok(result);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetAllTemplatesWithQuestionCount(int skip = 1, int take = 10)
    {
        var data = await _templateService.GetAllTemplatesWithQuestionCountAsync(skip, take);
        return Ok(data);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetTemplateById(string templateId)
    {
        var result = await _templateService.GetTemplateByIdAsync(templateId);
        return Ok(result);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> CreateTemplate(TemplateCreateDto templateCreateDto)
    {
        await _templateService.CreateTemplateAsync(templateCreateDto);
        return Ok();
    }

    [HttpPut("[action]")]
    public async Task<IActionResult> UpdateTemplate(TemplateUpdateDto templateUpdateDto)
    {
        await _templateService.UpdateTemplateAsync(templateUpdateDto);
        return Ok();
    }

    [HttpDelete("[action]")]
    public async Task<IActionResult> DeleteTemplate(string templateId)
    {
        await _templateService.DeleteTemplateAsync(templateId);
        return Ok();
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> SaveExamToTemplate(string examId, string templateId)
    {
        await _templateService.SaveExamToTemplateAsync(examId, templateId);
        return Ok();
    }
}