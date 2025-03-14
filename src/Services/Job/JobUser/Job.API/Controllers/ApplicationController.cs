using Job.Business.Dtos.ExamDtos;
using Job.Business.Services.Application;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Attributes;
using SharedLibrary.Enums;

namespace Job.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[AuthorizeRole(UserRole.SimpleUser)]
    public class ApplicationController(IUserApplicationService service) : ControllerBase
    {
        //[HttpGet("[action]")]
        //public async Task<IActionResult> GetExamIntro(string vacancyId)
        //{
        //    return Ok(await service.GetExamIntroAsync(vacancyId));
        //}

        [HttpGet("[action]")]
        public async Task<IActionResult> GetExamQuestions(Guid examId)
        {
            return Ok(await service.GetExamQuestionsAsync(examId));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> EvaluateExamAnswers(SubmitExamAnswersDto dto)
        {
            return Ok(await service.EvaluateExamAnswersAsync(dto));
        }
    }
}
