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
    public class ExamController(ExamService _examService) : ControllerBase
    {
        [AuthorizeRole(UserRole.CompanyUser, UserRole.EmployeeUser)]
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateExam([FromForm] CreateExamDto dto)
        {
            return Ok(await _examService.CreateExamAsync(dto));
        }

        [AuthorizeRole(UserRole.CompanyUser, UserRole.EmployeeUser)]
        [HttpPost("[action]")]
        public async Task<IActionResult> UpdateExam([FromForm] UpdateExamDto dto)
        {
            await _examService.UpdateExamAsync(dto);
            return Ok();
        }

        [Authorize]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetExamById(Guid examId)
        {
            return Ok(await _examService.GetExamByIdAsync(examId));
        }

        [AuthorizeRole(UserRole.CompanyUser, UserRole.EmployeeUser)]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetExams(string? examName, int skip = 1, int take = 9)
        {
            return Ok(await _examService.GetExamsAsync(examName,skip, take));
        }

        [Authorize]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetExamQuestionByStep(Guid examId, int step)
        {
            return Ok(await _examService.GetExamQuestionByStepAsync(examId, step));
        }

        [AuthorizeRole(UserRole.CompanyUser, UserRole.EmployeeUser)]
        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteExam(Guid examId)
        {
            await _examService.DeleteExamAsync(examId);
            return Ok();
        }

        [Authorize]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetExamIntro(Guid examId, Guid vacancyId)
        {
            return Ok(await _examService.GetExamIntroAsync(examId, vacancyId));
        }

        [AuthorizeRole(UserRole.CompanyUser, UserRole.EmployeeUser)]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetExamQuestions(Guid examId)
        {
            return Ok(await _examService.GetExamQuestionsAsync(examId));
        }

        [AuthorizeRole(UserRole.SimpleUser)]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetExamQuestionsForUser(Guid examId)
        {
            return Ok(await _examService.GetExamQuestionsForUserAsync(examId));
        }

        [AuthorizeRole(UserRole.SimpleUser)]
        [HttpPost("[action]")]
        public async Task<IActionResult> EvaluateExamAnswers(SubmitExamAnswersDto dto)
        {
            return Ok(await _examService.EvaluateExamAnswersAsync(dto));
        }

        [AuthorizeRole(UserRole.CompanyUser, UserRole.EmployeeUser)]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetUserExam(Guid examId, Guid userId)
        {
            return Ok(await _examService.GetUserExamAsync(examId, userId));
        }
    }
}
