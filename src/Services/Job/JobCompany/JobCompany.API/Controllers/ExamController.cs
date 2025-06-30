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
    //[AuthorizeRole(UserRole.CompanyUser, UserRole.EmployeeUser)]
    [Authorize]
    public class ExamController(ExamService examService) : ControllerBase
    {
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateExam([FromForm] CreateExamDto dto)
        {
            return Ok(await examService.CreateExamAsync(dto));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> UpdateExam([FromForm] UpdateExamDto dto)
        {
            await examService.UpdateExamAsync(dto);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetExamById(string examId)
        {
            return Ok(await examService.GetExamByIdAsync(examId));
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetExams(string? examName, int skip = 1, int take = 9)
        {
            return Ok(await examService.GetExamsAsync(examName,skip, take));
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

        [HttpGet("[action]")]
        public async Task<IActionResult> GetExamIntro(string examId, string vacancyId)
        {
            return Ok(await examService.GetExamIntroAsync(examId, vacancyId));
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetExamQuestions(string examId)
        {
            return Ok(await examService.GetExamQuestionsAsync(examId));
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetExamQuestionsForUser(string examId)
        {
            return Ok(await examService.GetExamQuestionsForUserAsync(examId));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> EvaluateExamAnswers(SubmitExamAnswersDto dto)
        {
            return Ok(await examService.EvaluateExamAnswersAsync(dto));
        }
    }
}
