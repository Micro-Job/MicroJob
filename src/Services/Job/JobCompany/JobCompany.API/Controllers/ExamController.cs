using JobCompany.Business.Dtos.ExamDtos;
using JobCompany.Business.Dtos.QuestionDtos;
using JobCompany.Business.Services.ExamServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Attributes;
using SharedLibrary.Enums;

namespace JobCompany.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExamController(ExamService examService) : ControllerBase
    {
        [AuthorizeRole(UserRole.CompanyUser, UserRole.EmployeeUser)]
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateExam([FromForm] CreateExamDto dto)
        {
            return Ok(await examService.CreateExamAsync(dto));
        }

        [AuthorizeRole(UserRole.CompanyUser, UserRole.EmployeeUser)]
        [HttpPost("[action]")]
        public async Task<IActionResult> UpdateExam([FromForm] UpdateExamDto dto)
        {
            await examService.UpdateExamAsync(dto);
            return Ok();
        }

        [Authorize]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetExamById(Guid examId)
        {
            return Ok(await examService.GetExamByIdAsync(examId));
        }

        [AuthorizeRole(UserRole.CompanyUser, UserRole.EmployeeUser)]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetExams(string? examName, int skip = 1, int take = 9)
        {
            return Ok(await examService.GetExamsAsync(examName,skip, take));
        }

        [Authorize]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetExamQuestionByStep(Guid examId, int step)
        {
            return Ok(await examService.GetExamQuestionByStepAsync(examId, step));
        }

        [AuthorizeRole(UserRole.CompanyUser, UserRole.EmployeeUser)]
        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteExam(Guid examId)
        {
            await examService.DeleteExamAsync(examId);
            return Ok();
        }

        [Authorize]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetExamIntro(Guid examId, Guid vacancyId)
        {
            return Ok(await examService.GetExamIntroAsync(examId, vacancyId));
        }

        [AuthorizeRole(UserRole.CompanyUser, UserRole.EmployeeUser)]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetExamQuestions(Guid examId)
        {
            return Ok(await examService.GetExamQuestionsAsync(examId));
        }

        [AuthorizeRole(UserRole.SimpleUser)]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetExamQuestionsForUser(Guid examId)
        {
            return Ok(await examService.GetExamQuestionsForUserAsync(examId));
        }

        [AuthorizeRole(UserRole.SimpleUser)]
        [HttpPost("[action]")]
        public async Task<IActionResult> EvaluateExamAnswers(SubmitExamAnswersDto dto)
        {
            return Ok(await examService.EvaluateExamAnswersAsync(dto));
        }
    }
}
