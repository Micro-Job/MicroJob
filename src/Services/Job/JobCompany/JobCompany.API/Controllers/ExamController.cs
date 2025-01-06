using JobCompany.Business.Dtos.ExamDtos;
using JobCompany.Business.Dtos.QuestionDtos;
using JobCompany.Business.Services.ExamServices;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Attributes;
using SharedLibrary.Enums;

namespace JobCompany.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AuthorizeRole(UserRole.CompanyUser, UserRole.EmployeeUser)]
    public class ExamController(IExamService examService) : ControllerBase
    {
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateExam([FromForm] CreateExamDto dto)
        {
            return Ok(await examService.CreateExamAsync(dto));
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetExamById(string examId)
        {
            return Ok(await examService.GetExamByIdAsync(examId));
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetExamQuestionByStep(string examId, int step)
        {
            return Ok(await examService.GetExamQuestionByStepAsync(examId, step));            
        }

        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteExam(string examId)
        {
            await examService.DeleteExamAsync(examId);
            return Ok();
        }
    }
}