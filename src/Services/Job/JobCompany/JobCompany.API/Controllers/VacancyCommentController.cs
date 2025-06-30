using JobCompany.Business.Dtos.VacancyComment;
using JobCompany.Business.Services.VacancyComment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JobCompany.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VacancyCommentController(VacancyCommentService _service) : ControllerBase
    {
        [HttpPost("[action]")]
        public async Task<IActionResult> VacancyCommentCreate(VacancyCommentCreateDto dto)
        {
            await _service.VacancyCommentCreateAsync(dto);
            return Ok();
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> VacancyCommentUpdate(List<VacancyCommentUpdateDto> dtos)
        {
            await _service.VacancyCommentUpdateAsync(dtos);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> VacancyCommentGetAll()
        {
            return Ok(await _service.VacancyCommentGetAllAsync());
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> VacancyCommentGetDetail(string id)
        {
            return Ok(await _service.VacancyCommentGetDetailAsync(id));
        }

        [HttpDelete("[action]")]
        public async Task<IActionResult> VacancyCommentDelete(string id)
        {
            await _service.VacancyCommentDeleteAsync(id);
            return Ok();
        }
    }
}
