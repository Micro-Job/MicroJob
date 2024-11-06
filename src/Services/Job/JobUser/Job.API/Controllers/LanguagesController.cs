using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job.Business.Dtos.LanguageDtos;
using Job.Business.Services.Language;
using Microsoft.AspNetCore.Mvc;

namespace Job.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LanguagesController : ControllerBase
    {
        readonly ILanguageService _service;

        public LanguagesController(ILanguageService service)
        {
            _service = service;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateLanguage([FromForm] LanguageCreateDto languageCreateDto)
        {
            await _service.CreateLanguageAsync(languageCreateDto);
            return Ok();
        }

        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> UpdateLanguage(string id, [FromForm] LanguageUpdateDto languageUpdateDto)
        {
            await _service.UpdateLanguageAsync(id, languageUpdateDto);
            return Ok();
        }
    }
}