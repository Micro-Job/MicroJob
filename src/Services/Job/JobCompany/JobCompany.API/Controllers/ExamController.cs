using JobCompany.Business.Dtos.ExamDtos;
using JobCompany.Business.Services.ExamServices;
using Microsoft.AspNetCore.Mvc;

namespace JobCompany.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamController(IExamService examServices) : ControllerBase
    {
        private readonly IExamService _examServices = examServices;

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateExam(CreateExamDto dto)
        {
            await _examServices.CreateExamAsync(dto);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetExamByIdAsync(string examId, byte step)
        {
            var data = await _examServices.GetExamByIdAsync(examId, step);
            return Ok(data);
        }
    }
}