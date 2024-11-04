using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job.Business.Dtos.PersonDtos;
using Job.Business.Services.Person;
using Microsoft.AspNetCore.Mvc;

namespace Job.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonsController : ControllerBase
    {
        readonly IPersonService _service;

        public PersonsController(IPersonService service)
        {
            _service = service;
        }

        [HttpPost("PersonCreate")]
        public async Task<IActionResult> Post([FromForm] PersonCreateDto dto)
        {
            await _service.CreateAsync(dto);
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPut("PersonUpdate/{id}")]
        public async Task<IActionResult> Put([FromForm] PersonUpdateDto dto)
        { 
            await _service.UpdateAsync(dto);
            return NoContent();
        }
        
        [HttpGet("PersonGetAll")]
        public async Task<IActionResult> Get()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("PersonGetById/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }
    }
}