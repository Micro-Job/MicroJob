using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobCompany.Business.Dtos.ExamDtos.QuestionDtos;
using JobCompany.Business.Services.ExamServices.QuestionServices;
using Microsoft.AspNetCore.Mvc;

namespace JobCompany.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionService _service;

        public QuestionController(IQuestionService service)
        {
            _service = service;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateQuestionAsync(QuestionCreateDto dto)
        {
            await _service.CreateQuestionAsync(dto);
            return Ok();
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteQuestionAsync(string id)
        {
            await _service.DeleteQuestionAsync(id);
            return Ok();
        }
    }
}