using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job.Business.Dtos.NumberDtos;
using Job.Business.Services.Number;
using Microsoft.AspNetCore.Mvc;

namespace Job.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NumbersController : ControllerBase
    {
        readonly INumberService _service;

        public NumbersController(INumberService service)
        {
            _service = service;
        }

        [HttpPost("NumberCreate")]
        public async Task<IActionResult> Post([FromForm] NumberCreateDto dto)
        {
            await _service.CreateAsync(dto);
            return Ok();
        }

        [HttpPut("NumberUpdate")]
        public async Task<IActionResult> Put([FromForm] NumberUpdateDto dto)
        {
            await _service.UpdateAsync(dto);
            return Ok();
        }

        [HttpGet("NumberGetAll")]
        public async Task<IActionResult> Get()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("NumberGetById")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }
    }
}