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
