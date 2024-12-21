using JobCompany.Business.Dtos.ExamDtos;
using JobCompany.Business.Services.ExamServices;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Attributes;
using SharedLibrary.Enums;

namespace JobCompany.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamController(IExamService examService) : ControllerBase
    {
        [HttpPost("[action]")]
        [AuthorizeRole(UserRole.CompanyUser, UserRole.EmployeeUser)]
        public async Task<IActionResult> CreateExam(CreateExamDto dto)
        {
            await examService.CreateExamAsync(dto);
            return Ok();
        }

        [HttpGet]
        [AuthorizeRole(UserRole.CompanyUser, UserRole.EmployeeUser)]
        public async Task<IActionResult> GetExamById(string examId, byte step)
        {
            var data = await examService.GetExamByIdAsync(examId, step);
            return Ok(data);
        }
    }
}